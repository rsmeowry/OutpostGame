using System;
using System.Collections.Generic;
using System.Linq;
using Game.Citizens;
using Game.Citizens.Navigation;
using Game.POI;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Production.POI
{
    // Forest clearing + Rock stuff etc.
    public abstract class ResourceContainingPOI: PointOfInterest, IOrderTarget
    {
        public List<Transform> citizenWorkingPositions;
        public string citizenAnimation;
        public int capacity;
        public QueuePosition entrancePos;
        
        private int _lastFreePos;
        private bool _isFull;

        private List<CitizenAgent> _citizens = new();
        private Dictionary<int, int> _citizenPos = new();
        private List<bool> _positions;

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
        
        public bool Accept(CitizenAgent agent)
        {
            if (_isFull)
                return false;
            _citizens.Add(agent);
            _isFull = _citizens.Count >= capacity;
            
            if (citizenWorkingPositions.Count > 0)
            {
                var spot = FindSpot();
                _citizenPos[agent.citizenId] = spot;
                // TODO: make citizen walk towards the pos?
                agent.transform.position = citizenWorkingPositions[spot].position;

                if (_positions.All(it => it))
                {
                    _isFull = true;
                }

                PreAnimation(agent);
                agent.GetComponent<Animator>().Play(citizenAnimation);
                // agent.GetComponent<NavMeshAgent>().enabled = false;
            }
            else
            {
                agent.HideSelf();
                agent.navMeshAgent.enabled = false;
            }

            return true;
        }


        public void Release(CitizenAgent agent)
        {
            if (!_citizens.Contains(agent))
                return;

            _citizens.Remove(agent);
            if (citizenWorkingPositions.Count > 0)
            {
                // TODO: remove pickaxe and shit
                ReleaseSpot(_citizenPos[agent.citizenId]);
                agent.PlayAnimation("Idle");
                _isFull = false;
            }
            else
            {
                agent.ShowSelf();
                agent.PlayAnimation("Idle");
                _isFull = false;
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
    }
}