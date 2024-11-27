using System;
using Game.Stocks;
using UnityEngine;

namespace UI.Interior.Stocks
{
    public class StockComponentHandler: MonoBehaviour
    {
        [SerializeField]
        private RectTransform stockContainer;

        [SerializeField]
        private SingleStockChoice prefab;

        private void Start()
        {
            var window = transform.parent.parent.GetComponent<PCWindowDisplay>();
            foreach (var item in StockManager.Instance.Markets[0].Offers.Keys)
            {
                var created = Instantiate(prefab, stockContainer);
                created.window = window;
                created.Item = item;
            }

            var anchPos = stockContainer.anchoredPosition;
            anchPos.y -= stockContainer.rect.height;
            stockContainer.anchoredPosition = anchPos;
        }
    }
}