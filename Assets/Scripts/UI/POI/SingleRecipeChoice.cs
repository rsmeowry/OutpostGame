using System;
using Game.Production;
using Game.Production.Products;
using Game.State;
using TMPro;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.POI
{
    public class SingleRecipeChoice: MonoBehaviour, IPointerClickHandler
    {
        [NonSerialized]
        public RecipeSelectorParent Parent;

        [NonSerialized]
        public RecipeData RecipeData;

        [SerializeField]
        private GameObject singleIngredientPrefab;

        [SerializeField]
        private Sprite waterIcon;

        [SerializeField]
        private GameObject selectedIcon;

        public void Start()
        {
            var container = transform.GetChild(0);
            // ingredients
            if (RecipeData.waterRequirements > 0)
            {
                var ing = Instantiate(singleIngredientPrefab, container);
                ing.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = waterIcon;
                ing.transform.GetChild(1).GetComponentInChildren<TMP_Text>().text = RecipeData.waterRequirements.ToString();
                var tt = ing.GetComponent<SimpleTooltipDisplay>();
                tt.title = "Вода: " + RecipeData.waterRequirements;
                tt.body = "Ингредиент (берется из системы жидкостей)";
                ing.transform.SetAsFirstSibling();
            }

            foreach (var ingredient in RecipeData.inputs)
            {
                var key = StateKey.FromString(ingredient.item);
                var data = ProductRegistry.Instance.GetProductData(key);
                var ing = Instantiate(singleIngredientPrefab, container);
                ing.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = data.icon;
                ing.transform.GetChild(1).GetComponentInChildren<TMP_Text>().text = ingredient.count.ToString();
                var tt = ing.GetComponent<SimpleTooltipDisplay>();
                tt.title = data.name + " x" + ingredient.count;
                tt.body = "Ингредиент";
                ing.transform.SetAsFirstSibling();
            }

            // result
            var oKey = StateKey.FromString(RecipeData.outputItem);
            var oData = ProductRegistry.Instance.GetProductData(oKey);
            var p = Instantiate(singleIngredientPrefab, container);
            p.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = oData.icon;
            p.transform.GetChild(1).GetComponentInChildren<TMP_Text>().text = RecipeData.outputCount.ToString();
            var tlt = p.GetComponent<SimpleTooltipDisplay>();
            tlt.title = oData.name + " x" + RecipeData.outputCount;
            tlt.body = "Результат";
            PollChanges();
        }

        public void PollChanges()
        {
            selectedIcon.SetActive(Parent.RecipeContainer.ActiveRecipe == RecipeData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Parent.RecipeContainer.ActiveRecipe = RecipeData;
            Parent.Poll();
        }
    }
}