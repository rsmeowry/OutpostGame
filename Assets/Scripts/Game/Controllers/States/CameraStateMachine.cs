using System.Collections;

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
    }
}