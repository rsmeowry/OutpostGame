using Game.State;

namespace Game.Upgrades
{
    public static class Upgrades
    {
        // BASE
        public static readonly StateKey HarderTasks = new("harder_tasks");
        
        // TOP RIGHT BRANCH
        public static readonly StateKey AnotherHireSlot = new("hire_slot");
        public static readonly StateKey IncreasedExperience = new("increase_exp");
        
        public static readonly StateKey VolatileMarket = new("volatile_market");
        public static readonly StateKey ExpFromSelling = new("exp_from_selling");
        
        // TOP LEFT BRANCH
        public static readonly StateKey BasicElectricity = new("basic_electricity");
        
        public static readonly StateKey Metallurgy = new("metallurgy");
        public static readonly StateKey MetallurgyProductivity = new("metallurgy_productivity");

        public static readonly StateKey FluidHandling = new("fluid_handling");
        public static readonly StateKey HydroElectricity = new("hydro_electricity");
        public static readonly StateKey Chemistry = new("chemistry");
        public static readonly StateKey ChemistryProductivity = new("chemistry_productivity");
        
        // BOTTOM BRANCH
        public static readonly StateKey Utility = new("utility");
        
        public static readonly StateKey EnergyEfficiency = new("energy_efficiency");
        
        public static readonly StateKey Geology = new("geology");
        
        public static readonly StateKey Forestry = new("forestry");

        public static readonly StateKey ChoppingEfficiency = new("chopping_efficiency");

        public static readonly StateKey Deco = new("deco");

        public static readonly StateKey Enlightenment = new("enlightenment");
        
        public static readonly StateKey DeeperPockets = new("deeper_pockets");

    }
}