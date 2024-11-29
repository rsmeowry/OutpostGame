﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Electricity
{
    public class ElectricityManager: MonoBehaviour
    {
        [SerializeField]
        private LayerMask electricityLayer;
        
        public static ElectricityManager Instance { get; private set; }

        private HashSet<IElectricityConsumer> _consumers = new();
        private HashSet<IElectricityProducer> _producers = new();
        private HashSet<SubstationPOI> _substations = new();

        private float _availablePower;
        private float _lastTick;

        public void Awake()
        {
            Instance = this;
        }

        public ElectricityStatistics GetStatistics()
        {
            var maxProd = 0f;
            var maxCons = 0f;
            foreach (var cons in _consumers)
            {
                maxCons += cons.MaxConsumption;
            }

            foreach (var prod in _producers)
            {
                maxProd += prod.MaxProduction;
            }

            return new ElectricityStatistics
            {
                MaxConsumption = maxCons,
                MaxProduction = maxProd
            };
        }

        public void AddSubstation(SubstationPOI sub)
        {
            _substations.Add(sub);
        }

        public bool IsCovering(Transform obj)
        {
            if (obj.TryGetComponent<IElectricityConsumer>(out var cons))
                _consumers.Add(cons);
            if (obj.TryGetComponent<IElectricityProducer>(out var prod))
                _producers.Add(prod);

            foreach (var sub in _substations)
            {
                var hits = new Collider[10];
                Physics.OverlapBoxNonAlloc(sub.transform.position, new Vector3(45f, 45f, 45f), hits, Quaternion.identity, electricityLayer);
                foreach (var hit in hits)
                {
                    if (hit == null)
                        continue;
                    if (obj.gameObject == hit.transform.gameObject)
                        return true;
                }
            }

            return false;
        }

        private void Tick()
        {
            if (Time.time - _lastTick >= 1)
            {
                _availablePower = 0f;
                foreach (var prod in _producers)
                {
                    _availablePower += prod.ProductionTick();
                }

                _lastTick = Time.time;
            }
        }

        public bool RequestPower(float amount)
        {
            Debug.Log("POWER REQUESTED");
            Tick();
            if (amount > _availablePower)
                return false;
            _availablePower -= amount;
            return true;
        }

        public void Cover(Transform tf)
        {
            var hits = new Collider[10];
            
            Physics.OverlapBoxNonAlloc(tf.position, new Vector3(45f, 50f, 45f), hits, Quaternion.identity, electricityLayer);
            foreach (var hit in hits)
            {
                if (hit == null)
                    continue;
                Debug.Log($"JUST HIT {hit.gameObject}");
                if (hit.TryGetComponent<IElectrical>(out var ele))
                {
                    Debug.Log("GOT COVERED");
                    ele.IsCovered = true;
                    
                    if (hit.TryGetComponent<IElectricityConsumer>(out var cons))
                        _consumers.Add(cons);
                    if (hit.TryGetComponent<IElectricityProducer>(out var prod))
                        _producers.Add(prod);
                }
            }
        }
    }

    public struct ElectricityStatistics
    {
        public float MaxProduction;
        public float MaxConsumption;
    }
}