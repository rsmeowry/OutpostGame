using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using External.Util;
using Game.Building;
using Game.Citizens;
using Game.Citizens.Navigation;
using Game.POI;
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

        private List<CitizenAgent> _citizensInside = new();
        private Dictionary<int, int> _citizenPos = new();
        private List<bool> _positions;
        [SerializeField]
        private GameObject tool;

        private void Awake()
        {
            _positions = new List<bool>(new bool[capacity]);
        }

        public virtual void PreAnimation(CitizenAgent agent)
        {
            
        }
        
        private int FindSpot()
        {
            for (int i = 0; i < _positions.Count; i++)
            {
                if (!_positions[i])
                {
                    _positions[i] = true;
                    return i; 
                }
            }
            return -1;
        }
        
        private void ReleaseSpot(int positionIndex)
        {
            if (positionIndex >= 0 && positionIndex < _positions.Count)
            {
                _positions[positionIndex] = false;
            }
        }

        public bool HireAgent(CitizenAgent agent)
        {
            if (_isFull)
                return false;
            AssignedAgents.Add(agent);
            _isFull = AssignedAgents.Count >= capacity;
            agent.MarkHiredAt(this);
            return true;
        }

        public IEnumerator BeginWork(CitizenAgent agent, Action<WorkPlaceEnterResult> callback)
        {
            _awaitingArrival.Remove(agent);
            
            var spot = _citizenPos[agent.citizenId];
            agent.transform.position = citizenWorkingPositions[spot].position;
            agent.transform.DORotateQuaternion(citizenWorkingPositions[spot].rotation, 0.5f).SetEase(Ease.Linear).Play();
                
            PreAnimation(agent);
            if (tool != null)
            {
                var t = Instantiate(tool, agent.rightArm);
                var originalScale = t.transform.localScale;
                t.transform.localScale = Vector3.zero;
                t.transform.DOScale(originalScale, 0.75f).SetEase(Ease.OutBack).Play();
            }

            yield return new WaitForSeconds(0.5f);
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
                _citizensInside.Add(agent);
                yield return BeginWork(agent, callback);
                yield break;
            }
            
            if (citizenWorkingPositions.Count > 0)
            {
                var spot = FindSpot();
                _citizenPos[agent.citizenId] = spot;
                
                _awaitingArrival.Add(agent);

                callback(WorkPlaceEnterResult.NeedToMoveToSpot);
                yield break; 
            }
            
            _citizensInside.Add(agent);
            
            agent.HideSelf();
            agent.navMeshAgent.enabled = false;

            callback(WorkPlaceEnterResult.Accepted);
        }

        public bool IsCurrentlyWorking(CitizenAgent agent)
        {
            return _citizensInside.Contains(agent);
        }

        public virtual void Fire(CitizenAgent agent)
        {
            AssignedAgents.Remove(agent);
            _isFull = false;
        }

        public Vector3 DesignatedWorkingSpot(CitizenAgent agent)
        {
            if (citizenWorkingPositions.Count < 0 || !_citizenPos.ContainsKey(agent.citizenId))
                return Vector3.zero;

            return citizenWorkingPositions[_citizenPos[agent.citizenId]].position;
        }

        public virtual IEnumerator LeaveWorkPlace(CitizenAgent agent)
        {
            if (!_citizensInside.Contains(agent))
            {
                yield break;
            }

            _citizensInside.Remove(agent);
            if (citizenWorkingPositions.Count > 0)
            {
                // TODO: remove pickaxe and shit
                ReleaseSpot(_citizenPos[agent.citizenId]);
                
                // tool disappear animation
                var tf = agent.rightArm.GetChild(0);
                tf.DOKill();
                tf.DOScale(Vector3.zero, 0.75f).SetEase(Ease.OutExpo)
                    .OnComplete(() => Destroy(agent.rightArm.GetChild(0).gameObject)).Play();
                
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

        public virtual PointOfInterest GatheringPost
        {
            get
            {
                var results = new Collider[10];
                Physics.OverlapSphereNonAlloc(transform.position, 25f, results);
                var firstPost = results.Where(it => it != null && it.gameObject.TryGetComponent(out GatheringPost post))
                    .OrderBy(it => (it.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
                if (firstPost == null)
                    return PlayerBaseCenter.Instance;
                return firstPost.GetComponent<GatheringPost>();
            }
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
    }
    
    [Serializable]
    public class SerializedProductionPOI: SerializedPOIData
    {
        public List<int> assignedAgents;
    }
}