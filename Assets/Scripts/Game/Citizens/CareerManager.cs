using System;
using System.Collections.Generic;
using System.Threading;
using External.Util;
using Game.DayNight;
using Game.Production.Products;
using Game.State;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Game.Citizens
{
    // TODO: preferable add saving citizen offers to save file
    public class CareerManager: MonoBehaviour
    {
        public static CareerManager Instance { get; private set; }

        public List<CareerOffer> CareerOffers = new();

        public UnityEvent onOffersUpdated = new();

        private float[] resourceModifier = new[] { 0.5f, 0.8f, 1f, 1.5f, 2.5f, 2.6f, 3f, 3.1f, 4.5f, 6f, 8f, 9f, 11f, 12f, 12.1f, 12.4f, 11f, 15f, 20f, 30f, 35f, 40f, 50f };

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            DayCycleManager.Instance.onDayChanged.AddListener(HandleDayChanged);
            HandleDayChanged();
        }

        public void Hire(CareerOffer offer)
        {
            // TODO: sounds!!!!
            foreach (var res in offer.ResourceRequest)
            {
                GameStateManager.Instance.IncreaseProduct(res.Key, -res.Value);
            }
            GameStateManager.Instance.ChangeCurrency(-offer.CurrencyRequest, "Hired a citizen", true);
            
            CitizenManager.Instance.SpawnCitizen(new Vector3(180, 0, 220), offer.CitizenData);
            CareerOffers.Remove(offer);
            onOffersUpdated.Invoke();
        }

        private bool _busyUpdating;
        private void HandleDayChanged()
        {
            if (_busyUpdating)
                return;
            _busyUpdating = true;
            CareerOffers.Clear();
            
            var resModifier = DayCycleManager.Instance.days > resourceModifier.Length
                ? 120f
                : resourceModifier[DayCycleManager.Instance.days];
            var possibleResources = new List<StateKey>();
            var minResourceCount = 1;
            var maxResourceCount = 1;
            possibleResources.AddRange(new[] { ProductRegistry.Stone, ProductRegistry.CopperOre, ProductRegistry.Honey, ProductRegistry.Wood, ProductRegistry.IronOre });
            if (DayCycleManager.Instance.days > 1)
            {
                // if we are on day 2+
                possibleResources.AddRange(new[] { ProductRegistry.IronPlate, ProductRegistry.Bricks, ProductRegistry.CopperIngot });
            }

            if (DayCycleManager.Instance.days > 2)
            {
                // if we are on day 3+
                possibleResources.AddRange(new[] { ProductRegistry.IronPlate, ProductRegistry.Bricks, ProductRegistry.CopperIngot, ProductRegistry.CopperWires });
                maxResourceCount = 2;
            }

            if (DayCycleManager.Instance.days > 3)
            {
                // day 4+
                possibleResources.AddRange(new[] { ProductRegistry.Cog, ProductRegistry.IronBars });
            }

            if (DayCycleManager.Instance.days > 4)
            {
                // day 5+
                possibleResources.AddRange(new[] { ProductRegistry.Steel, ProductRegistry.Concrete });
                minResourceCount = 2;
            }
            
            // TODO: dynamic amount of citizens
            for (var i = 0; i < 2; i++)
            {
                var offer = new CareerOffer
                {
                    CitizenData = new PersistentCitizenData()
                    {
                        Awards = new List<string>(),
                        Name = Rng.Bool() ? CitizenNames.RandomFemName() : CitizenNames.RandomMascName(),
                        Profession = CitizenNames.RandomCaste()
                    },
                    CurrencyRequest = Mathf.RoundToInt(Random.Range(50, 80) * resModifier)
                };
                var projectedResources = Random.Range(minResourceCount, maxResourceCount);
                var res = new Dictionary<StateKey, int>();
                for (var j = 0; j < projectedResources; j++)
                {
                    res[Rng.Choice(possibleResources)] = Mathf.RoundToInt(Random.Range(10, 30) * resModifier);
                }

                offer.ResourceRequest = res;
                CareerOffers.Add(offer);
            }
            
            onOffersUpdated.Invoke();
            _busyUpdating = false;
        }
    }

    public class CareerOffer
    {
        public PersistentCitizenData CitizenData;
        public Dictionary<StateKey, int> ResourceRequest;
        public int CurrencyRequest;
    }
}