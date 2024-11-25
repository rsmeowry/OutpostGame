﻿using System;
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

        public Dictionary<StateKey, int> PlayerProductCount = new();
        public Dictionary<string, int> PlayerProductDelta = new();
        public int Currency { get; private set; }

        public void Awake()
        {
            Instance = this;
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
            InitialLoadPlayerData();
            
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