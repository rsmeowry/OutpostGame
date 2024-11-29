using Game.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.POI
{
    public class PanelCloseButton: MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            TownCameraController.Instance.StartCoroutine(TownCameraController.Instance.StateMachine.SwitchState(TownCameraController.Instance.FreeMoveState));
        }
    }
}