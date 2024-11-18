using System.Collections;

namespace Game.Citizens.States
{
    public class CitizenGoWorkState: CitizenState
    {
        public CitizenGoWorkState(CitizenAgent agent, CitizenStateMachine stateMachine) : base(agent, stateMachine)
        {
            
        }

        public override IEnumerator EnterState()
        {
            Agent.navMeshAgent.isStopped = false;
            Agent.navMeshAgent.SetDestination(Agent.OrderTarget.EntrancePos.GetSelfPosition(Agent));
            yield break;
        }

        public override IEnumerator ExitState()
        {
            Agent.navMeshAgent.isStopped = true;
            yield break;
        }

        public override void FrameUpdate()
        {
            if (Agent.OrderTarget.EntrancePos.DoesAccept(Agent))
            {
                Agent.OrderTarget.EntrancePos.Dequeue();
                Agent.StartCoroutine(StateMachine.ChangeState(Agent.WorkState));
            }
        }

        public override void Renavigate()
        {
            Agent.navMeshAgent.SetDestination(Agent.OrderTarget.EntrancePos.GetSelfPosition(Agent));
        }
    }
}