using System;
using System.Linq;
using External.Util;
using Game.Building;
using Game.Citizens;
using Game.Citizens.Navigation;
using Game.POI.Deco;
using UI.POI;
using UnityEngine;

namespace Game.POI.Housing
{
    public class PortalPOI: PointOfInterest, IBuildingWithConditions
    {
        [SerializeField]
        private QueuePosition entrancePos;

        public override QueuePosition EntrancePos => entrancePos;
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

        public bool CanBePlacedAt(Vector3 position)
        {
            var hasPortals = POIManager.Instance.LoadedPois.Values.Any(it => it is PortalPOI);
            return !hasPortals;
        }
    }
}