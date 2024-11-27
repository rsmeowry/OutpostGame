using System;
using Game.Production.Products;
using Game.State;
using Game.Stocks;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Interior.Stocks
{
    public class StockBuyButton: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        public StocksGraphRenderer parent;
        [SerializeField]
        private int buyAmount;
        
        private Button _self;
        private int _estimatedCost;
        private string _productName;

        private void Start()
        {
            _self = GetComponent<Button>();
            _productName = ProductRegistry.Instance.GetProductData(parent.Item).name;
            RecalculatePossibilities();
        }

        private void OnEnable()
        {
            GameStateManager.Instance.currencyIncreaseEvent.AddListener(RecalculatePossibilities);
            StockManager.Instance.onFluctuate.AddListener(RecalculatePossibilities);
        }

        private void OnDisable()
        {
            GameStateManager.Instance.currencyIncreaseEvent.RemoveListener(RecalculatePossibilities);
            StockManager.Instance.onFluctuate.RemoveListener(RecalculatePossibilities);
        }

        private void RecalculatePossibilities()
        {
            var market = StockManager.Instance.Markets[0];
            var receipt = market.Buy(parent.Item, buyAmount);
            _estimatedCost = receipt.TotalCost;
            _self.interactable = receipt.TotalCost <= GameStateManager.Instance.Currency;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var market = StockManager.Instance.Markets[0];
            var receipt = market.Buy(parent.Item, buyAmount);
            // TODO: fancy receipt pop up
            if (receipt.TotalCost > GameStateManager.Instance.Currency) return;
            
            market.DoBuy(receipt);
            GameStateManager.Instance.ChangeCurrency(-receipt.TotalCost,
                $"Bought {receipt.BoughtCount} items of type {receipt.Item.Formatted()}",
                receipt.BoughtCount >= 50);
            var add = (_estimatedCost > GameStateManager.Instance.Currency ? " (НЕДОСТАТОЧНО)" : "", 0.2f);
            TooltipCtl.Instance.Show($"Купить {_productName}: {buyAmount}", $"Обойдется вам в {_estimatedCost} ЭМ" + add);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var add = (_estimatedCost > GameStateManager.Instance.Currency ? " (НЕДОСТАТОЧНО)" : "", 0.2f);
            TooltipCtl.Instance.Show($"Купить {_productName}: {buyAmount}", $"Обойдется вам в {_estimatedCost} ЭМ" + add);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }
    }
}