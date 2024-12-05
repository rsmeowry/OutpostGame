using System.Collections;
using External.Util;
using Game.POI;
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

        private PointOfInterest _gatheringPost;

        public override IEnumerator EnterState()
        {
            _gatheringPost = Agent.WorkPlace.GatheringPost;
            if (Agent.ProductDepositer == null)
                Agent.ProductDepositer = (IProductDepositer) _gatheringPost;
            Debug.Log($"ENTERING STATE: {_gatheringPost}");
            Agent.navMeshAgent.enabled = true;
            Agent.navMeshAgent.SetDestination(_gatheringPost.EntrancePos.GetSelfPosition(Agent));
            
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
            if (_gatheringPost.EntrancePos.DoesAccept(Agent))
            {
                DoTick = false;
                Agent.SetAnimatorTrigger(DoPlaceBox);
                Agent.Delayed(0.3f + Random.Range(-0.1f, 0.1f), () =>
                {
                    Agent.ProductDepositer.DepositInventory(Agent.Inventory);
                    _gatheringPost.EntrancePos.Dequeue();
                    DoTick = true;
                    Agent.StartCoroutine(StateMachine.ChangeState(Agent.GoWorkState));
                });
            }
        }

        public override void Renavigate()
        {
            var newPos = _gatheringPost.EntrancePos.GetSelfPosition(Agent);
            Agent.navMeshAgent.SetDestination(newPos);
        }
    }
}