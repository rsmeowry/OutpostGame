using System;
using Game.Building;
using Game.Citizens;
using Game.Controllers.States;
using Game.POI;
using Game.State;
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
        public CameraBuildingState BuildingState;
        public CameraRoadBuildingState RoadBuildingState;

        internal Camera Camera;

        public CameraInteractionFilter interactionFilter;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Camera = GetComponentInChildren<Camera>();
            StateMachine = new CameraStateMachine(this);
            FreeMoveState = new CameraFreeMoveState(StateMachine, this);
            FocusedState = new CameraFocusedState(StateMachine, this);
            BuildingState = new CameraBuildingState(StateMachine, this);
            RoadBuildingState = new CameraRoadBuildingState(StateMachine, this);
            
            StateMachine.Init(FreeMoveState);
        }

        public Vector3 MouseToWorld()
        {
            var ray = Camera.ScreenPointToRay(Input.mousePosition);
            var hit = new RaycastHit[1];
            Physics.RaycastNonAlloc(ray, hit, 100000f, BuildingManager.Instance.terrainLayer);
            return hit[0].point;
        }

        private void LateUpdate()
        {
            StateMachine.LateFrameUpdate();
        }

        public void ShouldClampTo(Vector3 position)
        {
            StateMachine.ShouldClampTo(position);
        }
    }

    public enum CameraInteractionFilter
    {
        ProductionAndCitizens,
        Intersections,
        None
    }
}
