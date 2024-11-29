using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using External.Util;
using Game.Citizens;
using Game.Network;
using Game.Player;
using Game.Production.Products;
using Game.Stocks;
using Game.Storage;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Game.State
{
    public class GameStateManager: MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        public Dictionary<string, int> PlayerProductCount = new();
        public Dictionary<string, int> PlayerProductDelta = new();

        public Dictionary<StateKey, int> FluidCount = new();
        public Dictionary<StateKey, int> FluidLimits = new();
        
        public int Currency { get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                foreach (var k in ProductRegistry.Instance.AllItems())
                {
                    IncreaseProduct(k, 100);
                }
                IncreaseCurrency(100);
            }
        }

        public IEnumerator LoadGameState()
        {
            if (!FileManager.Instance.Storage.FileExists("save.json", true))
            {
                FileManager.Instance.Storage.CreateDir(PlayerDataManager.Instance.playerName);
                var playerDict = new Dictionary<string, object>();
            
                playerDict.Add("resources", PreparePlayerData());
                playerDict.Add("name", PlayerDataManager.Instance.playerName);
                yield return NetworkManager.Instance.TryCreatePlayer(JsonConvert.SerializeObject(playerDict));
                // yield return SavePlayerData("Initially created player");
            }
            yield return InitialLoadPlayerData();
            
            yield return StockManager.Instance.EnsureAllMarketsCreated();
        }

        private IEnumerator InitialLoadPlayerData()
        {
            var o = new CoroutineOutput<string>();
            yield return NetworkManager.Instance.FetchPlayerData(o);
            var data = o.Value;
            var deserialized = JsonConvert.DeserializeObject<SavedPlayerData>(data);
            var playerData = deserialized.Resources;
            PlayerProductCount = playerData.ProductCounts.Select(it => (StateKey.FromString(it.Key).Formatted(), it.Value))
                .ToDictionary(x => x.Item1, x => x.Value);
            Currency = playerData.Currency;
            FluidCount = playerData.Fluids.ToDictionary(it => StateKey.FromString(it.Key), it => it.Value);
            FluidLimits = playerData.FluidLimits.ToDictionary(it => StateKey.FromString(it.Key), it => it.Value);
        }

        public UnityEvent<ProductChangedData> onProductChanged = new();

        public void IncreaseProduct(StateKey product, int amount)
        {
            PlayerProductCount[product.Formatted()] = PlayerProductCount.TryGetValue(product.Formatted(), out var pVal) ? pVal + amount : amount;
            var k = product.Formatted();
            PlayerProductDelta[k] = PlayerProductDelta.TryGetValue(k, out var value) ? value + amount : amount;
            onProductChanged.Invoke(new ProductChangedData()
            {
                Product = product,
                Delta = amount
            });
        }
        
        public UnityEvent currencyIncreaseEvent = new();

        public void ChangeCurrency(int delta, string desc, bool log)
        {
            Currency += delta;
            currencyIncreaseEvent.Invoke();

            if (!log) return;
            var dt = new Dictionary<string, int>();
            dt["currency:delta"] = delta;
            StartCoroutine(NetworkManager.Instance.PostPlayerLog(new PlayerLogData()
            {
                Comment = desc,
                PlayerName = PlayerDataManager.Instance.playerName,
                Deltas = new Dictionary<string, int>(dt)
            }));

        }

        public void IncreaseCurrency(int amount)
        {
            Currency += amount;
            currencyIncreaseEvent.Invoke();

            var dt = new Dictionary<string, int>();
            dt["currency:delta"] = amount;
            StartCoroutine(NetworkManager.Instance.PostPlayerLog(new PlayerLogData()
            {
                Comment = "Currency gain",
                PlayerName = PlayerDataManager.Instance.playerName,
                Deltas = new Dictionary<string, int>(dt)
            }));
        }
        
        public IEnumerator SavePlayerData(string log = "Product delta log forced by a save")
        {
            var logData = PlayerProductDelta
                .Select(it => (it.Key + ":delta", it.Value)).ToDictionary(it => it.Item1, it => it.Value);
            PlayerProductDelta.Clear();
            var playerDict = new Dictionary<string, object>();
            
            playerDict.Add("resources", PreparePlayerData());

            StartCoroutine(NetworkManager.Instance.PostPlayerLog(new PlayerLogData()
            {
                Comment = log,
                Deltas = logData,
                PlayerName = PlayerDataManager.Instance.playerName
            }));
            yield return NetworkManager.Instance.UpdatePlayerData(JsonConvert.SerializeObject(playerDict));
        }

        private SerializablePlayerData PreparePlayerData()
        {
            var productCounts = PlayerProductCount.Where(it => it.Value > 0)
                .Select(it => (it.Key, it.Value)).ToDictionary(it => it.Item1, it => it.Value);
            var fluidCounts = FluidCount.Where(it => it.Value > 0)
                .Select(it => (it.Key, it.Value)).ToDictionary(it => it.Item1.Formatted(), it => it.Value);
            var fluidLimits = FluidLimits
                .Select(it => (it.Key, it.Value)).ToDictionary(it => it.Item1.Formatted(), it => it.Value);
            if (!fluidLimits.ContainsKey(ProductRegistry.Water.Formatted()))
                fluidLimits[ProductRegistry.Water.Formatted()] = 10;

            return new SerializablePlayerData
            {
                ProductCounts = productCounts,
                Currency = Currency,
                FluidLimits = fluidLimits,
                Fluids = fluidCounts
            };
        }
    }

    public struct ProductChangedData
    {
        public StateKey Product;
        public int Delta;
    }

    internal struct SavedPlayerData
    {
        [JsonProperty("resources")]
        public SerializablePlayerData Resources;
    }

    public struct SerializablePlayerData
    {
        [JsonProperty("products")]
        public Dictionary<string, int> ProductCounts;
        [JsonProperty("currency")]
        public int Currency;
        [JsonProperty("fluids")]
        public Dictionary<string, int> Fluids;
        [JsonProperty("fluid_limits")]
        public Dictionary<string, int> FluidLimits;
    }
}