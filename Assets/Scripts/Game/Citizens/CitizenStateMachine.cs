using System.Collections;
using Game.DayNight;

namespace Game.Citizens
{
    public class CitizenStateMachine
    {
        public CitizenState CurrentState;
        public CitizenAgent Agent;

        private bool _doTick = false;

        public CitizenStateMachine(CitizenAgent agent)
        {
            Agent = agent;
        }
        
        public IEnumerator Init(CitizenState state)
        {
            CurrentState = state;
            yield return state.EnterState();
            _doTick = true;
        }

        private bool _isSwitchingStates;
        public IEnumerator ChangeState(CitizenState newState)
        {
            if (CurrentState == Agent.SleepState && DayCycleManager.Instance.DayTimeMinutes() > 700)
                yield break; // nope
            _doTick = false; // disabling ticking because state exit might take several frames
            yield return CurrentState.ExitState();
            CurrentState = newState;
            yield return newState.EnterState();
            _doTick = true;
        }

        public void FrameUpdate()
        {
            if(_doTick && CurrentState.DoTick)
                CurrentState.FrameUpdate();
        }

        public void PhysicsUpdate()
        {
            if(_doTick && CurrentState.DoTick)
                CurrentState.PhysicsUpdate();
        }

        public void Renavigate()
        {
            if (_doTick && CurrentState.DoTick)
                CurrentState.Renavigate();
        }
    }
}