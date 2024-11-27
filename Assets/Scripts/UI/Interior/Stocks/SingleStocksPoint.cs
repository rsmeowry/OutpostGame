using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Interior.Stocks
{
    public class SingleStocksPoint: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string title;
        public string desc;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show(title, desc);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }
    }
}