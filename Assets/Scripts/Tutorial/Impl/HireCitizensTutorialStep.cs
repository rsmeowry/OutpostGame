using Game.Citizens;
using Game.Controllers;
using Game.Production.POI;
using TMPro;
using UnityEngine;

namespace Tutorial.Impl
{
    public class HireCitizensTutorialStep: TutorialStep
    {
        [SerializeField]
        private GameObject prefab;
        
        public override void Activate()
        {
            TownCameraController.Instance.interactionFilter = CameraInteractionFilter.None;
            TutorialCtl.Instance.topBar.GetComponentInChildren<TMP_Text>().text = "Отправьте пасечника собирать мед";
        }

        public override void DisplayModal(RectTransform modal)
        {
            var inst = Instantiate(prefab, modal);
            inst.transform.SetSiblingIndex(1);
        }

        public override void ReceiveCitizenHired(CitizenAgent agent, ResourceContainingPOI poi)
        {
            if (poi.TryGetComponent<HiveAreaPOI>(out _))
            {
                MarkDone();
            }
        }
    }
}