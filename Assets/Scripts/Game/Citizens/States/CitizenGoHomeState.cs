using System.Collections;
using External.Util;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Citizens.States
{
    public class CitizenGoHomeState: CitizenState
    {
        public CitizenGoHomeState(CitizenAgent agent, CitizenStateMachine stateMachine) : base(agent, stateMachine)
        {
        }
        
        public override IEnumerator EnterState()
        {
            Agent.navMeshAgent.enabled = true;
            Agent.navMeshAgent.isStopped = false;
            Agent.StartCoroutine(Agent.RemoveItem());
            Agent.navMeshAgent.destination = Agent.Home.EntrancePos.GetSelfPosition(Agent);
            yield return new WaitUntil(() => !Agent.navMeshAgent.pathPending);
            yield return null;
        }

        public override IEnumerator ExitState()
        {
            Agent.navMeshAgent.isStopped = true;
            yield break;
        }

        public override void FrameUpdate()
        {
            var accepts = Agent.Home.EntrancePos.DoesAccept(Agent);
            if (!accepts) return;
            
            Agent.Home.EntrancePos.Dequeue();
            Agent.StartCoroutine(StateMachine.ChangeState(Agent.SleepState));
        }

        public override void Renavigate()
        {
            Agent.navMeshAgent.SetDestination(Agent.Home.EntrancePos.GetSelfPosition(Agent));
        }
    }
}