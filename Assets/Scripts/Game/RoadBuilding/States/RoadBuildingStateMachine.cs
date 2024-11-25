namespace Game.RoadBuilding.States
{
    public class RoadBuildingStateMachine
    {
        internal RoadManager Manager;
        internal RoadBuildingState CurrentState;
        
        public RoadBuildingStateMachine(RoadManager manager)
        {
            Manager = manager;
        }

        public void Init(RoadBuildingState initialState)
        {
            CurrentState = initialState;
            initialState.EnterState();
        }

        public void ChangeState(RoadBuildingState newState)
        {
            CurrentState.ExitState();
            CurrentState = newState;
            newState.EnterState();
        }

        public void FrameUpdate()
        {
            CurrentState.FrameUpdate();
        }

        public void HandleClickOther(RoadIntersection other)
        {
            CurrentState.HandleClickOther(other);
        }
    }
}