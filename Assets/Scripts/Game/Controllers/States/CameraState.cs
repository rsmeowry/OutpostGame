using System.Collections;
using UnityEngine;

namespace Game.Controllers.States
{
    public abstract class CameraState
    {
        protected TownCameraController CameraController;
        protected CameraStateMachine StateMachine;
        public bool DoTick = true;
        
        public CameraState(CameraStateMachine stateMachine, TownCameraController cameraController)
        {
            CameraController = cameraController;
            StateMachine = stateMachine;
        }
        
        public abstract IEnumerator ExitState();
        public abstract IEnumerator EnterState();

        public virtual void LateFrameUpdate()
        {
            
        }

        public virtual void ShouldClampTo(Vector3 pos)
        {
            
        }
    }
}