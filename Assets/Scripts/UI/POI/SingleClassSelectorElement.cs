using System;
using System.Collections.Generic;
using Game.Citizens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.POI
{
    public class SingleClassSelectorElement: MonoBehaviour
    {
        public List<CitizenAgent> assignedAgents;
        public CitizenCaste caste;
        public ClassSelectorElement parent;

        private TMP_Text _count;
        private TMP_Text _casteText;
        private Button _buttonLess;
        private Button _buttonMore;
        private GameObject _citizenListTooltip; // TODO: tooltip stuff

        private bool _canAddMore = true;

        private void Start()
        {
            _buttonLess = transform.GetChild(0).GetComponent<Button>();
            _count = transform.GetChild(1).GetComponent<TMP_Text>();
            _buttonMore = transform.GetChild(2).GetComponent<Button>();
            _casteText = transform.GetChild(3).GetComponent<TMP_Text>();
            _citizenListTooltip = transform.GetChild(4).gameObject;

            _casteText.SetText(caste switch
            {
                CitizenCaste.Creator => "Творцы",
                CitizenCaste.Explorer => "Первопроходцы",
                CitizenCaste.Beekeeper => "Пасечники",
                CitizenCaste.Engineer => "Инженеры",
                _ => throw new ArgumentOutOfRangeException()
            });
            
            PollChanges();
        }

        public void AddAgent()
        {
            var unassigned = CitizenManager.Instance.FindUnassignedCitizen(caste);
            if (unassigned == null)
                return;
            
            AssignAgent(unassigned);
        }

        public void RemoveAgent()
        {
            var freed = assignedAgents[0];
            RemoveAgent(freed);
        }

        public void AssignAgent(CitizenAgent agent)
        {
            assignedAgents.Add(agent);
            parent.PollAdd(agent);
            PollChanges();
        }

        public void RemoveAgent(CitizenAgent agent)
        {
            assignedAgents.Add(agent);
            // TODO:
            // parent.PollRemove(agent);
            PollChanges();
        }

        public void StopAdding()
        {
            _canAddMore = false;
            PollChanges();
        }

        public void AllowAdding()
        {
            _canAddMore = true;
            PollChanges();
        }

        private void PollChanges()
        {
            var newCount = assignedAgents.Count;
            _count.SetText(newCount.ToString());

            if (newCount <= 0)
            {
                _buttonLess.interactable = false;
            }

            if (!_canAddMore)
            {
                _buttonMore.interactable = false;
            }

            if (newCount > 0 && !_citizenListTooltip.activeSelf)
            {
                _citizenListTooltip.SetActive(true);
            } else if (newCount <= 0 && _citizenListTooltip.activeSelf)
            {
                _citizenListTooltip.SetActive(false);
            }
        }
    }
}