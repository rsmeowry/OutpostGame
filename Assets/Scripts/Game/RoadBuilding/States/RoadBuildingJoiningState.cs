using System;
using External.Util;
using Game.Building;
using Game.Controllers;
using UnityEngine;

namespace Game.RoadBuilding.States
{
    public class RoadBuildingJoiningState: RoadBuildingState
    {
        private Vector3 _beginPoint;
        private Vector3 _oldPos;
        private RoadIntersection _endIntersection;
        private RoadIntersection _currentIntersection;
        
        public RoadBuildingJoiningState(RoadManager manager, RoadBuildingStateMachine stateMachine) : base(manager, stateMachine)
        {
            
        }

        public override void EnterState()
        {
            _beginPoint = BuildingManager.Instance.SnapToGrid(Manager.ActiveIntersection.transform.position);
            _endIntersection = UnityEngine.Object.Instantiate(Manager.intersectionPrefab, Manager.transform);
        }
        
        public override void HandleClickOther(RoadIntersection other)
        {
            if (other != _currentIntersection)
                return;
            Manager.BuildRoad(other);
            StateMachine.ChangeState(Manager.InactiveState);
        }

        public override void FrameUpdate()
        {
            var worldPos = TownCameraController.Instance.MouseToWorld();
            var snappedPos = BuildingManager.Instance.SnapToGrid(worldPos);
            Manager.JointObject.transform.position =
                BuildingManager.Instance.LerpedSnap(Manager.JointObject.transform.position, _oldPos);
            if (_oldPos == snappedPos)
                return;
            _oldPos = snappedPos;

            var directionVector = worldPos - Manager.ActiveIntersection.transform.position;
            
            var projectionForward = Vector3.Project(directionVector, new Vector3(1f, 0f, 0f));
            var projectionUp = Vector3.Project(directionVector, new Vector3(0f, 0f, 1f));
            
            var lenX = projectionForward.x;
            var lenY = projectionUp.z;

            var newPos = Mathf.Abs(lenY) > Mathf.Abs(lenX)
                ? BuildingManager.Instance.SnapToGrid(Manager.ActiveIntersection.transform.position +
                                                      new Vector3(0f, 0f, lenY)) + new Vector3(5f, 5f, 5f)
                : BuildingManager.Instance.SnapToGrid(Manager.ActiveIntersection.transform.position +
                                                      new Vector3(lenX, 0f, 0f)) + new Vector3(5f, 5f, 5f);

            var snappedPosNew = BuildingManager.Instance.SnapToGrid(newPos);
            var interAt = Manager.GetIntersectionAt(snappedPosNew);
            Debug.Log($"INTERSECTION AT {snappedPosNew} - {interAt}");
            if (interAt != null)
            {
                _endIntersection.gameObject.SetActive(false);
                _currentIntersection = interAt;
            }
            else
            {
                if(!_endIntersection.gameObject.activeSelf)
                    _endIntersection.gameObject.SetActive(true);
                snappedPosNew.y = 0f;
                _endIntersection.transform.position = snappedPosNew;
            }
            
            Manager.JointObject.SetPosition(1, newPos);

            // TODO: check for intersection with existing intersection (words)

            // var difference = Manager.ActiveIntersection.transform.position - worldPos;
            // var oScale = Manager.JointObject.transform.localScale;
            // oScale.x = difference.x;

            // Manager.JointObject.transform.localScale = oScale;

            // Manager.JointObject.transform.position = worldPos;
        }
    }
}