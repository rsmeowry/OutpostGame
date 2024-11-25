namespace Game.RoadBuilding.States
{
    public abstract class RoadBuildingState
    {
        protected RoadManager Manager;
        protected RoadBuildingStateMachine StateMachine;

        protected RoadBuildingState(RoadManager manager, RoadBuildingStateMachine stateMachine)
        {
            Manager = manager;
            StateMachine = stateMachine;
        }

        public virtual void EnterState()
        {
            
        }

        public virtual void ExitState()
        {
            
        }

        public virtual void FrameUpdate()
        {
            
        }

        public virtual void HandleClickOther(RoadIntersection other)
        {
            
        }
    }
}