using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Interior
{
    public class IconCloseWindow: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private PCWindowDisplay window;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            window.Hide();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show("Закрыть окно", "", 0.3f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }
    }
}