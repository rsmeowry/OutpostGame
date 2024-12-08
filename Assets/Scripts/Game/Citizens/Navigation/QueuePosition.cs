using System;
using System.Collections.Generic;
using System.Linq;
using External.Network;
using External.Util;
using Game.Production.POI;
using Game.Stocks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Citizens.Navigation
{
    public class QueuePosition: MonoBehaviour
    {
        public Queue<CitizenAgent> QueuedAgents = new();

        private Dictionary<int, int> _cachedPositions = new();
        private Dictionary<int, float> _waitingTime = new();

        public Vector3 GetSelfPosition(CitizenAgent self)
        {
            if (!QueuedAgents.Contains(self))
                Enqueue(self);
            return transform.position + -2 * QueuedAgents.ToList().FindIndex(it => it.citizenId == self.citizenId) * transform.forward;
        }

        public void Enqueue(CitizenAgent self)
        {
            QueuedAgents.Enqueue(self);
            RecacheIndices();
        }

        public void Dequeue()
        {
            var agent = QueuedAgents.Dequeue();
            _waitingTime.Remove(agent.citizenId);
            _cachedPositions.Remove(agent.citizenId);
            RecacheIndices();
        }

        public void DequeueIdxd(CitizenAgent agnt)
        {
            if (!QueuedAgents.Contains(agnt))
            {
                RecacheIndices();
                return;
            }

            var list = QueuedAgents.ToList();
            list.Remove(agnt);
            QueuedAgents = new Queue<CitizenAgent>(list);
            _waitingTime.Remove(agnt.citizenId);
            _cachedPositions.Remove(agnt.citizenId);
            RecacheIndices();
        }

        public bool DoesAccept(CitizenAgent self)
        {
            if(!_cachedPositions.ContainsKey(self.citizenId))
                Enqueue(self);
            if (!self.navMeshAgent.enabled)
                self.navMeshAgent.enabled = true;
            var isNear = self.navMeshAgent.remainingDistance <= self.navMeshAgent.stoppingDistance * 5f;
            if (isNear && !_waitingTime.ContainsKey(self.citizenId))
                _waitingTime[self.citizenId] = Time.time; // softlock prevention
            var justGoIn = Time.time - _waitingTime.GetValueOrDefault(self.citizenId, Time.time) >= WaitingTime(self);
            return  justGoIn || (_cachedPositions[self.citizenId] == 0 && isNear);
        }
        
        public void OnTriggerEnter(Collider other)
        {
            // Debug.Log($"ENTERED TRIGGER {other}");
            // if (!other.CompareTag("Unit"))
            //     return;
            //
            // var agent = other.GetComponent<CitizenAgent>();
            // if(!QueuedAgents.Contains(agent))
            //     return;
            //
            // var awaitedIndex = QueuedAgents.ToList().FindIndex(it => it.citizenId == agent.citizenId);
            //
            // if (awaitedIndex != 0)
            //     return;
        }

        private void RecacheIndices()
        {
            QueuedAgents = new Queue<CitizenAgent>(QueuedAgents.OrderBy(it => (it.transform.position - transform.position).sqrMagnitude)
                .ToList());
            _cachedPositions = QueuedAgents.ToList().Select((it, idx) => (it, idx))
                .ToDictionary(it => it.it.citizenId, it => it.idx);
            
            foreach (var agnt in QueuedAgents)
            {
                agnt.Renavigate();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position - transform.forward * 3f);
        }

        private float WaitingTime(CitizenAgent agent)
        {
            return 15f * (1 + (agent.GetHashCode() % 100) / 100f);
        }
    }
}