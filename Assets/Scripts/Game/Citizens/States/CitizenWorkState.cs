using System;
using System.Collections;
using Game.Production;
using UI.POI;
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
            yield return new WaitForSeconds(0.2f);
            if (Agent.WorkPlace == null)
            { 
                yield return StateMachine.ChangeState(Agent.WanderState);
                yield break;
            }
            yield return Agent.WorkPlace.EnterWorkPlace(Agent, (s) =>
            {
                switch (s)
                {
                    case WorkPlaceEnterResult.Accepted:
                        Agent.navMeshAgent.enabled = true;
                        Agent.navMeshAgent.isStopped = true;
                        // Agent.navMeshAgent.enabled = false;
                        break;
                    case WorkPlaceEnterResult.Declined:
                        Agent.navMeshAgent.enabled = true;
                        Agent.Order(Agent.WanderState);
                        break;
                    case WorkPlaceEnterResult.NeedToMoveToSpot:
                        Agent.navMeshAgent.enabled = true;
                        Agent.StartCoroutine(StateMachine.ChangeState(Agent.MoveToWorkSpotState));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(s), s, null);
                }
            });
        }

        public override IEnumerator ExitState()
        {
            Agent.navMeshAgent.enabled = true;
            Agent.navMeshAgent.isStopped = false;
            
            if (Agent.WorkPlace != null)
            {
                if (Agent.WorkPlace.IsCurrentlyWorking(Agent))
                {
                    yield return Agent.WorkPlace.LeaveWorkPlace(Agent);
                }
            }
            
            // Agent.navMeshAgent.SetDestination(Agent.OrderTarget.EntrancePos.GetSelfPosition(Agent));
            // while (Agent.navMeshAgent.remainingDistance > Agent.navMeshAgent.stoppingDistance)
            // {
                // yield return null;
            // }
            // yield break;
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