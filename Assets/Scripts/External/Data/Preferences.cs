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

        private void Awake()
        {
            Instance = this;
        }

        public void Load()
        {
            ReadSaved();
        }

        private void ReadSaved()
        {
            if (!FileManager.Instance.Storage.FileExists("prefs.json"))
            {
                Prefs = new SerializedPreferences()
                {
                    ShadowDistanceLevel = 5,
                    GrassBlendLevel = 5,
                    MusicVolume = 0.5f,
                    SoundVolume = 0.5f,
                    LastPlayerName = "Джон",
                    ResWidth = Screen.currentResolution.width,
                    ResHeight = Screen.currentResolution.height
                };
                return;
            }

            Prefs = JsonConvert.DeserializeObject<SerializedPreferences>(FileManager.Instance.Storage.ReadFile("prefs.json"));
        }

        public void Save()
        {
            FileManager.Instance.Storage.SaveString("prefs.json", JsonConvert.SerializeObject(Prefs));
        }
    }

    public class SerializedPreferences
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
        public int GrassBlendLevel = 5;
        [JsonProperty("musicVolume")]
        public float MusicVolume = 0.5f;
        [JsonProperty("soundVolume")]
        public float SoundVolume = 0.5f;
        [JsonProperty("lastPlayerName")]
        public string LastPlayerName = "Джон";
        [JsonProperty("resWidth")]
        public int ResWidth = 1920;
        [JsonProperty("resHeight")]
        public int ResHeight = 1080;
    }
}