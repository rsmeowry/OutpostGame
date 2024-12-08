using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using External.Achievement;
using External.Util;
using Game.Building;
using Game.Citizens;
using Game.Citizens.Navigation;
using Game.Controllers.States;
using Game.Electricity;
using Game.POI;
using Tutorial;
using UI.POI;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Production.POI
{
    // Forest clearing + Rock stuff etc.
    public abstract class ResourceContainingPOI: PointOfInterest, ICitizenWorkPlace
    {
        public List<Transform> citizenWorkingPositions;
        public string citizenAnimation;
        public int capacity;
        public QueuePosition entrancePos;
        public List<CitizenCaste> acceptedProfessions;
        
        private int _lastFreePos;
        private bool _isFull;

        public List<CitizenAgent> AssignedAgents = new();

        protected List<CitizenAgent> CitizensInside = new();
        [SerializeField]
        private GameObject tool;

        private Dictionary<int, Transform> _citizenWorkPos;

        public virtual void PreAnimation(CitizenAgent agent)
        {
            
        }

        private void RecalculateSpots()
        {
            if (citizenWorkingPositions.Count <= 0)
                return;
            _citizenWorkPos = new Dictionary<int, Transform>();
            for (var i = 0; i < AssignedAgents.Count; i++)
            {
                _citizenWorkPos[AssignedAgents[i].citizenId] = citizenWorkingPositions[i];
            }
        }

        public bool HireAgent(CitizenAgent agent)
        {
            // TODO: in tutorial this might kidna softlock us when we hire ppl in wrong order
            if (_isFull)
                return false;
            Debug.Log("HIRING");
            AssignedAgents.Add(agent);
            RecalculateSpots();
            _isFull = AssignedAgents.Count >= capacity;

            if (AssignedAgents.Count >= 4)
                AchievementManager.Instance.GiveAchievement(Achievements.ALotOfWork);
            
            agent.MarkHiredAt(this);
            
            // tutorial
            TutorialCtl.Instance.ActiveStep?.ReceiveCitizenHired(agent, this);
            
            return true;
        }

        public IEnumerator BeginWork(CitizenAgent agent, Action<WorkPlaceEnterResult> callback)
        {
            _awaitingArrival.Remove(agent);
            
            var spot = _citizenWorkPos[agent.citizenId];
            agent.transform.position = spot.position;
            agent.transform.DORotateQuaternion(spot.rotation, 0.5f).SetEase(Ease.Linear).Play();
            
            PreAnimation(agent);
            if (tool != null)
            { 
                yield return agent.SpawnItem(tool);
            }

            agent.PlayAnimation(citizenAnimation);

            callback(WorkPlaceEnterResult.Accepted);
        }

        private List<CitizenAgent> _awaitingArrival = new();
        
        public IEnumerator EnterWorkPlace(CitizenAgent agent, Action<WorkPlaceEnterResult> callback)
        {
            if (!AssignedAgents.Contains(agent))
            {
                callback(WorkPlaceEnterResult.Declined);
                yield break;
            }

            if (_awaitingArrival.Contains(agent))
            {
                CitizensInside.Add(agent);
                yield return BeginWork(agent, callback);
                yield break;
            }
            
            if (citizenWorkingPositions.Count > 0)
            {
                _awaitingArrival.Add(agent);

                callback(WorkPlaceEnterResult.NeedToMoveToSpot);
                yield break; 
            }
            
            CitizensInside.Add(agent);
            
            agent.HideSelf();
            agent.navMeshAgent.isStopped = true;
            // agent.navMeshAgent.enabled = false;

            callback(WorkPlaceEnterResult.Accepted);
        }

        public bool IsCurrentlyWorking(CitizenAgent agent)
        {
            return CitizensInside.Contains(agent);
        }

        public virtual void Fire(CitizenAgent agent)
        {
            AssignedAgents.Remove(agent);
            RecalculateSpots();
            entrancePos?.DequeueIdxd(agent);
            agent.ShowSelf();
            _isFull = false;
        }

        public Vector3 DesignatedWorkingSpot(CitizenAgent agent)
        {
            RecalculateSpots();
            return _citizenWorkPos.TryGetValue(agent.citizenId, out var po) ? po.position : transform.position;
        }

        public virtual IEnumerator LeaveWorkPlace(CitizenAgent agent)
        {
            if (!CitizensInside.Contains(agent))
            {
                yield break;
            }

            CitizensInside.Remove(agent);
            if (citizenWorkingPositions.Count > 0)
            {
                // tool disappear animation
                StartCoroutine(agent.RemoveItem());
                
                agent.PlayAnimation("Walk");
                _isFull = false;
            }
            else
            {
                agent.ShowSelf();
                agent.PlayAnimation("Walk");
                _isFull = false;
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var pos in citizenWorkingPositions)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(pos.position, 0.5f);
                Gizmos.DrawLine(pos.position, pos.position + pos.forward * 8f);
            }
        }

        public abstract void WorkerTick(CitizenAgent agent);
        public override QueuePosition EntrancePos => entrancePos;

        private PointOfInterest _gatheringPost;

        public virtual PointOfInterest GatheringPost
        {
            get
            {
                if(_gatheringPost == null)
                    RecalculateNearestGatheringPost();
                return _gatheringPost;
            }
        }

        public void RecalculateNearestGatheringPost()
        {
            var loadedPoi = POIManager.Instance.LoadedPois.Values
                .Where(it => it is GatheringPost)
                .OrderBy(it => (it.EntrancePos.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();
            var p = loadedPoi == null ? PlayerBaseCenter.Instance : loadedPoi;
            _gatheringPost = p;
        }

        public override SerializedPOIData Serialize()
        {
            if (doNotSerialize)
                return null;
            return new SerializedProductionPOI
            {
                assignedAgents = AssignedAgents.Select(it => it.citizenId).ToList(),
                originPrefabId = buildingData.BuildingType,
                position = BuildingManager.Instance.SnapToGrid(transform.position).Ser(),
                rotation = new Vector3(0f, buildingData.Rotation, 0f).Ser(),
                data = buildingData,
                SelfId = Guid.Parse(pointId)
            };
        }

        protected override void LoadForInspect(PanelViewPOI panel)
        {
            // hire citizens
            panel.AddCitizenHireSelector();
        }
    }
    
    [Serializable]
    public class SerializedProductionPOI: SerializedPOIData
    {
        public List<int> assignedAgents;
    }
}