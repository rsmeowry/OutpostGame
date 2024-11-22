using System.Collections;
using UI.BottomRow;

namespace Game.Controllers.States
{
    public class CameraRoadBuildingState: CameraFreeMoveState
    {
        public CameraRoadBuildingState(CameraStateMachine stateMachine, TownCameraController cameraController) : base(stateMachine, cameraController)
        {
            
        }

        public override IEnumerator EnterState()
        {
            yield return base.EnterState();
            CameraController.interactionFilter = CameraInteractionFilter.Intersections;
            
            // Hide top row so we don't accidentally click
            yield return BottomRowCtl.Instance.HideTopRow();
        }

        public override IEnumerator ExitState()
        {
            yield return base.ExitState();
            CameraController.interactionFilter = CameraInteractionFilter.ProductionAndCitizens;
        }
    }
}