namespace Game.RoadBuilding.States
{
    public class RoadBuildingActiveState: RoadBuildingState
    {
        public RoadBuildingActiveState(RoadManager manager, RoadBuildingStateMachine stateMachine) : base(manager, stateMachine)
        {
        }

        public override void EnterState()
        {
            RoadManager.Instance.ShowAll();
        }

        public override void HandleClickOther(RoadIntersection other)
        {
            Manager.ActiveIntersection = other;
            Manager.CreateJoint();
            StateMachine.ChangeState(Manager.JoiningState);
        }
    }
}