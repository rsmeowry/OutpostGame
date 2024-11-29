using System.Collections;
using UnityEngine;

namespace Game.Citizens.States
{
    public class CitizenMoveToWorkSpotState: CitizenState
    {
        public CitizenMoveToWorkSpotState(CitizenAgent agent, CitizenStateMachine stateMachine) : base(agent, stateMachine)
        {
            
        }
        
        public override IEnumerator EnterState()
        {
            Agent.navMeshAgent.isStopped = false;
            Agent.navMeshAgent.enabled = true;
            Agent.navMeshAgent.SetDestination(Agent.WorkPlace.DesignatedWorkingSpot(Agent));
            yield break;
        }

        public override void FrameUpdate()
        {
            FaceTarget();
            if (!Agent.navMeshAgent.enabled)
                Agent.navMeshAgent.enabled = true;
            if (Agent.navMeshAgent.remainingDistance <= Agent.navMeshAgent.stoppingDistance)
            {
                Agent.StartCoroutine(StateMachine.ChangeState(Agent.WorkState));
            }
        }
        
        private void FaceTarget()
        {
            var turnTowardNavSteeringTarget = Agent.navMeshAgent.steeringTarget;
      
            var direction = (turnTowardNavSteeringTarget - Agent.transform.position).normalized;
            var dir = new Vector3(direction.x, 0, direction.z);
            if (dir.sqrMagnitude >= 0.002)
            {
                var lookRotation = Quaternion.LookRotation(dir);
                Agent.transform.rotation = Quaternion.Slerp(Agent.transform.rotation, lookRotation, Time.deltaTime * 5);
            }
        }
        
        public override void Renavigate()
        {
            Agent.navMeshAgent.SetDestination(Agent.WorkPlace.DesignatedWorkingSpot(Agent));
        }
    }
}