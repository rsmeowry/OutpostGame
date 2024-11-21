using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Time
{
    public class TimeBoostButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private int boost;
        [SerializeField]
        private TimeSection parent;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show("", boost switch
            {
                0 => "Поставить игру на паузу",
                1 => "Стандартное время",
                _ => $"Ускорить время в\nигре в {boost}X раз"
            }, 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            parent.ChangeSpeed(boost);
        }
    }
}