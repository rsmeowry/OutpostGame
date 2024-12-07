using System.Collections;
using UnityEngine;

namespace Game.Citizens.States
{
    public class CitizenGoWorkState: CitizenState
    {
        public CitizenGoWorkState(CitizenAgent agent, CitizenStateMachine stateMachine) : base(agent, stateMachine)
        {
            
        }

        
        public override IEnumerator EnterState()
        {
            Agent.navMeshAgent.enabled = true;
            _enterTime = Time.time;
            Agent.navMeshAgent.isStopped = false;
            Agent.navMeshAgent.SetDestination(Agent.WorkPlace.EntrancePos.GetSelfPosition(Agent));
            yield break;
        }

        public override IEnumerator ExitState()
        {
            Agent.navMeshAgent.isStopped = true;
            yield break;
        }

        private float _enterTime;
        public override void FrameUpdate()
        {
            if (Agent.WorkPlace.EntrancePos.DoesAccept(Agent) || Time.time - _enterTime > 60f)
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