using System;
using System.Collections.Generic;
using Game.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace External.Data
{
    public class MiscSavedData: MonoBehaviour
    {
        public static MiscSavedData Instance { get; private set; }
        public MiscData Data;

        private void Awake()
        {
            Instance = this;
        }

        public void Save()
        {
            FileManager.Instance.Storage.SaveString("misc.json", JsonConvert.SerializeObject(Data), true);
        }

        public void Load()
        {
            if (!FileManager.Instance.Storage.FileExists("misc.json", true))
            {
                Data = new MiscData();
                return;
            }

            Data = JsonConvert.DeserializeObject<MiscData>(FileManager.Instance.Storage.ReadFile("misc.json", true));
            
        }
    }

    public struct MiscData
    {
        [JsonProperty("achievements")]
        public List<string> Achievements;
        [JsonProperty("notes")]
        public string NotesData;
    }
}