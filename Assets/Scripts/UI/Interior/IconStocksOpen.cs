using Game.Production.Products;
using UI.Interior.Stocks;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Interior
{
    public class IconStocksOpen: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private PCWindowDisplay window;

        [SerializeField]
        private StocksGraphRenderer componentPrefab;

        [SerializeField]
        private string title;
        
        [SerializeField]
        private string tipTitle;
        [SerializeField] [TextArea(2, 4)]
        private string tipDesc;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            var v = window.ShowWithData(title, componentPrefab.gameObject).GetComponent<StocksGraphRenderer>();
            v.Item = ProductRegistry.CopperIngot;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show(tipTitle, tipDesc, 0.4f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }
    }
}