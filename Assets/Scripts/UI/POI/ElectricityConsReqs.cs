using System;
using Game.Electricity;
using TMPro;
using UnityEngine;

namespace UI.POI
{
    public class ElectricityConsReqs: MonoBehaviour
    {
        [SerializeField]
        private Color connColor;
        [SerializeField]
        private Color notConnColor;

        [SerializeField]
        private TMP_Text descText;

        public IElectricityConsumer Consumer;

        private void Start()
        {
            var isConn = Consumer.IsCovered;
            var cons = Consumer.MaxConsumption;

            var color = isConn ? connColor : notConnColor;
            var t = isConn ? "ДА" : "НЕТ";
            descText.text =
                $"Потребление: {ElectricityManager.ConvertToWatts(cons)}\nПодключен к сети: <color=#{ColorUtility.ToHtmlStringRGB(color)}>{t}";
        }
    }
}