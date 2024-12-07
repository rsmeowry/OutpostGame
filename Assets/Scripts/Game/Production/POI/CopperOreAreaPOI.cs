using System.Collections.Generic;
using External.Util;
using Game.Citizens;
using Game.Production.Products;
using Game.Sound;
using UI.POI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Production.POI
{
    public class CopperOreAreaPOI : UtilityResourcePOI
    {
        public override void WorkerTick(CitizenAgent agent)
        {
            if (ShouldSubtick(agent, 4))
            {
                var amount = ApplyProductivityBonus(1, Upgrades.Upgrades.Geology);
                agent.Inventory.Increment(ProductRegistry.CopperOre, amount);
            }

            SoundManager.Instance.PlaySoundAt(SoundBank.Instance.GetSound("citizen.pickaxe"), agent.transform.position,
                0.1f, Random.Range(0.8f, 1.2f));
        }

        protected override void LoadForInspect(PanelViewPOI panel)
        {
            var productData = ProductRegistry.Instance.GetProductData(ProductRegistry.CopperOre);
            panel.AddResourceProduction(productData.name, productData.icon, "1шт/4с");
            base.LoadForInspect(panel);
        }
    }
}