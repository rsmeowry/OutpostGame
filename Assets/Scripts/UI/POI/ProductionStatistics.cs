using System;
using TMPro;
using UI.Util;
using UnityEngine;
using UnityEngine.UI;

namespace UI.POI
{
    public class ProductionStatistics: MonoBehaviour
    {
        public string title;
        public string helpTitle;
        public string helpDescription;
        public Sprite itemIcon;
        public string productionLevel;
        public string itemName;

        public void Start()
        {
            var ttl = transform.GetChild(0);
            var tooltip = ttl.GetComponent<SimpleTooltipDisplay>();
            tooltip.title = helpTitle;
            tooltip.body = helpDescription;
            ttl.GetComponentInChildren<TMP_Text>().text = title;
            var container = transform.GetChild(1);
            var itemBg = container.GetChild(0);
            itemBg.GetComponent<SimpleTooltipDisplay>().title = itemName;
            itemBg.GetChild(0).GetComponent<Image>().sprite = itemIcon;
            container.GetChild(1).GetComponentInChildren<TMP_Text>().text = productionLevel;
        }
    }
}