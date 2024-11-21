using System;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.BottomRow
{
    public class BottomRowTabSelector: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        public GameObject bottomTabPrefab;

        [SerializeField]
        private BottomRowCtl parent;

        [SerializeField]
        private string tooltipTitle;
        [SerializeField]
        [TextArea(2, 4)]
        private string tooltipDesc;
        
        public RectTransform rectTransform;

        private void Start()
        {
            rectTransform = (RectTransform) transform;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (parent.activeTab == this)
            {
                StartCoroutine(parent.CloseTab());
            } else if (parent.activeTab == null)
            {
                StartCoroutine(parent.DisplayTab(this));
            }
            else
            {
                StartCoroutine(parent.SwitchTab(this));
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show(tooltipTitle, tooltipDesc, 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }
    }
}