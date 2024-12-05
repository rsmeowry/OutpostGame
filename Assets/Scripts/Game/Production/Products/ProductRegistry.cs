using System;
using System.Collections.Generic;
using System.Linq;
using External.Util;
using Game.Citizens;
using Game.State;
using UI.Interior.Stocks;
using UnityEngine;

namespace Game.Production.Products
{
    public class ProductRegistry: MonoBehaviour
    {
        public static readonly Dictionary<string, ProductData> Entries = new();
        public static ProductRegistry Instance { get; private set; }

        [SerializeField]
        private ProductDatabase products;

        private Dictionary<string, SingleProductData> _eachProduct = new();

        private static StateKey Reg(string key, ProductData data)
        {
            var k = new StateKey(key);
            Entries[k.Formatted()] = data;
            return k;
        }

        // products
        public static StateKey IronOre = Reg("iron_ore", new ProductData(5, 3, 1000));
        public static StateKey CopperOre = Reg("copper_ore", new ProductData(6, 4, 1000));
        public static StateKey IronPlate = Reg("iron_plate", new ProductData(9, 5, 800));
        public static StateKey IronBars = Reg("iron_bars", new ProductData(7, 3, 1200));
        public static StateKey CopperWires = Reg("copper_wires", new ProductData(8, 4, 2000));
        public static StateKey CopperIngot = Reg("copper_ingot", new ProductData(10, 5, 1100));
        public static StateKey Cog = Reg("cog", new ProductData(15, 8, 1200));
        public static StateKey Stone = Reg("stone", new ProductData(4, 2, 5000));
        public static StateKey Bricks = Reg("bricks", new ProductData(6, 3, 4000));
        public static StateKey Concrete = Reg("concrete", new ProductData(50, 12, 300));
        public static StateKey Steel = Reg("steel", new ProductData(50, 24, 500));
        public static StateKey Honey = Reg("honey", new ProductData(10, 4, 300));
        public static StateKey Wood = Reg("wood", new ProductData(5, 2, 1500));
        
        // fluids
        public static StateKey Water = new StateKey("water");
        
        private void Start()
        {
            foreach (var product in products.products)
            {
                var key = StateKey.FromString(product.key);
                _eachProduct[key.Formatted()] = product;
            }
        }

        public SingleProductData GetProductData(StateKey product)
        {
            return _eachProduct[product.Formatted()];
        }

        public HashSet<StateKey> AllItems()
        {
            return _eachProduct.Keys.Select(StateKey.FromString).ToHashSet();
        }
        
        public StateKey RandomItem()
        {
            return StateKey.FromString(Rng.Choice(Entries.Keys.ToList()));
        }

        public void Awake()
        {
            Instance = this;
        }
    }
}