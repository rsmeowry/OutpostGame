using System.Linq;
using External.Util;
using Game.Building;
using Game.Controllers;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Building.Tabs
{
    public class SingleBuildingChoice: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public BuildingData buildingData;

        private Image _icon;

        private string _title;
        private string _description;

        public void Init()
        {
            _icon = transform.GetChild(0).GetComponent<Image>();

            _title = buildingData.name;
            _description = buildingData.description + "\n" + ResourceRequirement.ToDict(buildingData.requirements).Select(it => it.Key.Formatted() + " = " + it.Value).ToLineSeparatedString();
            _icon.sprite = buildingData.icon;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // TODO: maybe highlight all buildings of this type?
            TooltipCtl.Instance.Show(_title, _description, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (BuildingManager.Instance.isBuilding)
                return;
            BuildingManager.Instance.BeginBuilding(buildingData);
            StartCoroutine(TownCameraController.Instance.StateMachine.SwitchState(TownCameraController.Instance.BuildingState));
        }
    }
}