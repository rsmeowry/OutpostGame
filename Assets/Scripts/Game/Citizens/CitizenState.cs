using System.Collections;

namespace Game.Citizens
{
    public class CitizenState
    {
        protected CitizenAgent Agent;
        protected CitizenStateMachine StateMachine;

        internal bool DoTick = true;

        public CitizenState(CitizenAgent agent, CitizenStateMachine stateMachine)
        {
            Agent = agent;
            StateMachine = stateMachine;
        }

        public virtual IEnumerator EnterState()
        {
            yield break;
        }

        public virtual IEnumerator ExitState()
        {
            yield break;
        }
        public virtual void FrameUpdate() { }
        public virtual void PhysicsUpdate() { }
        public virtual void AnimationTriggerEvent() { }
        public virtual void Tick() { }

        public virtual IEnumerator OnOtherAgentApproach()
        {
            yield break;
        }

        public virtual void Renavigate() { }
    }
}