using System;
using System.Globalization;
using Game.Production.Products;
using Game.State;
using Game.Stocks;
using TMPro;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Interior.Stocks
{
    public class SingleStockChoice: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public StateKey Item;
        public PCWindowDisplay window;

        [SerializeField]
        private StocksGraphRenderer nextPrefab;

        private string _itemName;

        public void Start()
        {
            var icon = transform.GetChild(0).GetChild(0).GetComponent<Image>();
            var info = transform.GetChild(1);

            var title = info.GetChild(0).GetComponent<TMP_Text>();
            var buyPrice = info.GetChild(1).GetComponent<TMP_Text>();
            var sellPrice = info.GetChild(2).GetComponent<TMP_Text>();
            
            var data = ProductRegistry.Instance.GetProductData(Item);
            icon.sprite = data.icon;

            _itemName = data.name;
            title.text = data.name;

            var offers = StockManager.Instance.Markets[0].Offers;
            var sp = Mathf.RoundToInt(offers[Item].SellPrice);
            var bp = Mathf.RoundToInt(offers[Item].BuyPrice);
            buyPrice.text = $"Покупка: {bp.ToString(CultureInfo.GetCultureInfo("ru-RU"))} ЭМ";
            sellPrice.text = $"Продажа: {sp.ToString(CultureInfo.GetCultureInfo("ru-RU"))} ЭМ";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show("Просмотреть информацию", $"Нажмите чтобы изучить информацию о '{_itemName}'");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
            window.ShowLoading($"GALACTION - '{_itemName}'", nextPrefab.gameObject, obj => obj.GetComponent<StocksGraphRenderer>().Item = Item);
        }
    }
}