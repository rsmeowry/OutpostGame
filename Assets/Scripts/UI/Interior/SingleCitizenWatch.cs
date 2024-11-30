using System;
using Game.Citizens;
using Game.Citizens.States;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Interior
{
    public class SingleCitizenWatch: MonoBehaviour
    {
        [SerializeField]
        private Sprite beekeeperIcon;
        [SerializeField]
        private Sprite engineerIcon;
        [SerializeField]
        private Sprite explorerIcon;
        [SerializeField]
        private Sprite creatorIcon;
        
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text statusText;
        [SerializeField]
        private Image icon;

        [NonSerialized]
        public CitizenAgent Agent;

        private void Start()
        {
            icon.sprite = Agent.PersistentData.Profession switch
            {
                CitizenCaste.Creator => creatorIcon,
                CitizenCaste.Explorer => explorerIcon,
                CitizenCaste.Beekeeper => beekeeperIcon,
                CitizenCaste.Engineer => engineerIcon,
                _ => throw new ArgumentOutOfRangeException()
            };
            nameText.text = Agent.PersistentData.Name;
            statusText.text = "Прямо сейчас: " + Agent.StateMachine.CurrentState switch
            {
                CitizenCarryResourcesState => "несет ресурсы",
                CitizenGoWorkState => "идет работать",
                CitizenMoveToWorkSpotState => "почти работает",
                CitizenWanderState => "гуляет",
                CitizenWorkState => "работает",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}