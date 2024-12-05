using System;
using Game.Citizens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.POI
{
    public class SingleTenantStats: MonoBehaviour
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
        private Image casteIcon;
        [SerializeField]
        private TMP_Text citizenName;
        
        [NonSerialized]
        public CitizenAgent Agent;

        public void Start()
        {
            if (Agent == null)
            {
                citizenName.text = "<i>Свободное место";
            }
            else
            {
                citizenName.text = Agent.PersistentData.Name;
                casteIcon.sprite = Agent.PersistentData.Profession switch
                {
                    CitizenCaste.Creator => creatorIcon,
                    CitizenCaste.Explorer => explorerIcon,
                    CitizenCaste.Beekeeper => beekeeperIcon,
                    CitizenCaste.Engineer => engineerIcon,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}