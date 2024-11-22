using Game.Building;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Building.Tabs
{
    public class BuildingTabSelector: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        public BuildingDatabase storedBuildings;

        [SerializeField]
        public BuildingSection parent;

        public string title;
        [TextArea(2, 4)]
        public string description;


        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show(title, description, 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (parent.selectedTab == this)
                return;
            StartCoroutine(parent.SwitchTab(this));
        }
    }
}