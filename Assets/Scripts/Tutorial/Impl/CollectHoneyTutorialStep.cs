using Game.Citizens;
using Game.Controllers;
using Game.Production.POI;
using Game.Production.Products;
using Game.State;
using TMPro;
using UnityEngine;

namespace Tutorial.Impl
{
    public class CollectHoneyTutorialStep: TutorialStep
    {
        [SerializeField]
        private GameObject prefab;

        private float _count;
        private float _needed = 10;
        
        public override void Activate()
        {
            TownCameraController.Instance.interactionFilter = CameraInteractionFilter.None;
            TutorialCtl.Instance.topBar.GetComponentInChildren<TMP_Text>().text = "Соберите 10 меда";
        }

        public override void DisplayModal(RectTransform modal)
        {
            var inst = Instantiate(prefab, modal);
            inst.transform.SetSiblingIndex(1);
        }

        public override void ReceiveResourceData(StateKey item, int count)
        {
            if (item != ProductRegistry.Honey) return;
            
            _count += count;
            TutorialCtl.Instance.topBarImage.fillAmount = _count / _needed;

            if (_count >= _needed)
            {
                GameStateManager.Instance.IncreaseProduct(ProductRegistry.Wood, 30);
                GameStateManager.Instance.IncreaseProduct(ProductRegistry.Stone, 30);
                MarkDone();
            }
        }
    }
}