using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using External.Network;
using External.Util;
using Game.Player;
using Game.Stocks;
using Game.Storage;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Network
{
    public class NetworkManager: MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }
        
        private static readonly LoggerHandle _log = LoggerHandle.LogHandle<NetworkManager>();
        private NetConnection _conn;
        private bool _offline = false;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
            StartCoroutine(SetupConnection());
            Instance = this;
        }

        [ContextMenu("Test/Resource Update")]
        public void TestResources()
        {
            var newResources = new Dictionary<string, object>();
            newResources.Add("hi", "hello");
            newResources.Add("two", Random.Range(1, 100000));
            
            StartCoroutine(UpdatePlayerData(JsonConvert.SerializeObject(new ReqCreatePlayer()
            {
                Name = PlayerDataManager.Instance.playerName,
                Values = newResources
            })));
        }
        
        [ContextMenu("Test/Resource Fetch")]
        public void TestFetchResources()
        {
            StartCoroutine(__TestFetch());
        }

        private IEnumerator __TestFetch()
        {
            var handle = new CoroutineOutput<string>();
            yield return FetchPlayerData(handle);
            _log.Dbg(handle.Value);
        }

        public IEnumerator UpdateMarketData(MarketData market)
        {
            if (_offline)
                yield break;
            var data = market.Offers.ToDictionary(it => it.Key.Formatted(), it => it.Value);
            var dict = new Dictionary<string, Dictionary<string, SoldProductData>>();
            dict["resources"] = data;
            yield return _conn.Post($"/players/{PlayerDataManager.Instance.playerName}/shops/{market.MarketId}/", dict,
                new NetResponseHandle(), "PUT");
        }

        public IEnumerator PostMarketLogs(MarketLogData logData)
        {
            if (_offline)
                yield break;
            // Thankfully we don't need to post any market logs while offline!

            var data = JsonConvert.SerializeObject(logData);
            yield return _conn.Post("/logs/", data, new NetResponseHandle(), raw: true);
        }

        // Here fallback is used if we are offline, so that we can update any missing markets either way
        public IEnumerator FetchAllMarkets(List<SerializedMarketData> fallback)
        {
            if (_offline)
            {
                FileManager.Instance.Storage.SaveString(
                    Path.Combine($"{PlayerDataManager.Instance.playerName}", "markets.json"),
                    JsonConvert.SerializeObject(fallback)
                );
                yield break;
            }

            var handle = new NetResponseHandle();
            yield return _conn.Get($"/players/{PlayerDataManager.Instance.playerName}/shops/", handle);
            FileManager.Instance.Storage.SaveString(
                Path.Combine($"{PlayerDataManager.Instance.playerName}", "markets.json"),
                handle.Value
            );  // TODO: a null value is highly unlikely here, should handle it either way
        }
        
        public IEnumerator CreateMarket(SerializedMarketData initial)
        {
            if (_offline)
                yield break;
            // We don't need to save any markets offline there, it's done in a different spot

            var data = JsonConvert.SerializeObject(initial);
            yield return _conn.Post($"/players/{PlayerDataManager.Instance.playerName}/shops/", data,
                new NetResponseHandle(), raw: true);
            
        }

        public IEnumerator PostPlayerLog(PlayerLogData logData)
        {
            if (_offline)
                yield break;
            // Thankfully we don't need to post any players logs offline!

            var data = JsonConvert.SerializeObject(logData);
            yield return _conn.Post("/logs/", data, new NetResponseHandle(), raw: true);
        }
        
        public IEnumerator UpdatePlayerData(string newData)
        {
            // Always back up data locally too!
            FileManager.Instance.Storage.SaveString(Path.Combine($"{PlayerDataManager.Instance.playerName}", "save.json"), newData);
            if (_offline)
                yield break;
            
            yield return _conn.Post($"/players/{PlayerDataManager.Instance.playerName}/", newData, new NetResponseHandle(), "PUT", true);
        }

        public IEnumerator FetchPlayerData(CoroutineOutput<string> data)
        {
            if (_offline)
            {
                // Local data first
                var o = FileManager.Instance.Storage.ReadFile(Path.Combine($"{PlayerDataManager.Instance.playerName}", "save.json"));
                data.End(o);
                yield break;
            }

            var handle = new NetResponseHandle();
            yield return _conn.Get($"/players/{PlayerDataManager.Instance.playerName}/", handle);
            if (handle.Value != null)
            {
                data.End(handle.Value);
            }
            else
            {
                // Fall back to local data
                var o = FileManager.Instance.Storage.ReadFile(Path.Combine($"{PlayerDataManager.Instance.playerName}", "save.json"));
                data.End(o);
                yield break;
            }
        }

        // Attempts to create player. if it fails fetches data instead
        public IEnumerator TryCreatePlayer(string seedData)
        {
            FileManager.Instance.Storage.SaveString(Path.Combine($"{PlayerDataManager.Instance.playerName}", "save.json"), seedData);
            
            if (_offline)
                yield break;

            // trying to upload data
            
            var handle = new NetResponseHandle();
            yield return _conn.Post("/players/", seedData, handle, raw: true);
            if (handle.Error == null) yield break;
            
            // if it fails then it already exists, lets fetch it instead
            var ou = new CoroutineOutput<string>();
            yield return FetchPlayerData(ou);
                
            FileManager.Instance.Storage.SaveString(Path.Combine($"{PlayerDataManager.Instance.playerName}", "save.json"), ou.Value);
        }

        private IEnumerator SetupConnection()
        {
            _conn = new NetConnection();
            yield return _conn.Setup();
            _offline = true; // TODO: !_conn.InternetAvailable;

            if (!FileManager.Instance.Storage.FileExists(PlayerDataManager.Instance.playerName)) 
            {
                // Player does not exist locally! Maybe does not exist at all. Let's create a new data entry
                // TODO: this is a BAD idea!!! Remake this when the game is going to release
                
                // First we save them locally

                var seedValues = new Dictionary<string, object>();
                seedValues.Add("test", "hello world");
                seedValues.Add("another", Random.Range(1, 10000));
                var seedData = new ReqCreatePlayer()
                {
                    Name = PlayerDataManager.Instance.playerName,
                    Values = seedValues
                }; // TODO: put some actual data here
                var converted = JsonConvert.SerializeObject(seedData, Formatting.Indented);
                yield return TryCreatePlayer(converted);
            }
        }
    }
    
    internal struct ReqCreatePlayer
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("resources")]
        public Dictionary<string, object> Values;
    }

    public struct PlayerLogData
    {
        [JsonProperty("comment")]
        public string Comment;
        [JsonProperty("player_name")]
        public string PlayerName;
        [JsonProperty("resources_changed")]
        public Dictionary<string, int> Deltas;
    }

    public struct MarketLogData
    {
        [JsonProperty("comment")]
        public string Comment;
        [JsonProperty("player_name")]
        public string PlayerName;
        [JsonProperty("shop_name")]
        public string MarketId;
        [JsonProperty("resources_changed")]
        public Dictionary<string, SoldProductData> Deltas;
    }
}