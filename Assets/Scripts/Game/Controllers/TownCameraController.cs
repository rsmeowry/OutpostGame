using System;
using Game.Controllers.States;
using Game.POI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Controllers
{
    public class TownCameraController: MonoBehaviour
    {
        [SerializeField] public float moveSpeed = 2f;
        [SerializeField] internal float moveTime = 2f;
        [FormerlySerializedAs("rotationTime")] [SerializeField]
        internal float rotationSpeed = 0.6f;
        [SerializeField] internal float zoomSpeed = 0.6f;

        [SerializeField] internal float zoomMin = 10f;

        [SerializeField] internal float zoomMax = 30f;
        
        [SerializeField] internal float zoomDampen = 2f;

        [SerializeField] internal Transform cameraTransform;
        
        public static TownCameraController Instance { get; private set; }
        
        public PointOfInterest FocusedPOI;

        public CameraStateMachine StateMachine;
        public CameraFreeMoveState FreeMoveState;
        public CameraFocusedState FocusedState;

        private void Start()
        {
            Instance = this;
            StateMachine = new CameraStateMachine(this);
            FreeMoveState = new CameraFreeMoveState(StateMachine, this);
            FocusedState = new CameraFocusedState(StateMachine, this);
            
            StateMachine.Init(FreeMoveState);
        }

        private void LateUpdate()
        {
            StateMachine.LateFrameUpdate();
        }
    }
}
