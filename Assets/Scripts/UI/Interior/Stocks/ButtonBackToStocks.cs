using Game.Sound;
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
            SoundManager.Instance.PlaySound2D(SoundBank.Instance.GetSound("ui.mouse_click"), 0.8f);
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