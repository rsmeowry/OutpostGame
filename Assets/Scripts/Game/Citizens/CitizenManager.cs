﻿using System;
using System.Collections.Generic;
using System.Linq;
using External.Util;
using Game.Production.POI;
using UnityEngine;

namespace Game.Citizens
{
    public class CitizenManager: MonoBehaviour
    {
        public static CitizenManager Instance { get; private set; }

        private int citizenIdTracker = 0;

        [SerializeField]
        private CitizenAgent citizenPrefab;
        
        private List<CitizenAgent> _citizens = new();

        public void Awake()
        {
            Instance = this;
        }

        [SerializeField]
        private ResourceContainingPOI poi;
        [ContextMenu("Test/Spawn Citizen")]
        private void __TestSpawn()
        {
            var citizen = SpawnCitizen(Vector3.zero + Vector3.up * 2);
            this.Delayed(5f, () =>
            {
                citizen.OrderTarget = poi;
                citizen.Order(citizen.GoWorkState);
            });
        }

        public CitizenAgent SpawnCitizen(Vector3 position)
        {
            var citizen = Instantiate(citizenPrefab);
            citizen.transform.position = position;
            citizen.citizenId = citizenIdTracker++;
            _citizens.Add(citizen);
            return citizen;
        }

        public CitizenAgent FindUnassignedCitizen(CitizenCaste caste)
        {
            return _citizens.FirstOrDefault(it => it.PersistentData.Profession == caste && it.IsUnoccupied());
        }

        public List<CitizenAgent> FindUnassignedCitizens(CitizenCaste caste, int amount)
        {
            return _citizens.Where(it => it.PersistentData.Profession == caste && it.IsUnoccupied()).Take(amount).ToList();
        }
    }
}