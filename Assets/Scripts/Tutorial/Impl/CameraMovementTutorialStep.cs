using Game.Controllers;
using TMPro;
using UnityEngine;

namespace Tutorial.Impl
{
    public class CameraMovementTutorialStep: TutorialStep
    {
        private Vector3 _oldPos;
        [SerializeField]
        private GameObject prefab;
        private readonly float _requiredAmount = 70f * 70f;
        private float _totalMoved = 0f;
        public override void ReceiveMovementData(Vector3 pos)
        {
            if (_oldPos == Vector3.zero)
            {
                _oldPos = pos;
                return;
            }

            var diff = (pos - _oldPos).sqrMagnitude;
            _totalMoved += diff;
            _oldPos = pos;
            
            UpdateTopBar(_totalMoved / _requiredAmount);

            if (_totalMoved >= _requiredAmount)
            {
                TownCameraController.Instance.interactionFilter = CameraInteractionFilter.ProductionAndCitizens;
                MarkDone();
            }
        }

        public override void Activate()
        {
            TownCameraController.Instance.interactionFilter = CameraInteractionFilter.None;
            TutorialCtl.Instance.topBar.GetComponentInChildren<TMP_Text>().text = "Подвигайте камерой";
        }

        public override void DisplayModal(RectTransform modal)
        {
            var inst = Instantiate(prefab, modal);
            inst.transform.SetSiblingIndex(1);
        }
    }
}