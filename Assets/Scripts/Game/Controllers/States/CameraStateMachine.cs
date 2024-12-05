using System.Collections;
using UnityEngine;

namespace Game.Controllers.States
{
    public class CameraStateMachine
    {
        public CameraState CurrentState;
        public TownCameraController Camera;

        private bool _doTick;

        public CameraStateMachine(TownCameraController camera)
        { 
            Camera = camera;
        }

        public void Init(CameraState initState)
        {
            _doTick = false;
            CurrentState = initState;
            Camera.StartCoroutine(initState.EnterState());
            _doTick = true;
        }

        public IEnumerator SwitchState(CameraState newState)
        {
            _doTick = false;
            yield return CurrentState.ExitState();
            CurrentState = newState;
            yield return newState.EnterState();
            _doTick = true;
        }

        public void LateFrameUpdate()
        {
            if(_doTick && CurrentState.DoTick)
                CurrentState.LateFrameUpdate();
        }

        public void ShouldClampTo(Vector3 pos)
        {
            if (_doTick && CurrentState.DoTick)
                CurrentState.ShouldClampTo(pos);
        }
    }
}