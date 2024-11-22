using System.Collections;
using External.Util;
using Game.Building;
using UI.BottomRow;
using UnityEngine;

namespace Game.Controllers.States
{
    public class CameraBuildingState: CameraFreeMoveState
    {
        public CameraBuildingState(CameraStateMachine stateMachine, TownCameraController cameraController) : base(stateMachine, cameraController)
        {
            
        }

        public override IEnumerator EnterState()
        {
            yield return base.EnterState();
            CameraController.interactionFilter = CameraInteractionFilter.None;

            // Hide top row so we don't accidentally click
            BottomRowCtl.Instance.StartCoroutine(BottomRowCtl.Instance.CloseTab(true));
        }

        public override IEnumerator ExitState()
        {
            yield return base.ExitState();
            CameraController.interactionFilter = CameraInteractionFilter.ProductionAndCitizens;
        }

        public override void LateFrameUpdate()
        {
            base.LateFrameUpdate();
            
            HandleMovement();
            HandleKeyboard();
            HandleClicks();
        }

        private void HandleMovement()
        {
            var ray = CameraController.Camera.ScreenPointToRay(Input.mousePosition);
            var hit = new RaycastHit[1];
            Physics.RaycastNonAlloc(ray, hit, 100000f, BuildingManager.Instance.terrainLayer);
            BuildingManager.Instance.currentBuilding.position = BuildingManager.Instance.LerpedSnap(BuildingManager.Instance.currentBuilding.position, hit[0].point);
        }

        private void HandleClicks()
        {
            if (Input.GetMouseButtonDown(0))
            {
                BuildingManager.Instance.StartCoroutine(BuildingManager.Instance.PlaceBuilding());
                CameraController.StartCoroutine(StateMachine.SwitchState(CameraController.FreeMoveState));
                // TODO: more complex placement
            }
        }

        private void HandleKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                BuildingManager.Instance.Rotate(Input.GetKey(KeyCode.LeftShift));
            }
        }
    }
}