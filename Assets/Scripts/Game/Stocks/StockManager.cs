using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using External.Util;
using Game.Network;
using Game.Player;
using Game.Production.Products;
using Game.State;
using Game.Storage;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Stocks
{
    public class StockManager: MonoBehaviour
    {
        public static StockManager Instance { get; private set; }
        private static readonly LoggerHandle _log = LoggerHandle.LogHandle<StockManager>();

        public List<MarketData> Markets { get; private set; } = new();

        private void Awake()
        {
            Instance = this;
        }

        private void SetupMarkets()
        {
            Markets.Add(new MarketData("stock_market", 
                (ProductRegistry.CopperOre, new SoldProductData(3000, 12f, 24f)),
                (ProductRegistry.IronOre, new SoldProductData(2000, 9f, 14f))
            ));
        }

        [ContextMenu("Test/Fluctuate")]
        private void TestFluctuate()
        {
            StartCoroutine(Fluctuate(false));
        }
        
        [ContextMenu("Test/Fluctuate + Restock")]
        private void TestFluctuateRestock()
        {
            StartCoroutine(Fluctuate(true));
        }

        [ContextMenu("Util/Market Reset")]
        private void ResetMarkets()
        {
            foreach (var market in Markets)
            { 
                StartCoroutine(NetworkManager.Instance.UpdateMarketData(market));
            }
        }

        public IEnumerator SaveAll()
        {
            foreach (var market in Markets)
            {
                var delta = market.Offers.ToDictionary(entry => entry.Key, 
                    entry => (SoldProductData) entry.Value.Clone());
                var deltas = delta.Select(k => (k.Key.Formatted(), k.Value.Delta(market.Offers[k.Key]))).ToDictionary(x => x.Item1, x => x.Item2);
                StartCoroutine(NetworkManager.Instance.PostMarketLogs(new MarketLogData()
                {
                    Comment = "[SAVE] Save-triggered update",
                    Deltas = deltas,
                    MarketId = market.MarketId,
                    PlayerName = PlayerDataManager.Instance.playerName
                }));

                yield return NetworkManager.Instance.UpdateMarketData(market);
            }
            
            // saving markets locally now
            FileManager.Instance.Storage.SaveString(
                Path.Combine($"{PlayerDataManager.Instance.playerName}", "markets.json"),
                JsonConvert.SerializeObject(Markets.Select(it => it.PrepareData()).ToList())
            );
        }

        // Daily fluctuations
        public IEnumerator Fluctuate(bool shouldRestock)
        {
            foreach (var market in Markets)
            {
                var delta = market.Offers.ToDictionary(entry => entry.Key, 
                    entry => (SoldProductData) entry.Value.Clone());
                market.Fluctuate();
                if(shouldRestock)
                    market.Restock();
                var deltas = delta.Select(k => (k.Key.Formatted(), k.Value.Delta(market.Offers[k.Key]))).ToDictionary(x => x.Item1, x => x.Item2);
                StartCoroutine(NetworkManager.Instance.PostMarketLogs(new MarketLogData()
                {
                    Comment = "Daily market fluctuations" + (shouldRestock ? " (+ weekly restock)" : ""),
                    Deltas = deltas,
                    MarketId = market.MarketId,
                    PlayerName = PlayerDataManager.Instance.playerName
                }));

                yield return NetworkManager.Instance.UpdateMarketData(market);
            }
            
            // saving markets locally now
            FileManager.Instance.Storage.SaveString(
                Path.Combine($"{PlayerDataManager.Instance.playerName}", "markets.json"),
                JsonConvert.SerializeObject(Markets.Select(it => it.PrepareData()).ToList())
            );
        }

        public IEnumerator EnsureAllMarketsCreated()
        {
            // first creating all basic markets
            SetupMarkets();
            
            // reading market lockfile from local storage first
            var marketsThatNeedCreating = Markets.ToDictionary(it => it.MarketId, it => it);
            var path = Path.Combine($"{PlayerDataManager.Instance.playerName}", "markets.json");
            if (FileManager.Instance.Storage.FileExists(path))
            {
                // checking all markets
                var localMarkets = JsonConvert.DeserializeObject<List<SerializedMarketData>>(FileManager.Instance.Storage.ReadFile(path));
                // TODO: crashes if the file is empty
                foreach (var local in localMarkets)
                {
                    marketsThatNeedCreating.Remove(local.MarketId);
                }

                foreach (var needed in marketsThatNeedCreating)
                {
                    _log.Dbg($"Uploading shop {needed}");
                    yield return NetworkManager.Instance.CreateMarket(needed.Value.PrepareData());
                }
            }
            else
            {
                // nothing exists we need to make new markets altogether
                foreach (var needed in marketsThatNeedCreating)
                {
                    _log.Dbg($"Uploading shop {needed}");
                    yield return NetworkManager.Instance.CreateMarket(needed.Value.PrepareData());
                }
            }
            
            // HOWEVER THATS NOT ALL
            // since now we need to fetch ALL markets from the server
            // since we have server authority
            yield return NetworkManager.Instance.FetchAllMarkets(Markets.Select(it => it.PrepareData()).ToList());
        }
    }
    
    public class MarketData
    {
        public string MarketId;
        public Dictionary<StateKey, SoldProductData> Offers;

        public MarketData(string id, params (StateKey, SoldProductData)[] offers)
        {
            MarketId = id;
            Offers = offers.ToDictionary(it => it.Item1, it => it.Item2);
        }

        // Fluctuations happen on daily basis
        public void Fluctuate()
        {
            // TODO: actual fluctuations going on there
            foreach (var (k, v) in Offers)
            {
                Offers[k].SellPrice = v.SellPrice * Random.Range(0.9f, 1.1f);
                Offers[k].BuyPrice = v.BuyPrice * Random.Range(0.9f, 1.1f);
            }
        }

        // Restocks happen every week
        public void Restock()
        {
            // TODO: actual restocks
            foreach (var (k, v) in Offers)
            {
                Offers[k].RemainingStock += Random.Range(3000, 4000);
            }
        }

        internal SerializedMarketData PrepareData()
        {
            return new SerializedMarketData
            {
                MarketId = MarketId,
                Offers = Offers.ToDictionary(it => it.Key.Formatted(), it => it.Value)
            };
        }
    }
    
    public struct SerializedMarketData
    {
        [JsonProperty("name")]
        public string MarketId;
        [JsonProperty("resources")] 
        public Dictionary<string, SoldProductData> Offers;
    }
    
    public class SoldProductData: ICloneable
    {
        [JsonProperty("remaining")]
        public int RemainingStock;
        [JsonProperty("buy")]
        public float BuyPrice;
        [JsonProperty("sell")]
        public float SellPrice;

        public SoldProductData(int remainingStock, float buyPrice, float sellPrice)
        {
            RemainingStock = remainingStock;
            BuyPrice = buyPrice;
            SellPrice = sellPrice;
        }

        public SoldProductData Delta(SoldProductData other)
        {
            return new SoldProductData(other.RemainingStock - this.RemainingStock, other.BuyPrice - this.BuyPrice,
                other.SellPrice - this.SellPrice);
        }

        public object Clone()
        {
            return new SoldProductData(RemainingStock, BuyPrice, SellPrice);
        }
    }

    [SerializeField]
    public struct StocksLogsData
    {
        
    }
}