using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Interior
{
    public class IconAppOpen: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private PCWindowDisplay window;

        [SerializeField]
        private GameObject componentPrefab;

        [SerializeField]
        private string title;
        
        [SerializeField]
        private string tipTitle;
        [SerializeField] [TextArea(2, 4)]
        private string tipDesc;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            window.ShowWithData(title, componentPrefab);
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