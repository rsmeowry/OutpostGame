using UnityEngine;

namespace Game.RoadBuilding.States
{
    public class RoadBuildingInactiveState: RoadBuildingState
    {
        public RoadBuildingInactiveState(RoadManager manager, RoadBuildingStateMachine stateMachine) : base(manager, stateMachine)
        {
        }

        public override void EnterState()
        {
            Manager.HideAll();
            Manager.ActiveIntersection = null;
            Object.Destroy(Manager.JointObject);
            Manager.JointObject = null;
        }

        // noop
    }
}