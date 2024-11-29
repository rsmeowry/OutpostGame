using System;
using System.Collections.Generic;
using External.Util;
using Game.Building;
using Game.Citizens;
using Game.Citizens.Navigation;
using Game.POI;
using Game.State;
using UI.POI;
using UnityEngine;

namespace Game.Production.POI
{
    public class GatheringPost: PointOfInterest, IProductDepositer
    {
        [SerializeField]
        private QueuePosition entrancePos;
        
        public override QueuePosition EntrancePos => entrancePos;
        protected override void LoadForInspect(PanelViewPOI panel)
        {
            throw new NotImplementedException();
        }

        public override SerializedPOIData Serialize()
        {
            return new GatheringPostData()
            {
                data = buildingData,
                originPrefabId = buildingData.BuildingType,
                position = BuildingManager.Instance.SnapToGrid(transform.position).Ser(),
                rotation = new Vector3(0f, buildingData.Rotation, 0f).Ser(),
                SelfId = Guid.Parse(pointId)
            };
        }

        public void DepositInventory(Dictionary<StateKey, int> inventory)
        {
            foreach (var key in inventory.Keys)
            {
                
                GameStateManager.Instance.IncreaseProduct(key, inventory[key]);
            }
            inventory.Clear();
        }
    }
    
    [Serializable]
    public class GatheringPostData: SerializedPOIData
    {
        
    }
}