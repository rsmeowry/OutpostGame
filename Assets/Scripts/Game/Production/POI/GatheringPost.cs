using System.Collections.Generic;
using Game.Citizens;
using Game.Citizens.Navigation;
using Game.POI;
using Game.State;
using UnityEngine;

namespace Game.Production.POI
{
    public class GatheringPost: PointOfInterest, IProductDepositer
    {
        [SerializeField]
        private QueuePosition entrancePos;
        
        public override QueuePosition EntrancePos => entrancePos;
        public void DepositInventory(Dictionary<StateKey, int> inventory)
        {
            foreach (var key in inventory.Keys)
            {
                
                GameStateManager.Instance.IncreaseProduct(key, inventory[key]);
            }
            inventory.Clear();
        }
    }
}