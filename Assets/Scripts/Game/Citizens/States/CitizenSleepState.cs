using System.Collections;
using Game.DayNight;
using UnityEngine;

namespace Game.Citizens.States
{
    public class CitizenSleepState: CitizenState
    {
        public CitizenSleepState(CitizenAgent agent, CitizenStateMachine stateMachine) : base(agent, stateMachine)
        {
            
        }
        
        public override IEnumerator EnterState()
        {
            Agent.navMeshAgent.isStopped = true;
            // Agent.navMeshAgent.enabled = false;
            Agent.HideSelf();
            yield break;
        }

        public override IEnumerator ExitState()
        {
            Agent.navMeshAgent.enabled = true;
            Agent.navMeshAgent.isStopped = false;
            Agent.ShowSelf();
            yield break;
        }

        public override void FrameUpdate()
        {
            if (DayCycleManager.Instance.DayTimeMinutes() > 200) return;
            if (Agent.WorkPlace != null)
                Agent.StartCoroutine(StateMachine.ChangeState(Agent.GoWorkState));
            else
                Agent.StartCoroutine(StateMachine.ChangeState(Agent.WanderState));
        }
    }
}