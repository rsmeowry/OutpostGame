using System;
using System.Collections.Generic;
using System.Linq;
using External.Util;
using Game.Building;
using Game.Controllers;
using Game.Production.Products;
using Game.State;
using Game.Upgrades;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Building.Tabs
{
    public class SingleBuildingChoice: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public BuildingData buildingData;

        [SerializeField] private Sprite unknownIcon;

        [SerializeField]
        private GameObject impossibleIcon;
        
        private Image _icon;

        private string _title;
        private string _description;

        public void Init()
        {
            _icon = transform.GetChild(0).GetComponent<Image>();

            if (!HasUpgrade() && buildingData.requiredUpgrade != "")
            {
                var k = StateKey.FromString(buildingData.requiredUpgrade);
                var requiredUpgrade = UpgradeTreeManager.Instance.upgradeDatabase.First(it => StateKey.FromString(it.id) == k);
                _icon.sprite = unknownIcon;
                _title = "???";
                _description = $"Это строение откроется вам когда вы изучите '{requiredUpgrade.title}'";
            }
            else
            {
                _title = buildingData.name;
                _description = buildingData.description + "\nТребуются ресурсы:\n" + ResourceRequirement
                    .ToDict(buildingData.requirements).Select(it =>
                    {
                        var data = ProductRegistry.Instance.GetProductData(it.Key);
                        return "<b>" + data.name + ": " + it.Value + " шт.</b>";
                    }).ToLineSeparatedString();
                _icon.sprite = buildingData.icon;
            }
            
            CheckReqs(new ProductChangedData());
        }

        private void OnEnable()
        {
            GameStateManager.Instance.onProductChanged.AddListener(CheckReqs);
        }

        private void OnDisable()
        {
            GameStateManager.Instance.onProductChanged.RemoveListener(CheckReqs);
        }

        private void CheckReqs(ProductChangedData _)
        {
            var hasUpgrade = HasUpgrade() || buildingData.requiredUpgrade == "";
            impossibleIcon.SetActive(!HasResources() && hasUpgrade);
        }

        private bool HasUpgrade()
        {
            return buildingData.requiredUpgrade != "" &&
                   UpgradeTreeManager.Instance.Has(StateKey.FromString(buildingData.requiredUpgrade));
        }

        private bool HasResources()
        {
            foreach (var req in buildingData.requirements)
            {
                var k = StateKey.FromString(req.key);
                if (GameStateManager.Instance.PlayerProductCount.GetValueOrDefault(k.Formatted(), 0) < req.count)
                    return false;
            }

            return true;
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

            if (!HasUpgrade() && buildingData.requiredUpgrade != "")
                return;
            
            // check resource requirements
            if (!HasResources())
            {
                ToastManager.Instance.ShowToast("Недостаточно ресурсов для постройки!");
                return;
            }
            
            BuildingManager.Instance.BeginBuilding(buildingData);
            StartCoroutine(TownCameraController.Instance.StateMachine.SwitchState(TownCameraController.Instance.BuildingState));
        }
    }
}