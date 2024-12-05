using System;
using System.Collections.Generic;
using Game.Storage;
using Game.Upgrades;
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
            Load();
        }

        public void Save()
        {
            FileManager.Instance.Storage.SaveString("misc.json", JsonConvert.SerializeObject(Data), true);
        }

        public void Load()
        {
            if (!FileManager.Instance.Storage.FileExists("misc.json", true))
            {
                Data = new MiscData()
                {
                    Experience = 0,
                    Achievements = new(),
                    NotesData = "",
                    UnlockedUpgrades = new List<UnlockedUpgrade>(),
                    TutorialStep = 0
                };
                return;
            }

            Data = JsonConvert.DeserializeObject<MiscData>(FileManager.Instance.Storage.ReadFile("misc.json", true));
        }
    }

    public struct MiscData
    {
        [JsonProperty("exp")]
        public int Experience;
        [JsonProperty("achievements")]
        public List<string> Achievements;
        [JsonProperty("notes")]
        public string NotesData;
        [JsonProperty]
        public List<UnlockedUpgrade> UnlockedUpgrades;
        [JsonProperty("tutorialStep")]
        public int TutorialStep;
    }
}