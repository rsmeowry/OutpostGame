using System;
using Game.Electricity;
using TMPro;
using UnityEngine;

namespace UI.POI
{
    public class POIElectricityStatistics: MonoBehaviour
    {
        [SerializeField]
        private TMP_Text statsText;

        [NonSerialized]
        public Game.Electricity.ElectricityStatistics Stats;

        public void Start()
        {
            statsText.text = $"Производство: {ElectricityManager.ConvertToWatts(Stats.MaxProduction)}\nПотребление: {ElectricityManager.ConvertToWatts(Stats.MaxConsumption)}";
        }
        
    }
}