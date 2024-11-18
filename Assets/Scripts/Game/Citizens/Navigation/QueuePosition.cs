using System;
using System.Collections.Generic;
using System.Linq;
using External.Util;
using UnityEngine;

namespace Game.Citizens.Navigation
{
    public class QueuePosition: MonoBehaviour
    {
        public Queue<CitizenAgent> QueuedAgents = new();

        private Dictionary<int, int> _cachedPositions = new();

        public Vector3 GetSelfPosition(CitizenAgent self)
        {
            if (!QueuedAgents.Contains(self))
                Enqueue(self);
            return transform.position + (-3 * QueuedAgents.ToList().FindIndex(it => it.citizenId == self.citizenId) * transform.forward);
        }

        public void Enqueue(CitizenAgent self)
        {
            QueuedAgents.Enqueue(self);
            RecacheIndices();
        }

        public void Dequeue()
        {
            QueuedAgents.Dequeue();
            RecacheIndices();
        }

        public bool DoesAccept(CitizenAgent self)
        {
            if(!_cachedPositions.ContainsKey(self.citizenId))
                Enqueue(self);
            return _cachedPositions[self.citizenId] == 0 && (self.navMeshAgent.remainingDistance <= self.navMeshAgent.stoppingDistance);
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
            _cachedPositions = QueuedAgents.Select((it, idx) => (it, idx))
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
    }
}