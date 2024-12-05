using System.Collections.Generic;
using External.Util;
using Game.Citizens;
using Game.Citizens.States;
using Game.Production.Products;
using Game.Sound;
using UI.POI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Production.POI
{
    public class TreeAreaPoi: UtilityResourcePOI
    {
        public override void WorkerTick(CitizenAgent agent)
        {
            if (ShouldSubtick(agent, 2))
            {
                agent.Inventory.Increment(ProductRegistry.Wood); 
            }
            SoundManager.Instance.PlaySoundAt(SoundBank.Instance.GetSound("citizen.axe"), agent.transform.position);
        }

        protected override void LoadForInspect(PanelViewPOI panel)
        {
            var productData = ProductRegistry.Instance.GetProductData(ProductRegistry.Wood);
            panel.AddResourceProduction(productData.name, productData.icon, "1шт/2с");
            base.LoadForInspect(panel);
        }
    }
}