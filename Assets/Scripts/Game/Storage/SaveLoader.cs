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
            MiscSavedData.Instance.Load();
            DayCycleManager.Instance.Load();
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
            MiscSavedData.Instance.Save();
            DayCycleManager.Instance.Save();
            POIManager.Instance.SaveData();
            yield return StockManager.Instance.SaveAll();
            yield return GameStateManager.Instance.SavePlayerData();
        }
    }
}