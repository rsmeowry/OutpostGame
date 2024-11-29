using System.Collections;
using External.Util;
using Game.Production;
using UnityEngine;

namespace Game.Citizens.States
{
    public class CitizenCarryResourcesState: CitizenState
    {
        private static readonly int DoPlaceBox = Animator.StringToHash("DoPlaceBox");
        private static readonly int Carrying = Animator.StringToHash("Carrying");
        
        public CitizenCarryResourcesState(CitizenAgent agent, CitizenStateMachine stateMachine) : base(agent, stateMachine)
        {
            
        }

        public override IEnumerator EnterState()
        {
            if (Agent.ProductDepositer == null)
                Agent.ProductDepositer = (IProductDepositer) Agent.WorkPlace.GatheringPost;
            Agent.navMeshAgent.SetDestination(Agent.WorkPlace.GatheringPost.EntrancePos.GetSelfPosition(Agent));
            
            Agent.SetAnimatorBool(Carrying, true);
            Agent.StartCoroutine(Agent.SpawnItem(CitizenManager.Instance.boxPrefab));

            yield break;
        }

        public override IEnumerator ExitState()
        {
            Agent.SetAnimatorBool(Carrying, false);
            Agent.StartCoroutine(Agent.RemoveItem());
            yield break;
        }

        public override void FrameUpdate()
        {
            if (Agent.WorkPlace.GatheringPost.EntrancePos.DoesAccept(Agent))
            {
                DoTick = false;
                Agent.SetAnimatorTrigger(DoPlaceBox);
                // TODO: maybe separate state for depositing resources
                Agent.Delayed(0.3f + Random.Range(-0.1f, 0.1f), () =>
                {
                    Agent.ProductDepositer.DepositInventory(Agent.Inventory);
                    Agent.WorkPlace.GatheringPost.EntrancePos.Dequeue();
                    DoTick = true;
                    Agent.StartCoroutine(StateMachine.ChangeState(Agent.GoWorkState));
                });
            }
        }

        public override void Renavigate()
        {
            var newPos = Agent.WorkPlace.GatheringPost.EntrancePos.GetSelfPosition(Agent);
            Agent.navMeshAgent.SetDestination(newPos);
        }
    }
}