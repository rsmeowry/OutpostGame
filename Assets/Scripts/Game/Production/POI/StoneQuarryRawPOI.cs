using System.Collections.Generic;
using External.Util;
using Game.Citizens;
using Game.Production.Products;
using UI.POI;
using UnityEngine.InputSystem;

namespace Game.Production.POI
{
    public class StoneQuarryRawPOI: UtilityResourcePOI
    {
        private Dictionary<CitizenAgent, int> _ticks;
        public override void WorkerTick(CitizenAgent agent)
        {
            if (ShouldSubtick(agent, 3))
            {
                agent.Inventory.Increment(ProductRegistry.Stone); 
            }
        }

        protected override void LoadForInspect(PanelViewPOI panel)
        {
            var productData = ProductRegistry.Instance.GetProductData(ProductRegistry.Stone);
            panel.AddResourceProduction(productData.name, productData.icon, "1шт/3с");
            base.LoadForInspect(panel);
        }
    }
}