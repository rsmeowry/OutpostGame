using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Util
{
    public class UITooltipDisplay: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private string title;
        [SerializeField]
        [TextArea(4, 5)]
        private string body;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show(title, body);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }
    }
}