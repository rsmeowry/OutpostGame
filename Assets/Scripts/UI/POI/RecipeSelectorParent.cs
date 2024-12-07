using System;
using System.Collections.Generic;
using Game.Production.POI;
using UnityEngine;

namespace UI.POI
{
    public class RecipeSelectorParent: MonoBehaviour
    {
        public IRecipeContainer RecipeContainer;

        [SerializeField]
        private SingleRecipeChoice singlePrefab;

        private List<SingleRecipeChoice> _children = new();

        public void Start()
        {
            var choiceContainer = transform;
            foreach (var rec in RecipeContainer.PossibleRecipes)
            {
                var single = Instantiate(singlePrefab, choiceContainer);
                single.Parent = this;
                single.RecipeData = rec;
                _children.Add(single);
            }
        }

        public void Poll()
        {
            foreach(var child in _children)
                child.PollChanges();
        }
    }
}