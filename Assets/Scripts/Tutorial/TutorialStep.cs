using Game.Building;
using Game.Citizens;
using Game.Production.POI;
using Game.State;
using TMPro;
using UI.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public abstract class TutorialStep: MonoBehaviour
    {
        public virtual void ReceiveMovementData(Vector3 pos)
        {
            
        }

        public virtual void ReceiveResourceData(StateKey item, int count)
        {
            
        }

        public virtual void ReceiveCurrencyData(int amount)
        {
            
        }

        public virtual void ReceiveModalClose()
        {
            
        }

        public virtual void ReceiveCitizenHired(CitizenAgent agent, ResourceContainingPOI poi)
        {
            
        }

        public virtual void ReceiveBuildingBuilt(BuildingData building)
        {
        }

        public virtual void ReceiveCitizenInvited()
        {
            
        }

        public virtual void ReceiveEnterHome()
        {
            
        }

        public void MarkDone()
        {
            TutorialCtl.Instance.StepCompleted();
        }

        public void UpdateTopBar(float newAmount)
        {
            TutorialCtl.Instance.topBarImage.fillAmount = newAmount;
        }

        public void UpdateTopBar(string newTitle)
        {
            TutorialCtl.Instance.topBar.GetComponentInChildren<TMP_Text>().text = newTitle;
        }

        public abstract void Activate();

        public abstract void DisplayModal(RectTransform modal);
    }
}