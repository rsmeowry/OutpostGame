using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Production.Products
{
    [CreateAssetMenu(fileName = "ProductDatabase", menuName = "Outpost/Product Database")]
    public class ProductDatabase: ScriptableObject
    {
        public List<SingleProductData> products;
    }

    [Serializable]
    public class SingleProductData
    {
        public string key;
        public Sprite icon;
        public string name;
    }
}