using System;
using System.Collections.Generic;
using External.Util;
using Game.Building;
using Game.Citizens;
using Game.Citizens.Navigation;
using UnityEngine;

namespace Game.POI.Deco
{
    public class GenericDecoPoi: PointOfInterest
    {
        public QueuePosition entrancePos;

        public override QueuePosition EntrancePos => entrancePos;
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

    [Serializable]
    public class SerializedDecoPoi: SerializedPOIData
    {
        
    }
}