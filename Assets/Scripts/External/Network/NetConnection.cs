using System.Collections;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using External.Util;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace External.Network
{
    public class NetConnection
    {
        private static readonly string GameId = "e87e2049-9d94-4969-a644-ca800ab8d2dd";
        private static readonly string BaseUrl = $"https://2025.nti-gamedev.ru/api/games/{GameId}";

        private static LoggerHandle _log = LoggerHandle.LogHandle<NetConnection>();

        public bool InternetAvailable { get; private set; }

        public NetConnection()
        {
            
        }

        public IEnumerator Post(string path, object data, NetResponseHandle handle, string mtd = "POST", bool raw = false)
        {
            if (!Connected(handle))
                yield break;
            
            _log.Dbg($"OUT -> {path} {mtd}", data);
            
            var req = new UnityWebRequest($"{BaseUrl}{path}", mtd);
            var bodyRaw = Encoding.UTF8.GetBytes(raw ? (string) data : JsonConvert.SerializeObject(data));
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("User-Agent", "Looper");
            yield return req.SendWebRequest();
            
            if (req.result != UnityWebRequest.Result.Success)
            {
                var err = req.error;
                _log.Warn($"Failed to send POST request: {err}");
                handle.Err(err);
            }
            else
            {
                handle.Success(Encoding.UTF8.GetString(req.downloadHandler.data));
            }
        }

        public IEnumerator Get(string path, NetResponseHandle handle)
        {
            if (!Connected(handle))
                yield break;

            _log.Dbg($"OUT -> {path} GET");

            var req = UnityWebRequest.Get($"{BaseUrl}{path}");
            req.SetRequestHeader("User-Agent", "Looper");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                var err = req.error;
                _log.Warn($"Failed to send GET request: {err}");
                handle.Err(err);
            }
            else
            {
                handle.Success(Encoding.UTF8.GetString(req.downloadHandler.data));
            }
        }

        public IEnumerator Setup()
        {
            yield return CheckInternetConnectivity();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Connected(NetResponseHandle handle)
        {
            if (InternetAvailable) return true;
            
            handle.NoInternet();
            return false;

        }

        private IEnumerator CheckInternetConnectivity()
        {
            _log.Log("Checking availability of internet connection...");
            var req = UnityWebRequest.Get(BaseUrl + $"/players/");
            req.SetRequestHeader("User-Agent", "Looper");
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                _log.Warn("Failed to establish internet connection. Moving to offline mode...");
                InternetAvailable = false;
            }
            else
            {
                InternetAvailable = true;
            }
        }

        public static NetResponseHandle Resp()
        {
            return new NetResponseHandle
            {
                Value = "null",
                Error = "null",
                Done = false
            };
        }
    }

    public class NetResponseHandle
    {
        public string Value { get; internal set; }
        public string Error { get; internal set; }
        public bool Done { get; internal set; }

        internal void NoInternet()
        {
            Error = "Internet is not available right now!";
            Done = true;
        }

        internal void Success(string val)
        {
            Value = val;
            Done = true;
        }

        internal void Err(string val)
        {
            Error = val;
            Done = true;
        }
    }
}