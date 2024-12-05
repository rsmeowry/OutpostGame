using System;
using System.Collections;
using External.Data;
using Game.Citizens;
using Game.DayNight;
using Game.Network;
using Game.Player;
using Game.POI;
using Game.Production.Products;
using Game.State;
using Game.Stocks;
using Game.Upgrades;
using UnityEngine;

namespace Game.Storage
{
    public class SaveLoader: MonoBehaviour
    {
        public static SaveLoader Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private IEnumerator Start()
        {
            UpgradeTreeManager.Instance.LoadData();
            DayCycleManager.Instance.Load();
            yield return null; // wait for a single frame
            POIManager.Instance.LoadData();
            yield return GameStateManager.Instance.LoadGameState();
        }

        [ContextMenu("Test/Save data")]
        private void __TestSaveData()
        {
            StartCoroutine(SaveData());
        }
        
        public IEnumerator SaveData()
        {
            UpgradeTreeManager.Instance.Save();
            MiscSavedData.Instance.Save();
            DayCycleManager.Instance.Save();
            POIManager.Instance.SaveData();
            yield return StockManager.Instance.SaveAll();
            yield return GameStateManager.Instance.SavePlayerData();
        }
    }
}