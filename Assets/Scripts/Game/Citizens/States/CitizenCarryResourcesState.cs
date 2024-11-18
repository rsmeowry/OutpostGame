using System.Collections;
using External.Util;
using Game.Production;
using UnityEngine;

namespace Game.Citizens.States
{
    public class CitizenCarryResourcesState: CitizenState
    {
        public CitizenCarryResourcesState(CitizenAgent agent, CitizenStateMachine stateMachine) : base(agent, stateMachine)
        {
            
        }

        public override IEnumerator EnterState()
        {
            Agent.navMeshAgent.SetDestination(Agent.OrderTarget.GatheringPost.EntrancePos.GetSelfPosition(Agent));
            yield break;
        }

        public override IEnumerator ExitState()
        {
            yield break;
        }

        public override void FrameUpdate()
        {
            if (Agent.OrderTarget.GatheringPost.EntrancePos.DoesAccept(Agent))
            {
                DoTick = false;
                // TODO: maybe separate state for depositing resources
                Agent.Delayed(0.3f + Random.Range(-0.1f, 0.1f), () =>
                {
                    Agent.ProductDepositer.DepositInventory(Agent.Inventory);
                    Agent.OrderTarget.GatheringPost.EntrancePos.Dequeue();
                    DoTick = true;
                    Agent.StartCoroutine(StateMachine.ChangeState(Agent.GoWorkState));
                });
            }
        }

        public override void Renavigate()
        {
            var newPos = Agent.OrderTarget.GatheringPost.EntrancePos.GetSelfPosition(Agent);
            Agent.navMeshAgent.SetDestination(newPos);
        }
    }
}