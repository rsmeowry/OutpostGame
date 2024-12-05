using System.Collections.Generic;

namespace Game.Production.POI
{
    public interface IRecipeContainer
    {
        public List<RecipeData> PossibleRecipes { get; }

        public RecipeData ActiveRecipe { get; set; }
    }
}