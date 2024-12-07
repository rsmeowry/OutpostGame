using External.Util;
using Game.Citizens;
using Game.Production.Products;
using UI.POI;

namespace Game.Production.POI
{
    public class HiveAreaPOI: UtilityResourcePOI
    {
        public override void WorkerTick(CitizenAgent agent)
        {
            if (ShouldSubtick(agent, 3))
            {
                var amount = ApplyProductivityBonus(1, Upgrades.Upgrades.Forestry);
                agent.Inventory.Increment(ProductRegistry.Honey, amount);
            }
            
            // SoundManager.Instance.PlaySoundAt(SoundBank.Instance.GetSound("citizen.axe"), agent.transform.position);
        }

        protected override void LoadForInspect(PanelViewPOI panel)
        {
            var productData = ProductRegistry.Instance.GetProductData(ProductRegistry.Honey);
            panel.AddResourceProduction(productData.name, productData.icon, "1шт/3с");
            base.LoadForInspect(panel);
        }
    }
}