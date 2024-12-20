﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using External.Util;
using Game.DayNight;
using Game.POI;
using Game.POI.Housing;
using Game.Production.POI;
using Game.Production.Products;
using Game.State;
using Game.Upgrades;
using Inside;
using Tutorial;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Game.Citizens
{
    public class CareerManager: MonoBehaviour
    {
        public static CareerManager Instance { get; private set; }

        public List<CareerOffer> CareerOffers = new();

        public UnityEvent onOffersUpdated = new();

        private readonly float[] _resourceModifier = { 0.5f, 0.8f, 0.9f, 0.9f, 0.9f, 1f, 1f, 1f, 1.2f, 1.5f, 2.5f, 2.6f, 3f, 3.1f, 4.5f, 6f, 8f, 9f, 11f, 12f, 12.1f, 12.4f, 11f, 15f, 20f, 30f, 35f, 40f, 50f };

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

            var portal = POIManager.Instance.LoadedPois.Values.FirstOrDefault(it => it is PortalPOI);
            var pos = portal == null ? PlayerBaseCenter.Instance.EntrancePos.transform.position : portal.EntrancePos.transform.position;
            CitizenManager.Instance.SpawnCitizen(pos, offer.CitizenData);
            CareerOffers.Remove(offer);
            onOffersUpdated.Invoke();
            
            TutorialCtl.Instance.ActiveStep?.ReceiveCitizenInvited();
        }

        private bool _busyUpdating;
        private void HandleDayChanged()
        {
            if (_busyUpdating)
                return;
            _busyUpdating = true;
            CareerOffers.Clear();
            
            var resModifier = DayCycleManager.Instance.days > _resourceModifier.Length
                ? 120f
                : _resourceModifier[DayCycleManager.Instance.days];
            var possibleResources = new List<StateKey>();
            var minResourceCount = 1;
            var maxResourceCount = 1;

            var citizenCount = CitizenManager.Instance.Citizens.Count;
            if (citizenCount < 9)
            {
                possibleResources.AddRange(new[] { ProductRegistry.Honey, ProductRegistry.Wood});

                if (DayCycleManager.Instance.days > 1)
                {
                    possibleResources.AddRange(new[] { ProductRegistry.Stone, ProductRegistry.CopperOre, ProductRegistry.IronOre });
                }
                
                var offrCount = UpgradeTreeManager.Instance.Has(Upgrades.Upgrades.AnotherHireSlot) ? 3 : 2;
                for (var i = 0; i < offrCount; i++)
                {
                    var offer = new CareerOffer
                    {
                        CitizenData = new PersistentCitizenData
                        {
                            Awards = new List<string>(),
                            Name = Rng.Bool() ? CitizenNames.RandomFemName() : CitizenNames.RandomMascName(),
                            Profession = CitizenNames.RandomCaste()
                        },
                        CurrencyRequest = Mathf.RoundToInt(Random.Range(35, 70) * resModifier)
                    };
                    var projectedResources = Random.Range(minResourceCount, maxResourceCount);
                    var res = new Dictionary<StateKey, int>();
                    for (var j = 0; j < projectedResources; j++)
                    {
                        res[Rng.Choice(possibleResources)] = Mathf.RoundToInt(Random.Range(6, 24) * resModifier);
                    }

                    offer.ResourceRequest = res;
                    CareerOffers.Add(offer);
                }
            
                onOffersUpdated.Invoke();
                _busyUpdating = false;
                return;
            }

            var citizenCountCoeff = Mathf.Sqrt(0.03f * citizenCount);
            resModifier *= citizenCountCoeff;
            
            possibleResources.AddRange(new[] { ProductRegistry.Stone, ProductRegistry.CopperOre, ProductRegistry.Honey, ProductRegistry.Wood, ProductRegistry.IronOre });
            if (DayCycleManager.Instance.days > 7)
            {
                // if we are on day 4+
                possibleResources.AddRange(new[] { ProductRegistry.IronPlate, ProductRegistry.Bricks, ProductRegistry.CopperIngot });
            }

            if (DayCycleManager.Instance.days > 8)
            {
                // if we are on day 5+
                possibleResources.AddRange(new[] { ProductRegistry.IronPlate, ProductRegistry.Bricks, ProductRegistry.CopperIngot, ProductRegistry.CopperWires });
            }

            if (DayCycleManager.Instance.days > 11)
            {
                maxResourceCount = 2;
            }

            if (DayCycleManager.Instance.days > 15)
            {
                // day 6+
                possibleResources.AddRange(new[] { ProductRegistry.Cog, ProductRegistry.IronBars });
            }

            if (DayCycleManager.Instance.days > 20)
            {
                minResourceCount = 2;
            }

            if (DayCycleManager.Instance.days > 23)
            {
                // day 11+
                possibleResources.AddRange(new[] { ProductRegistry.Steel, ProductRegistry.Concrete });
            }
            
            // TODO: dynamic amount of citizens
            var offerCount = UpgradeTreeManager.Instance.Has(Upgrades.Upgrades.AnotherHireSlot) ? 3 : 2;
            for (var i = 0; i < offerCount; i++)
            {
                var offer = new CareerOffer
                {
                    CitizenData = new PersistentCitizenData
                    {
                        Awards = new List<string>(),
                        Name = Rng.Bool() ? CitizenNames.RandomFemName() : CitizenNames.RandomMascName(),
                        Profession = CitizenNames.RandomCaste()
                    },
                    CurrencyRequest = Mathf.RoundToInt(Random.Range(35, 70) * resModifier)
                };
                var projectedResources = Random.Range(minResourceCount, maxResourceCount);
                var res = new Dictionary<StateKey, int>();
                for (var j = 0; j < projectedResources; j++)
                {
                    res[Rng.Choice(possibleResources)] = Mathf.RoundToInt(Random.Range(6, 24) * resModifier);
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