using System;
using System.Collections.Generic;
using System.Linq;
using External.Util;
using Game.Citizens;
using Game.Production.POI;
using UnityEngine;

namespace UI.POI
{
    public class CitizenSelectorParent: MonoBehaviour
    {
        public PanelViewPOI parentPanel;
        public List<CitizenCaste> acceptedProfessions;

        [SerializeField]
        private SingleProfessionSelector singlePrefab;
        
        private List<SingleProfessionSelector> _children = new();

        public void Init()
        {
            foreach (var caste in acceptedProfessions)
            {
                var single = Instantiate(singlePrefab, transform);
                single.parent = this;
                single.caste = caste;
                single.PreInit();
                _children.Add(single);
            }
            
            PollChildren();
            
            CitizenManager.Instance.onCitizenFired.AddListener(PollChildren);
        }

        private void OnDisable()
        {
            CitizenManager.Instance.onCitizenFired.RemoveListener(PollChildren);
        }

        public void PollChildren()
        {
            foreach(var child in _children)
                child.PollChanges();
        }

        public void AssignCitizen(CitizenCaste caste)
        {
            var ctzn = CitizenManager.Instance.FindUnassignedCitizen(caste);
            if (ctzn == null)
            {
                Debug.Log("NULL????");
                // TODO: just a placeholder!! this should be predicted via some event call
                PollChildren();
                return;
            }

            ((ResourceContainingPOI)parentPanel.poi).HireAgent(ctzn);
            PollChildren();
        }

        public void RemoveCitizen(CitizenCaste caste)
        {
            var citizen = ((ResourceContainingPOI) parentPanel.poi).AssignedAgents.FirstOrDefault(it => it.PersistentData.Profession == caste);
            if (citizen == null)
            {
                PollChildren();
                return;
            }
            citizen.Free();
            PollChildren();
        }
        
        public bool CanAssign(CitizenCaste caste)
        {
            var rcPoi = (ResourceContainingPOI)parentPanel.poi;
            return rcPoi.AssignedAgents.Count < rcPoi.capacity && CitizenManager.Instance.AnyFree(caste);
        }

        public bool CanRemove(CitizenCaste caste)
        {
            return ((ResourceContainingPOI) parentPanel.poi).AssignedAgents.Count(it => it.PersistentData.Profession == caste) > 0;
        }
    }
}