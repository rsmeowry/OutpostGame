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
            Agent.navMeshAgent.SetDestination(Agent.WorkPlace.EntrancePos.GetSelfPosition(Agent));
            yield break;
        }

        public override IEnumerator ExitState()
        {
            Agent.navMeshAgent.isStopped = true;
            yield break;
        }

        public override void FrameUpdate()
        {
            if (Agent.WorkPlace.EntrancePos.DoesAccept(Agent))
            {
                Agent.WorkPlace.EntrancePos.Dequeue();
                Agent.StartCoroutine(StateMachine.ChangeState(Agent.WorkState));
            }
        }

        public override void Renavigate()
        {
            Agent.navMeshAgent.SetDestination(Agent.WorkPlace.EntrancePos.GetSelfPosition(Agent));
        }
    }
}