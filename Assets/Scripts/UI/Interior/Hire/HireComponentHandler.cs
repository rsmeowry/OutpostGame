using System;
using Game.Citizens;
using TMPro;
using UnityEngine;

namespace UI.Interior.Hire
{
    public class HireComponentHandler: MonoBehaviour
    {
        [SerializeField]
        private Transform offersContainer;

        [SerializeField]
        private GameObject noOffersText;

        [SerializeField]
        private SingleCitizenHireChoice singleOffer;
        
        private void OnEnable()
        {
            CareerManager.Instance.onOffersUpdated.AddListener(OnOffersChanged);
        }

        private void Start()
        {
            OnOffersChanged();
        }

        private void OnDisable()
        {
            CareerManager.Instance.onOffersUpdated.RemoveListener(OnOffersChanged);
        }
        
        private void OnOffersChanged()
        {
            for (var i = 0; i < offersContainer.childCount; i++)
            {
                Destroy(offersContainer.GetChild(i).gameObject);
            }

            if (CareerManager.Instance.CareerOffers.Count <= 0)
                Instantiate(noOffersText, offersContainer);
            else
            {
                foreach (var offer in CareerManager.Instance.CareerOffers)
                {
                    var off = Instantiate(singleOffer, offersContainer);
                    off.Offer = offer;
                }
            }
        }
    }
}