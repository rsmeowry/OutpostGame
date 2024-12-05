using System;
using Game.Storage;
using Newtonsoft.Json;
using UnityEngine;

namespace External.Data
{
    public class Preferences: MonoBehaviour
    {
        public static Preferences Instance { get; private set; }
        public SerializedPreferences Prefs { get; private set; }

        public void Awake()
        {
            Instance = this;
            
            ReadSaved();
        }

        private void ReadSaved()
        {
            if (!FileManager.Instance.Storage.FileExists("prefs.json"))
            {
                Prefs = new SerializedPreferences()
                {
                    ShadowDistanceLevel = 5,
                };
                return;
            }

            Prefs = JsonConvert.DeserializeObject<SerializedPreferences>(FileManager.Instance.Storage.ReadFile("prefs.json"));
        }
    }

    public struct SerializedPreferences
    {
        [JsonProperty("noSkybox")]
        public bool NoSkybox;
        [JsonProperty("noClouds")]
        public bool NoClouds;
        [JsonProperty("lightBufferLevel")]
        public int LightBufferizationLevel;
        [JsonProperty("shadowDistanceLevel")]
        public int ShadowDistanceLevel;
        [JsonProperty("disableStylizedShading")]
        public bool DisableToonShading;
        [JsonProperty("grassBlendLevel")]
        public int GrassBlendLevel;
    }
}