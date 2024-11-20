using System;
using System.Linq;
using Game.Building;
using UI.Util;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Building
{
    public class SingleBuildingChoice: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private BuildingData buildingData;

        private Image _icon;

        private string _title;
        private string _description;

        private void Start()
        {
            _icon = transform.GetChild(0).GetComponent<Image>();

            _title = buildingData.name;
            _description = buildingData.description + "\n" + ResourceRequirement.ToDict(buildingData.requirements).Select(it => it.Key.Formatted() + " = " + it.Value).ToLineSeparatedString();
            _icon.sprite = buildingData.icon;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show(_title, _description, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }
    }
}