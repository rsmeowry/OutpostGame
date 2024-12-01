﻿using System;
using System.Collections.Generic;
using External.Util;
using Game.Building;
using Game.Citizens;
using Game.Citizens.Navigation;
using Game.POI.Deco;
using Game.Production.POI;
using UI.POI;
using UnityEngine;

namespace Game.POI.Housing
{
    public class HousePOI: PointOfInterest
    {
        [SerializeField]
        public int houseSize;
        [SerializeField]
        private QueuePosition entrance;

        public List<CitizenAgent> Tenants = new();

        public override QueuePosition EntrancePos => entrance;
        
        protected override void LoadForInspect(PanelViewPOI panel)
        {
            // TODO: panel housing view
        }

        private void Start()
        {
            if(doNotSerialize)
                CitizenManager.Instance.Houses.Add(this);
        }

        public override void OnBuilt()
        {
            CitizenManager.Instance.Houses.Add(this);
        }

        public override SerializedPOIData Serialize()
        {
            return new SerializedDecoPoi()
            {
                data = buildingData,
                originPrefabId = buildingData.BuildingType,
                position = BuildingManager.Instance.SnapToGrid(transform.position).Ser(),
                rotation = new Vector3(0f, buildingData.Rotation, 0f).Ser(),
                SelfId = Guid.Parse(pointId),
            };
        }
    }
}