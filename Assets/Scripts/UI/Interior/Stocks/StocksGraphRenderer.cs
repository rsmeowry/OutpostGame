using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Production.Products;
using Game.State;
using Game.Stocks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Interior.Stocks
{
    public class StocksGraphRenderer: MonoBehaviour
    {
        [SerializeField]
        private UILineRenderer rendererBuy;

        [SerializeField]
        private UILineRenderer rendererSell;

        [SerializeField]
        private TMP_Text textMaxBuy;

        [SerializeField]
        private TMP_Text textMaxSell;

        [SerializeField]
        private Image itemIcon;

        [SerializeField]
        private TMP_Text countText;

        [SerializeField]
        private TMP_Text titleText;

        private int _productCount;
        
        public StateKey Item;

        private void MarketChangeHandler()
        {
            RebuildSelf();
        }

        private void ProductIncreaseHandler(ProductChangedData productData)
        {
            if (productData.Product.Path != Item.Path)
                return;

            _productCount += productData.Delta;
            countText.text = "| У вас: <b>" + _productCount.ToString(CultureInfo.GetCultureInfo("ru-RU"));
        }

        private void Start()
        {
            var itemData = ProductRegistry.Instance.GetProductData(Item);
            _productCount = GameStateManager.Instance.PlayerProductCount.GetValueOrDefault(Item.Formatted(), 0);
            itemIcon.sprite = itemData.icon;
            titleText.text = itemData.name;
            countText.text = "| У вас: <b>" + _productCount.ToString(CultureInfo.GetCultureInfo("ru-RU"));
            RebuildSelf();
        }

        private void RebuildSelf()
        {
            var frozen = StockManager.Instance.Markets[0].FrozenOffers;
            var data = frozen.Select(it => it[Item]).ToList();
            data.Reverse();
            var buyData = data.TakeLast(10).Select(it => it.BuyPrice).ToList();
            var sellData = data.TakeLast(10).Select(it => it.SellPrice).ToList();
            Debug.Log("REBUILDING LINES");
            RebuildLines(rendererBuy, buyData, "Покупка");
            RebuildLines(rendererSell, sellData, "Продажа");

            textMaxBuy.text = Mathf.RoundToInt(buyData.Max()).ToString(CultureInfo.InvariantCulture);
            textMaxSell.text = Mathf.RoundToInt(sellData.Max()).ToString(CultureInfo.InvariantCulture);

        }

        private void RebuildLines(UILineRenderer rd, List<float> amounts, string prefix)
        {
            var perPointRatio = 130 / amounts.Max();
            var previous = 0f;
            var mapped = amounts.Select((each, idx) =>
            {
                if (idx == 0)
                    previous = each;
                var posY = each * perPointRatio;
                int posX;
                string descTitle;
                string desc;
                descTitle = $"{prefix}: {Mathf.RoundToInt(each)} ЭМ";
                if (idx == 0)
                {
                    posX = 10;
                    desc = "Начало отсчета";
                }
                else
                {
                    posX = idx * 50;
                    var diff = Mathf.RoundToInt(each) - Mathf.RoundToInt(previous);
                    desc = diff == 0 ? "Без изменений" : $"Изменение на {diff}";
                    previous = each;
                }

                return (new Vector2(posX, posY), descTitle, desc);
            }).ToArray();
            var arr = new Vector2[mapped.Length];
            var points = new (string, string)[mapped.Length];
            var i = 0;

            foreach (var each in mapped)
            {
                arr[i] = each.Item1;
                points[i] = (each.descTitle, each.desc);
                i++;
            }

            rd.points = arr;
            rd.circlePoints = points;
            
            rd.SetAllDirty();
            rd.RepositionCircles();
        }

        public void OnEnable()
        {
            StockManager.Instance.onFluctuate.AddListener(MarketChangeHandler);
            GameStateManager.Instance.onProductChanged.AddListener(ProductIncreaseHandler);
        }

        private void OnDisable()
        {
            StockManager.Instance.onFluctuate.RemoveListener(MarketChangeHandler);
            GameStateManager.Instance.onProductChanged.RemoveListener(ProductIncreaseHandler);
        }
    }
}