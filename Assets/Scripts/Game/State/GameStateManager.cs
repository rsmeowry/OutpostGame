using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Citizens;
using Game.Network;
using Game.Player;
using Game.Production.Products;
using Game.Stocks;
using Game.Storage;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.State
{
    public class GameStateManager: MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        public Dictionary<StateKey, int> PlayerProductCount = new Dictionary<StateKey, int>();
        public Dictionary<string, int> PlayerProductDelta = new Dictionary<string, int>();
        public int Currency { get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // TODO: this should generally be ran after we load our level
            // so we should move it there after
            StartCoroutine(LoadGameState());
        }

        public IEnumerator LoadGameState()
        {
            Debug.Log($"{CitizenNames.RandomMascName()} {CitizenNames.RandomMascName()} {CitizenNames.RandomMascName()}");
            Debug.Log($"{CitizenNames.RandomFemName()} {CitizenNames.RandomFemName()} {CitizenNames.RandomFemName()}");
            yield return new WaitForSeconds(2);
            InitialLoadPlayerData();

            // Load our markets
            yield return new WaitForSeconds(2);
            yield return StockManager.Instance.EnsureAllMarketsCreated();
        }

        private void InitialLoadPlayerData()
        {
            var data = FileManager.Instance.Storage.ReadFile($"{PlayerDataManager.Instance.playerName}/save.json");
            var deserialized = JsonConvert.DeserializeObject<SavedPlayerData>(data);
            var playerData = deserialized.Resources;
            PlayerProductCount = playerData.ProductCounts.Select(it => (StateKey.FromString(it.Key), it.Value))
                .ToDictionary(x => x.Item1, x => x.Value);
            Currency = playerData.Currency;
        }

        public void IncreaseProduct(StateKey product, int amount)
        {
            PlayerProductCount[product] = PlayerProductCount.TryGetValue(product, out var pVal) ? pVal + amount : amount;
            var k = product.Formatted();
            PlayerProductDelta[k] = PlayerProductDelta.TryGetValue(k, out var value) ? value + amount : amount;
        }

        public void IncreaseCurrency(int amount)
        {
            Currency += amount;

            var dt = new Dictionary<string, int>();
            dt["currency:delta"] = amount;
            StartCoroutine(NetworkManager.Instance.PostPlayerLog(new PlayerLogData()
            {
                Comment = "Currency gain",
                PlayerName = PlayerDataManager.Instance.playerName,
                Deltas = new Dictionary<string, int>(dt)
            }));
        }
        
        public IEnumerator SavePlayerData()
        {
            var logData = PlayerProductDelta
                .Select(it => (it.Key + ":delta", it.Value)).ToDictionary(it => it.Item1, it => it.Value);
            PlayerProductDelta.Clear();
            var playerDict = new Dictionary<string, object>();
            
            playerDict.Add("resources", PreparePlayerData());

            StartCoroutine(NetworkManager.Instance.PostPlayerLog(new PlayerLogData()
            {
                Comment = "Product delta log forced by a save",
                Deltas = logData,
                PlayerName = PlayerDataManager.Instance.playerName
            }));
            yield return NetworkManager.Instance.UpdatePlayerData(JsonConvert.SerializeObject(playerDict));
        }

        private SerializablePlayerData PreparePlayerData()
        {
            var productCounts = PlayerProductCount.Where(it => it.Value > 0)
                .Select(it => (it.Key.Formatted(), it.Value)).ToDictionary(it => it.Item1, it => it.Value);
            return new SerializablePlayerData
            {
                ProductCounts = productCounts,
                Currency = Currency
            };
        }
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
    }
}