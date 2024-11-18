using System.Collections.Generic;
using System.Linq;
using Game.Citizens;
using UnityEngine;

namespace UI.POI
{
    public class CitizenSelectorParent: MonoBehaviour
    {
        public InspectPanelPOI parentPanel;
        public List<CitizenCaste> acceptedProfessions;
        public SingleProfessionSelector singlePrefab;
        
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
        }

        public void PollChildren()
        {
            Debug.Log("Polling for the first time");
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
            parentPanel.rcPoi.AssignAgent(ctzn);
            PollChildren();
        }

        public void RemoveCitizen(CitizenCaste caste)
        {
            var citizen = parentPanel.rcPoi.AssignedAgents.FirstOrDefault(it => it.PersistentData.Profession == caste);
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
            return parentPanel.rcPoi.AssignedAgents.Count < parentPanel.rcPoi.capacity && CitizenManager.Instance.AnyFree(caste);
        }

        public bool CanRemove(CitizenCaste caste)
        {
            return parentPanel.rcPoi.AssignedAgents.Count(it => it.PersistentData.Profession == caste) > 0;
        }
    }
}