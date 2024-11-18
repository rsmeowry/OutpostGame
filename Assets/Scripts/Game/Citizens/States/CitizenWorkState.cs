using System.Collections;
using Game.Production;
using UnityEngine;

namespace Game.Citizens.States
{
    public class CitizenWorkState: CitizenState
    {
        private float _ticker = 1f;
        public CitizenWorkState(CitizenAgent agent, CitizenStateMachine stateMachine) : base(agent, stateMachine)
        {
            
        }

        public override IEnumerator EnterState()
        {
            Agent.navMeshAgent.isStopped = true;
            Agent.navMeshAgent.enabled = false;
            yield return new WaitForSeconds(0.2f);
            if (!Agent.WorkPlace.Accept(Agent))
            {
                Agent.navMeshAgent.enabled = true;
                Agent.Order(Agent.WanderState);
            }
            yield break;
        }

        public override IEnumerator ExitState()
        {
            Agent.navMeshAgent.enabled = true;
            Agent.navMeshAgent.isStopped = false;
            if(Agent.WorkPlace != null)
                Agent.WorkPlace.Release(Agent);
            
            // Agent.navMeshAgent.SetDestination(Agent.OrderTarget.EntrancePos.GetSelfPosition(Agent));
            // while (Agent.navMeshAgent.remainingDistance > Agent.navMeshAgent.stoppingDistance)
            // {
                // yield return null;
            // }
            yield break;
        }

        public override void FrameUpdate()
        {
            _ticker -= Time.deltaTime;
            if (_ticker <= 0f)
            {
                _ticker = 1f;
                Agent.WorkPlace.WorkerTick(Agent);
                if (Agent.InventoryFull())
                {
                    Agent.StartCoroutine(StateMachine.ChangeState(Agent.CarryResourcesState));
                    Agent.ProductDepositer = (IProductDepositer)Agent.WorkPlace.GatheringPost;
                }
            }
        }
    }
}