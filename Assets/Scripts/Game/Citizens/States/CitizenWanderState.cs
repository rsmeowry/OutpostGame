using System.Collections;
using External.Util;
using UnityEngine;

namespace Game.Citizens.States
{
    public class CitizenWanderState: CitizenState
    {
        public CitizenWanderState(CitizenAgent agent, CitizenStateMachine stateMachine) : base(agent, stateMachine)
        {
            
        }

        private float _waitTime = 0f;
        private Goal _goal = Goal.Go;

        public override IEnumerator EnterState()
        {
            Agent.wanderAnchor = Vectors.RemapXYToXZ(Random.insideUnitCircle) * 25 +
                                 Agent.Home.EntrancePos.transform.position;
            Agent.navMeshAgent.isStopped = false;
            Agent.navMeshAgent.enabled = true;
            Agent.navMeshAgent.SetDestination(GetWanderPosition());
            yield break;
        }

        public override IEnumerator ExitState()
        {
            Agent.navMeshAgent.isStopped = true;
            yield break;
        }

        public override void FrameUpdate()
        {
            if (_goal == Goal.Go)
            {
                if (Agent.navMeshAgent.remainingDistance <= Agent.navMeshAgent.stoppingDistance)
                {
                    UpdateGoal();
                }
            }
            else
            {
                _waitTime -= Time.deltaTime;
                if (!(_waitTime <= 0f)) return;
                
                _waitTime = 0f;
                var shouldGo = Rng.Bool();
                if (shouldGo)
                {
                    _goal = Goal.Go;
                    Agent.navMeshAgent.SetDestination(GetWanderPosition());
                }
                else
                {
                    _goal = Goal.Wait;
                    _waitTime = Random.Range(0.5f, 1.5f);
                }
            }
        }

        private void UpdateGoal()
        {
            var shouldGo = Rng.Bool();
            if (shouldGo)
            {
                _goal = Goal.Go;
                var pos = GetWanderPosition();
                Agent.navMeshAgent.SetDestination(pos);
            }
            else
            {
                _goal = Goal.Wait;
                _waitTime = Random.Range(0.5f, 1.5f);
            }

        }

        private Vector3 GetWanderPosition()
        {
            return Vectors.RemapXYToXZ(Random.insideUnitCircle) * Agent.wanderRange + Agent.wanderAnchor;
        }

        private enum Goal
        {
            Wait,
            Go,
        }
    }
}