using System;
using System.Collections.Generic;
using Game.State;
using UnityEngine;

namespace Game.Production.Products
{
    public class ProductRegistry: MonoBehaviour
    {
        private static Dictionary<StateKey, ProductData> _entries = new();
        public static ProductRegistry Instance { get; private set; }

        private static StateKey Reg(string key, ProductData data)
        {
            var k = new StateKey(key);
            _entries[k] = data;
            return k;
        }

        public static StateKey IronOre = Reg("iron_ore", new ProductData());
        public static StateKey CopperOre = Reg("copper_ore", new ProductData());
        public static StateKey IronPlate = Reg("iron_plate", new ProductData());
        public static StateKey CopperWires = Reg("copper_wires", new ProductData());
        public static StateKey GoldOre = Reg("gold_ore", new ProductData());
        public static StateKey Stone = Reg("stone", new ProductData());
        
        public void Awake()
        {
            Instance = this;
            PreloadProducts();
        }

        private void PreloadProducts()
        {
            // TODO: ?
        }
    }
}