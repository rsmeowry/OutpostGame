using System;
using Game.POI.Housing;
using UnityEngine;

namespace UI.POI
{
    public class HouseStatistics: MonoBehaviour
    {
        [SerializeField]
        private SingleTenantStats singlePrefab;

        [NonSerialized]
        public HousePOI House;

        public void Start()
        {
            for (var i = 0; i < House.houseSize; i++)
            {
                var single = Instantiate(singlePrefab, transform);
                if (i < House.Tenants.Count)
                {
                    single.Agent = House.Tenants[i];
                }
            }
        }
    }
}