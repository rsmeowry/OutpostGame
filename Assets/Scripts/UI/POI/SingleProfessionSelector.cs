using System;
using System.Linq;
using Game.Citizens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.POI
{
    public class SingleProfessionSelector: MonoBehaviour
    {
        private Button _buttonRemove;
        private Button _buttonAdd;
        private TMP_Text _counter;
        private TMP_Text _professionName;

        private Transform _infoTooltip;

        public CitizenSelectorParent parent;
        public CitizenCaste caste;
        
        public void PreInit()
        {
            _buttonRemove = transform.GetChild(0).GetComponent<Button>();
            _counter = transform.GetChild(1).GetComponent<TMP_Text>();
            _buttonAdd = transform.GetChild(2).GetComponent<Button>();
            _professionName = transform.GetChild(3).GetComponent<TMP_Text>();
            _infoTooltip = transform.GetChild(4);
            
            _buttonAdd.onClick.AddListener(() => parent.AssignCitizen(caste));
            _buttonRemove.onClick.AddListener(() => parent.RemoveCitizen(caste));
        }

        public void PollChanges()
        {
            _professionName.SetText(caste switch
            {
                CitizenCaste.Creator => "Творцы",
                CitizenCaste.Explorer => "Первопроходцы",
                CitizenCaste.Beekeeper => "Пасечники",
                CitizenCaste.Engineer => "Конструкторы",
                _ => throw new ArgumentOutOfRangeException()
            });

            _buttonAdd.interactable = parent.CanAssign(caste);
            _buttonRemove.interactable = parent.CanRemove(caste);
            
            _counter.SetText(parent.parentPanel.rcPoi.AssignedAgents.Count(it => it.PersistentData.Profession == caste).ToString());
        }
    }
}