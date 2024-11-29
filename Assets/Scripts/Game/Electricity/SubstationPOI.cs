using System;
using External.Util;
using Game.Building;
using Game.Citizens.Navigation;
using Game.POI;
using Game.POI.Deco;
using UI.POI;
using UnityEngine;

namespace Game.Electricity
{
    public class SubstationPOI: PointOfInterest
    {
        public override QueuePosition EntrancePos { get; }
        
        public override void OnBuilt()
        {
            base.OnBuilt();
            ElectricityManager.Instance.AddSubstation(this);
            ElectricityManager.Instance.Cover(transform);
        }

        protected override void LoadForInspect(PanelViewPOI panel)
        {
            throw new System.NotImplementedException();
        }

        public override SerializedPOIData Serialize()
        {
            return new SerializedDecoPoi()
            {
                data = buildingData,
                originPrefabId = buildingData.BuildingType,
                position = BuildingManager.Instance.SnapToGrid(transform.position).Ser(),
                rotation = new Vector3(0f, buildingData.Rotation, 0f).Ser(),
                SelfId = Guid.Parse(pointId)
            };
        }
    }
}