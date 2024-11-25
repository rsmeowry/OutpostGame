using System;
using System.Collections.Generic;
using System.Linq;
using Game.Building;
using Game.RoadBuilding.States;
using UnityEngine;

namespace Game.RoadBuilding
{
    public class RoadManager: MonoBehaviour
    {
        public static RoadManager Instance { get; private set; }

        // serialized
        private List<RoadIntersection> _roadPoints = new();

        [SerializeField]
        private Transform[] initialRoads;

        [SerializeField]
        internal RoadIntersection intersectionPrefab;

        [SerializeField]
        private GameObject roadDragPrefab;

        [SerializeField]
        private GameObject trailBuiltPrefab;

        [SerializeField]
        private GameObject gridPrefab;

        [SerializeField] [ColorUsage(false, true)]
        private Color badColor;
        [SerializeField] [ColorUsage(false, true)]
        private Color goodColor;

        internal RoadIntersection ActiveIntersection;
        internal LineRenderer JointObject;

        public RoadBuildingStateMachine StateMachine;
        public RoadBuildingInactiveState InactiveState;
        public RoadBuildingActiveState ActiveState;
        public RoadBuildingJoiningState JoiningState;
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private static readonly int GridColor = Shader.PropertyToID("_GridColor");

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            foreach (var t in initialRoads)
            {
                var inter = Instantiate(intersectionPrefab, transform);
                inter.transform.position = t.position;
                inter.Init();
                _roadPoints.Add(inter);
            }

            StateMachine = new RoadBuildingStateMachine(this);
            InactiveState = new RoadBuildingInactiveState(this, StateMachine);
            ActiveState = new RoadBuildingActiveState(this, StateMachine);
            JoiningState = new RoadBuildingJoiningState(this, StateMachine);
            
            StateMachine.Init(InactiveState);
        }

        public void HandleClick(RoadIntersection intersection)
        {
            if(ActiveIntersection != intersection)
                StateMachine.HandleClickOther(intersection);
            // ActiveIntersection = intersection;
            // StateMachine.ChangeState(JoiningState);
        }

        public void ShowAll()
        {
            foreach(var point in _roadPoints)
                point.Show();
        }

        public void HideAll()
        {
            foreach(var point in _roadPoints)
                point.Hide();
        }

        public void CreateJoint()
        {
            JointObject = Instantiate(roadDragPrefab, transform).GetComponent<LineRenderer>();
            JointObject.positionCount = 2;
            JointObject.SetPosition(0, ActiveIntersection.transform.GetChild(0).position);
            
            Instantiate(gridPrefab, JointObject.transform);
            
            JointObject.GetComponent<Renderer>().material.SetColor(Color1, goodColor);
            JointObject.GetComponentInChildren<Renderer>().material.SetColor(GridColor, goodColor);

            
        }

        public RoadIntersection GetIntersectionAt(Vector3 snappedPos)
        {
            // TODO: very inefficient -> rewrite this
            var snappedPosC = new Vector3(snappedPos.x, 0, snappedPos.z);
            return _roadPoints.FirstOrDefault(it => BuildingManager.Instance.SnapToGrid(it.transform.position) == snappedPosC);
        }

        private bool _isBad;
        public void MarkState(bool bad)
        {
            var wasBad = _isBad;
            _isBad = bad;
            ActiveIntersection.MarkState(bad);
            if (wasBad && !_isBad)
            {
                JointObject.GetComponent<Renderer>().material.SetColor(Color1, goodColor);
                JointObject.GetComponentInChildren<Renderer>().material.SetColor(GridColor, goodColor);
            } else if (!wasBad && _isBad)
            {
                JointObject.GetComponent<Renderer>().material.SetColor(Color1, badColor);
                JointObject.GetComponentInChildren<Renderer>().material.SetColor(GridColor, badColor);
            }
        }

        public void BuildRoad(RoadIntersection endPoint)
        {
            Destroy(JointObject);
            Debug.Log("BUILDING ROAD NOW!");
        }

        private void Update()
        {
            StateMachine.FrameUpdate();
        }
    }
}