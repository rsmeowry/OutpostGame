using System.Collections;
using DG.Tweening;
using External.Util;
using Game.Controllers;
using Game.State;
using Game.Tasks;
using Inside;
using Tutorial;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.BottomRow
{
    public class GoHomeButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private Image black;

        [SerializeField]
        private GameObject cameraInside;

        [SerializeField]
        private GameObject controlRoom;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show("В контрольную комнату", "Перемещает вас обратно в контрольную комнату", 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }

        private bool _transition;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (TownCameraController.Instance.interactionFilter == CameraInteractionFilter.None || _transition || TownCameraController.Instance.StateMachine.CurrentState != TownCameraController.Instance.FreeMoveState)
                return;
            
            GameStateManager.Instance.StartCoroutine(DoClick());
        }

        private IEnumerator DoClick()
        {
            _transition = true;
            yield return black.DOFade(1f, 0.3f).Play().WaitForCompletion();
            yield return ViewSwitchCtl.Instance.SwitchToLocal();
            TownCameraController.Instance.interactionFilter = CameraInteractionFilter.ProductionAndCitizens;
            TownCameraController.Instance.gameObject.SetActive(false);
            controlRoom.SetActive(true);
            cameraInside.SetActive(true);
            yield return new WaitForEndOfFrame();
            TooltipCtl.Instance.Hide();
            yield return black.DOFade(0f, 0.3f).Play().WaitForCompletion();
            _transition = false;
        }
    }
}