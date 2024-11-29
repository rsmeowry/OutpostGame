using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Util
{
    public class SimpleTooltipDisplay: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        public string title;
        [SerializeField]
        [TextArea(4, 5)]
        public string body;
        
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