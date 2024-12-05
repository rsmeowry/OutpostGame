using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Production
{
    [CreateAssetMenu(menuName = "Outpost/Recipe", fileName = "RecipeData")]
    public class RecipeData: ScriptableObject
    {
        public string outputItem;
        public int outputCount;
        [ColorUsage(false, true)]
        public Color productColor;

        public List<RecipeInput> inputs;
        public int waterRequirements;
    }

    [Serializable]
    public class RecipeInput
    {
        public string item;
        public int count;
    }
}