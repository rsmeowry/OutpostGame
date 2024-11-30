using System;
using System.Collections.Generic;
using System.Linq;
using External.Data;
using External.Util;
using Game.State;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Upgrades
{
    public class UpgradeTreeManager: MonoBehaviour
    {
        public static UpgradeTreeManager Instance { get; private set; }
        
        public List<UpgradeData> upgradeDatabase;
        public Dictionary<StateKey, int> Upgrades = new();

        private Dictionary<StateKey, UpgradeData> _mappedUpgradeDatabase = new();

        public UnityEvent<UpgradeUnlockedData> onUpgradeUnlocked = new();

        private void Awake()
        {
            Instance = this;

            _mappedUpgradeDatabase = upgradeDatabase.ToDictionary(it => StateKey.FromString(it.id), it => it);
        }

        public bool ShouldBeShown(UpgradeData data)
        {
            var k = StateKey.FromString(data.id);
            return k.Path == "harder_tasks" || Upgrades.ContainsKey(k) || Upgrades.Any(it =>
            {
                var unlocked = it.Key;
                var upg = _mappedUpgradeDatabase[unlocked];
                return upg.unlocks.Contains(data);
            });
        }

        public void UnlockUpgrade(UpgradeData data)
        {
            var k = StateKey.FromString(data.id);
            if (Upgrades.ContainsKey(k) && !data.isInfinite)
                return;
            MiscSavedData.Instance.Data.Experience -=
                data.baseExperienceCost * (int)Mathf.Pow(2, Upgrades.GetValueOrDefault(k));
            Upgrades.Increment(k);
            
            
            onUpgradeUnlocked.Invoke(new UpgradeUnlockedData { Upgrade = data });
        }

        public bool Has(StateKey key)
        {
            return Upgrades.ContainsKey(key);
        }
        
        public void LoadData()
        {
            foreach (var dataUnlockedUpgrade in MiscSavedData.Instance.Data.UnlockedUpgrades)
            {
                Upgrades[StateKey.FromString(dataUnlockedUpgrade.UpgradeId)] = dataUnlockedUpgrade.Tier;
            }
        }

        public void Save()
        {
            var mapped = Upgrades.Select(it => new UnlockedUpgrade { UpgradeId = it.Key.Formatted(), Tier = it.Value }).ToList();
            MiscSavedData.Instance.Data.UnlockedUpgrades = mapped;
        }
    }
    
    public struct UpgradeUnlockedData
    {
        public UpgradeData Upgrade;
    }

    public struct UnlockedUpgrade
    {
        [JsonProperty("upgradeId")]
        public string UpgradeId;
        [JsonProperty("tier")]
        public int Tier;
    }
}