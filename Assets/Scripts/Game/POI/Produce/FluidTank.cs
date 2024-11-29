using System;
using External.Util;
using Game.Building;
using Game.Citizens.Navigation;
using Game.Production.POI;
using Game.Production.Products;
using Game.State;
using UI.POI;
using UnityEngine;

namespace Game.POI.Produce
{
    public class FluidTank : PointOfInterest
    {
        private bool _wasActivated;
        public override QueuePosition EntrancePos => null;

        public override void OnBuilt()
        {
            base.OnBuilt();
            if (_wasActivated)
                return;
            GameStateManager.Instance.FluidLimits.Increment(ProductRegistry.Water, 50);
            _wasActivated = true;
        }

        protected override void LoadForInspect(PanelViewPOI panel)
        {
            // TODO: fluid display component
            throw new NotImplementedException();
        }

        public override void LoadData(SerializedPOIData pl)
        {
            _wasActivated = ((FluidTankSerializedData)pl).wasActivated;
        }

        public override SerializedPOIData Serialize()
        {
            return new FluidTankSerializedData
            {
                data = buildingData,
                originPrefabId = buildingData.BuildingType,
                position = BuildingManager.Instance.SnapToGrid(transform.position).Ser(),
                rotation = new Vector3(0f, buildingData.Rotation, 0f).Ser(),
                SelfId = Guid.Parse(pointId),
                wasActivated = _wasActivated
            };
        }
    }

    [Serializable]
    public class FluidTankSerializedData: SerializedPOIData
    {
        public bool wasActivated;
    }
}