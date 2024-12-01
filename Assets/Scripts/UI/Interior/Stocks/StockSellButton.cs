using System;
using System.Collections.Generic;
using External.Data;
using Game.Production.Products;
using Game.Sound;
using Game.State;
using Game.Stocks;
using Game.Upgrades;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Interior.Stocks
{
    public class StockSellButton: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        public StocksGraphRenderer parent;
        [SerializeField]
        private int sellAmount;
        
        private Button _self;
        private int _estimatedProfit;
        private int _itemCount;
        private string _productName;

        private bool _hasExpFromSelling;

        private Lazy<AudioClip> clip = new();

        private void Start()
        {
            UpgradeTreeManager.Instance.Has(Game.Upgrades.Upgrades.ExpFromSelling);
            _self = GetComponent<Button>();
            _productName = ProductRegistry.Instance.GetProductData(parent.Item).name;
            _itemCount = GameStateManager.Instance.PlayerProductCount.GetValueOrDefault(parent.Item.Formatted(), 0);
            RecalculatePossibilities();
        }

        private void OnEnable()
        {
            GameStateManager.Instance.onProductChanged.AddListener(HandleProductChange);
            GameStateManager.Instance.currencyIncreaseEvent.AddListener(RecalculatePossibilities);
            StockManager.Instance.onFluctuate.AddListener(RecalculatePossibilities);
        }

        private void OnDisable()
        {
            GameStateManager.Instance.onProductChanged.RemoveListener(HandleProductChange);
            GameStateManager.Instance.currencyIncreaseEvent.RemoveListener(RecalculatePossibilities);
            StockManager.Instance.onFluctuate.RemoveListener(RecalculatePossibilities);
        }

        private void HandleProductChange(ProductChangedData data)
        {
            if (data.Product.Path != parent.Item.Path)
                return;

            _itemCount += data.Delta;
            RecalculatePossibilities();
        }

        private void RecalculatePossibilities()
        {
            var market = StockManager.Instance.Markets[0];
            var receipt = market.Sell(parent.Item, sellAmount);
            _estimatedProfit = receipt.TotalProfit;
            _self.interactable = sellAmount == -1 ? _itemCount > 0 : _itemCount >= sellAmount;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var market = StockManager.Instance.Markets[0];
            var receipt = market.Sell(parent.Item, sellAmount == -1 ? _itemCount : sellAmount);
            // TODO: fancy receipt pop up
            if (_itemCount < sellAmount || (sellAmount == -1 && _itemCount <= 0)) return;
            var cnt = sellAmount == -1 ? _itemCount : sellAmount;

            if (_hasExpFromSelling && cnt > 50)
                MiscSavedData.Instance.Data.Experience += 100;
            market.DoSell(receipt);
            
            SoundManager.Instance.PlaySound2D(clip.Value, 0.5f);
            
            GameStateManager.Instance.ChangeCurrency(receipt.TotalProfit,
                $"Sold {receipt.SoldCount} items of type {receipt.Item.Formatted()}",
                receipt.SoldCount >= 50);
            TooltipCtl.Instance.Show($"Продать {_productName}: {cnt}", $"Вы получите {_estimatedProfit} ЭМ" + (_hasExpFromSelling && cnt > 50 ? " (+100 XP)" : "")  + (_itemCount < sellAmount || (sellAmount == -1 && _itemCount <= 0) ? " (НЕДОСТАТОЧНО ПРЕДМЕТОВ)" : ""), 0.2f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var cnt = sellAmount == -1 ? _itemCount : sellAmount;
            TooltipCtl.Instance.Show($"Продать {_productName}: {cnt}", $"Вы получите {_estimatedProfit} ЭМ" + (_hasExpFromSelling && cnt > 50 ? " (+100 XP)" : "") + (_itemCount < sellAmount || (sellAmount == -1 && _itemCount <= 0) ? " (НЕДОСТАТОЧНО ПРЕДМЕТОВ)" : ""), 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }
    }
}