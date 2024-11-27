using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Interior.Stocks
{
    public class ButtonBackToStocks: MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField]
        private GameObject stockPrefab;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
            transform.GetComponentInParent<PCWindowDisplay>().ShowLoading("GALACTION - Рынок", stockPrefab, _ => { });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show("Назад", "К рынку GALACTION");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }
    }
}