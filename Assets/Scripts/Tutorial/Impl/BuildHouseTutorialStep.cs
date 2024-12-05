using Game.Building;
using Game.Citizens;
using Game.Controllers;
using Game.Production.POI;
using Game.State;
using TMPro;
using UnityEngine;

namespace Tutorial.Impl
{
    public class BuildHouseTutorialStep: TutorialStep
    {
        [SerializeField]
        private GameObject prefab;
        
        public override void Activate()
        {
            TownCameraController.Instance.interactionFilter = CameraInteractionFilter.None;
            TutorialCtl.Instance.topBar.GetComponentInChildren<TMP_Text>().text = "Постройте жилой дом";
        }

        public override void DisplayModal(RectTransform modal)
        {
            var inst = Instantiate(prefab, modal);
            inst.transform.SetSiblingIndex(1);
        }

        public override void ReceiveBuildingBuilt(BuildingData building)
        {
            if (building.keyId != "house_1") return;
            
            GameStateManager.Instance.ChangeCurrency(300, "Completed tutorial part", true);
            MarkDone();
        }
    }
}