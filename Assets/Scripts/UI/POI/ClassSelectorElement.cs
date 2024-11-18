using System;
using System.Collections.Generic;
using System.Linq;
using Game.Citizens;
using Game.Production.POI;
using UnityEngine;

namespace UI.POI
{
    public class ClassSelectorElement: MonoBehaviour
    {
        [SerializeField]
        private SingleClassSelectorElement singleClassPrefab;

        public ResourceContainingPOI parent;
        public List<CitizenCaste> supportedCastes;
        public int maximumWorkers;
        public List<CitizenAgent> currentWorkers;

        public void Start()
        {
            foreach (var caste in supportedCastes)
            {
                var obj = Instantiate(singleClassPrefab, transform);
                obj.caste = caste;
                obj.assignedAgents = currentWorkers.Where(it => it.PersistentData.Profession == caste).ToList();
            }
        }

        public void PollAdd(CitizenAgent agent)
        {
            currentWorkers.Add(agent);
            agent.OrderTarget = parent;
            
            agent.Order(agent.GoWorkState);
        }
        
        
    }
}