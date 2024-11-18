using System.Collections;
using UnityEngine;

namespace Game.Citizens
{
    public class CitizenStateMachine
    {
        public CitizenState CurrentState;

        private bool _doTick = false;
        public IEnumerator Init(CitizenState state)
        {
            CurrentState = state;
            yield return state.EnterState();
            _doTick = true;
        }

        public IEnumerator ChangeState(CitizenState newState)
        {
            Debug.Log($"BEGINNING STATE CHANGE TO {newState}");
            _doTick = false; // disabling ticking because state exit might take several frames
            yield return CurrentState.ExitState();
            CurrentState = newState;
            yield return newState.EnterState();
            _doTick = true;
            Debug.Log($"ENDED STATE CHANGE TO {newState}");
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