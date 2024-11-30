using System;
using Game.Electricity;
using TMPro;
using UnityEngine;

namespace UI.POI
{
    public class ElectricityProdReqs: MonoBehaviour
    {
        [SerializeField]
        private Color connColor;
        [SerializeField]
        private Color notConnColor;

        [SerializeField]
        private TMP_Text descText;

        public IElectricityProducer Producer;

        private void Start()
        {
            var isConn = Producer.IsCovered;
            var cons = Producer.MaxProduction;

            var color = isConn ? connColor : notConnColor;
            var t = isConn ? "ДА" : "НЕТ";
            descText.text =
                $"Производство: {ElectricityManager.ConvertToWatts(cons)}\nПодключен к сети: <color=#{ColorUtility.ToHtmlStringRGB(color)}>{t}";
        }
    }
}