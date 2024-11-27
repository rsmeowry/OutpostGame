using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using External.Util;
using Game.DayNight;
using Game.Network;
using Game.News;
using Game.Player;
using Game.Production.Products;
using Game.State;
using Game.Storage;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Game.Stocks
{
    public class StockManager: MonoBehaviour
    {
        public static StockManager Instance { get; private set; }
        private static readonly LoggerHandle _log = LoggerHandle.LogHandle<StockManager>();

        public List<MarketData> Markets { get; private set; } = new();

        public UnityEvent onFluctuate = new();

        private IMarketDoom _impendingDoom;

        private void Awake()
        {
            Instance = this;
        }

        public void ImpendingDoom(IMarketDoom newDoom)
        {
            _impendingDoom = newDoom;
            NewsManager.Instance.PushNews(newDoom.GetNewsPrediction());
        }

        [ContextMenu("Test/Test Try Impending Doom")]
        private void ShouldDoImpendingDoom()
        {
            if (_impendingDoom != null)
                return;

            var dayNumber = DayCycleManager.Instance.days + 1;
            var shouldHappen = dayNumber >= 2 && Random.Range(0f, 1f) * (Mathf.Min(7f, dayNumber) / 10f) > 0.5f;
            if (shouldHappen)
            {
                var randomItem = ProductRegistry.Instance.RandomItem();
                var shouldSoar = Random.Range(0f, 1f) < 0.3f;
                if (shouldSoar)
                {
                    var soarAmount = 1 + Random.Range(0.7f, 1.9f);
                    Debug.Log("TRIGGERING MARKET SOAR");
                    ImpendingDoom(new MarketSoar
                    {
                        Item = randomItem,
                        Power = soarAmount
                    });
                }
                else
                {
                    var crashAmount = 1 + Random.Range(0.6f, 0.9f);
                    Debug.Log("TRIGGERING MARKET CRASH");
                    ImpendingDoom(new MarketCrash
                    {
                        Item = randomItem,
                        Power = crashAmount
                    });
                }
            }
        }

        private void Start()
        {
            DayCycleManager.Instance.onDayChanged.AddListener(() =>
            {
                StartCoroutine(Fluctuate(true));
            });
        }

        private void SetupMarkets()
        {
            Markets.Add(new MarketData("stock_market", 
                ProductRegistry.Entries
                    .Select(it => (StateKey.FromString(it.Key), new SoldProductData(it.Value.BaseStock, it.Value.BuyPrice, it.Value.SellPrice)))
                    .ToArray()
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
            var market = Markets[0];
            var delta = market.Offers.ToDictionary(entry => entry.Key, 
                entry => (SoldProductData) entry.Value.Clone());
            market.Fluctuate();
            
            onFluctuate.Invoke();

            if (_impendingDoom != null)
            {
                _impendingDoom.Execute(market);
                _impendingDoom = null;
            }

            market.FixUnrealisticValues();
            
            if(shouldRestock)
                market.Restock();

            market.Freeze();
            
            var deltas = delta.Select(k => (k.Key.Formatted(), k.Value.Delta(market.Offers[k.Key]))).ToDictionary(x => x.Item1, x => x.Item2);
            StartCoroutine(NetworkManager.Instance.PostMarketLogs(new MarketLogData()
            {
                Comment = "Daily market fluctuations" + (shouldRestock ? " (+ weekly restock)" : ""),
                Deltas = deltas,
                MarketId = market.MarketId,
                PlayerName = PlayerDataManager.Instance.playerName
            }));

            yield return NetworkManager.Instance.UpdateMarketData(market);
            
            // saving markets locally now
            FileManager.Instance.Storage.SaveString(
                Path.Combine($"{PlayerDataManager.Instance.playerName}", "markets.json"),
                JsonConvert.SerializeObject(Markets.Select(it => it.PrepareData()).ToList())
            );
            
            // Now we check if we want to do an impending market crash
            // TODO: probably a good idea to save it to data, so players dont abuse it
            ShouldDoImpendingDoom();
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
            // because now we need to fetch ALL markets from the server
            // since we have server authority
            yield return NetworkManager.Instance.FetchAllMarkets(Markets.Select(it => it.PrepareData()).ToList());
        }
    }
    
    public class MarketData
    {
        public string MarketId;
        public Dictionary<StateKey, SoldProductData> Offers;
        public Stack<Dictionary<StateKey, SoldProductData>> FrozenOffers = new();
        private readonly Dictionary<StateKey, int> _boughtAmounts = new();
        private readonly Dictionary<StateKey, int> _soldAmounts = new();

        public MarketData(string id, params (StateKey, SoldProductData)[] offers)
        {
            MarketId = id;
            Offers = offers.ToDictionary(it => it.Item1, it => it.Item2);
            var frozen = Offers.ToDictionary(entry => entry.Key, 
                entry => (SoldProductData) entry.Value.Clone());
            FrozenOffers.Push(frozen);

        }

        public BuyReceipt Buy(StateKey item, int amount)
        {
            var available = Offers[item];
            var cost = (amount * Mathf.RoundToInt(available.BuyPrice));
            return new BuyReceipt
            {
                TotalCost = cost,
                BoughtCount = amount,
                Item = item
            };
        }

        public void DoBuy(BuyReceipt receipt)
        {
            _boughtAmounts.Increment(receipt.Item, receipt.BoughtCount);
            GameStateManager.Instance.IncreaseProduct(receipt.Item, receipt.BoughtCount);
        }

        public SellReceipt Sell(StateKey item, int amount)
        {
            var profit = amount * Mathf.RoundToInt(Offers[item].SellPrice);
            return new SellReceipt
            {
                TotalProfit = profit,
                Item = item,
                SoldCount = amount
            };
        }

        public void DoSell(SellReceipt receipt)
        {
            _soldAmounts.Increment(receipt.Item, receipt.SoldCount);
            GameStateManager.Instance.IncreaseProduct(receipt.Item, -receipt.SoldCount);
        }

        public Dictionary<StateKey, (float, float)> CalculateChanges()
        {
            float CalculateBuyIncrease(float bought, float sold)
            {
                var boughtPower =
                    (1.97f * Mathf.Pow(Mathf.Log(1 + 0.019f * bought), 5) + Mathf.Pow(0.01f * bought, 2.9f)) /
                    Mathf.Pow(bought, 0.7f);
                return 1 + (boughtPower / Mathf.Pow((Mathf.Log(2 + sold / 300f)), 0.5f)) / 50f;
            }

            float CalculateSellDecrease(float bought, float sold)
            {
                if (sold < 100)
                    return 1.5f;
                var powerA =
                    (1.97f * Mathf.Pow(Mathf.Log(1 + 0.019f * sold), 5) + Mathf.Pow(0.01f * sold, 2.9f)) /
                    Mathf.Pow(sold, 0.2f);
                var powerB = powerA / 200f / Mathf.Pow(Mathf.Log(2 + bought / 200f), 9.9f);
                return powerB / 37.5f;
            }

            return Offers.ToDictionary(it => it.Key, it =>
            {
                var k = it.Key;
                var hasBought = _boughtAmounts.ContainsKey(k);
                var hasSold = _soldAmounts.ContainsKey(k);
                if (hasBought && hasSold)
                {
                    // calculate both
                    var bought = _boughtAmounts[k];
                    var sold = _soldAmounts[k];
                    return (CalculateBuyIncrease(bought, sold), CalculateSellDecrease(bought, sold));
                }

                if (hasBought)
                {
                    var bought = _boughtAmounts[k];
                    var sold = 0f;
                    return (CalculateBuyIncrease(bought, sold), 1.7f);
                }

                if (hasSold)
                {
                    var bought = 0f;
                    var sold = _soldAmounts[k];
                    return (0.8f, CalculateSellDecrease(bought, sold));
                }

                return (1f, 1f);
            });
        }

        public void FixUnrealisticValues()
        {
            // used to balance things out, so that buy price is greater than sell price
            foreach (var (k, v) in Offers)
            {
                var baseData = ProductRegistry.Entries[k.Formatted()];
                var buyCap = baseData.BuyPrice * 8f;
                var sellCap = baseData.SellPrice * 8f;
                if (v.BuyPrice <= v.SellPrice)
                {
                    Offers[k].BuyPrice = v.SellPrice * 1.5f;
                }

                Offers[k].BuyPrice = Mathf.Min(buyCap, v.BuyPrice);
                Offers[k].SellPrice = Mathf.Min(sellCap, v.SellPrice);
            }
        }

        // Fluctuations happen on daily basis
        public void Fluctuate()
        {
            var deltas = CalculateChanges();
            _boughtAmounts.Clear();
            _soldAmounts.Clear();
            foreach (var (k, v) in Offers)
            {
                Offers[k].BuyPrice = v.BuyPrice * Random.Range(0.86f, 1.12f) * deltas[k].Item1;
                Offers[k].SellPrice = v.SellPrice * Random.Range(0.86f, 1.12f) * deltas[k].Item2;
            }
        }

        // Freezes values for stock data
        public void Freeze()
        {
            var frozen = Offers.ToDictionary(entry => entry.Key, 
                entry => (SoldProductData) entry.Value.Clone());
            FrozenOffers.Push(frozen);
        }

        // Restocks happen every week
        public void Restock()
        {
            // TODO: actual restocks
            foreach (var (k, v) in Offers)
            {
                Offers[k].RemainingStock += Random.Range(300, 600);
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

    public class GlobalMarketSoar : IMarketDoom
    {
        public NewsEntry GetNewsPrediction()
        {
            return NewsLines.globalUplifting;
        }

        public void Execute(MarketData market)
        {
            market.Offers = market.Offers.ToDictionary(it => it.Key, it =>
            {
                var offer = it.Value;
                offer.BuyPrice *= (0.3f);
                offer.SellPrice /= (0.4f);
                return offer;
            });
        }
    }

    public class GlobalMarketCrash : IMarketDoom
    {
        public NewsEntry GetNewsPrediction()
        {
            return NewsLines.globalDoom;
        }

        public void Execute(MarketData market)
        {
            market.Offers = market.Offers.ToDictionary(it => it.Key, it =>
            {
                var offer = it.Value;
                offer.BuyPrice /= (0.35f);
                offer.SellPrice *= (0.45f);
                return offer;
            });
        }
    }

    public class MarketCrash: IMarketDoom
    {
        public StateKey Item;
        public float Power;
        public NewsEntry GetNewsPrediction()
        {
            var choice = Rng.Choice(NewsLines.marketCrash[Item]);
            return new NewsEntry
            {
                Title = choice.Item1,
                MoreInfo = choice.Item2
            };
        }

        public void Execute(MarketData market)
        {
            market.Offers = market.Offers.ToDictionary(it => it.Key, it =>
            {
                var offer = it.Value;
                offer.BuyPrice *= Power;
                offer.SellPrice /= (Power - 0.6f);
                return offer;
            });
        }
    }
    
    public class MarketSoar: IMarketDoom
    {
        public StateKey Item;
        public float Power;
        
        public NewsEntry GetNewsPrediction()
        {
            var choice = Rng.Choice(NewsLines.marketSoar[Item]);
            return new NewsEntry
            {
                Title = choice.Item1,
                MoreInfo = choice.Item2
            };
        }

        public void Execute(MarketData market)
        {
            market.Offers = market.Offers.ToDictionary(it => it.Key, it =>
            {
                var offer = it.Value;
                offer.BuyPrice /= (Power - 0.6f);
                offer.SellPrice *= (Power);
                return offer;
            });
        }
    }

    public interface IMarketDoom
    {
        public NewsEntry GetNewsPrediction();
        public void Execute(MarketData market);
    }

    public struct BuyReceipt
    {
        public int BoughtCount;
        public int TotalCost;
        public StateKey Item;
    }

    public struct SellReceipt
    {
        public int SoldCount;
        public int TotalProfit;
        public StateKey Item;
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