using System;
using System.Collections.Generic;
using Game.Citizens.Navigation;
using Game.POI;
using Game.State;
using UnityEngine;

namespace Game.Production.POI
{
    public class PlayerBaseCenter: PointOfInterest, IProductDepositer
    {
        public static PlayerBaseCenter Instance { get; private set; }

        [SerializeField]
        private QueuePosition entrancePos;

        private void Start()
        {
            Instance = this;
        }

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