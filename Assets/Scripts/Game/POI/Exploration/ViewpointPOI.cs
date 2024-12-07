using System;
using External.Util;
using Game.Bounds;
using Game.Building;
using Game.Citizens;
using Game.Citizens.Navigation;
using Game.POI.Deco;
using UI.POI;
using UnityEngine;

namespace Game.POI.Exploration
{
    public class ViewpointPOI: PointOfInterest
    {
        public override string PoiDesc => data.description;

        public override QueuePosition EntrancePos => null;
        
        public override void OnBuilt()
        {
            BoundaryManager.Instance.ExpandBoundaries(transform.position);
        }

        protected override void LoadForInspect(PanelViewPOI panel)
        {
        }

        public override SerializedPOIData Serialize()
        {
            return new SerializedDecoPoi
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