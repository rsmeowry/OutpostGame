﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using External.Achievement;
using External.Util;
using Game.Building;
using Game.Citizens;
using Game.Electricity;
using Game.Production.Products;
using Game.Sound;
using Game.State;
using UI.POI;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Production.POI
{
    public class Smeltery: UtilityResourcePOI, IElectricityConsumer, IRecipeContainer
    {
        public List<RecipeData> possibleRecipes;

        public RecipeData activeRecipe;

        [SerializeField]
        private Light windowLight;
        [SerializeField]
        private VisualEffectAsset vfxAsset;
        [SerializeField]
        private Transform vfx;
        private VisualEffect _vfx;

        private Material _windowMat;

        private static readonly int LiquidColor = Shader.PropertyToID("_LiquidColor");

        public override void OnBuilt()
        {
            _windowMat = transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material;
            _windowMat.SetColor(LiquidColor, Color.black);
            _vfx = vfx.gameObject.AddComponent<VisualEffect>();
            _vfx.visualEffectAsset = vfxAsset;
            _vfx.Stop();
            
            ((IElectrical) this).InitElectricity(transform);
            
            AchievementManager.Instance.GiveAchievement(Achievements.Smeltery);
        }

        protected override void LoadForInspect(PanelViewPOI panel)
        {
            base.LoadForInspect(panel);
            panel.AddElectricityConsumption();
            panel.AddRecipeSelector();
        }

        private Tween _liquidTween;
        private bool _startedPlaying;
        public override void WorkerTick(CitizenAgent agent)
        {
            if (!ShouldSubtick(agent, 4))
            {
                return;
            }

            if (!((IElectricityConsumer)this).IsConnectedAndWorking())
            {
                _vfx.Stop();
                return;
            }

            if (activeRecipe == null)
            {
                _vfx.Stop();
                return;
            }

            if (!HasAllIngredients())
            {
                _vfx.Stop();
                return;
            }
            
            _vfx.Play();
            
            // sound
            if (!_startedPlaying && GetTick(agent) % 8 == 0)
            {
                _startedPlaying = true;
                SoundManager.Instance.PlaySoundAt(SoundBank.Instance.GetSound("building.smeltery"), transform.position, 0.7f);
                this.Delayed(8f, () => { _startedPlaying = false; }, true);
            }
            
            foreach (var r in activeRecipe.inputs)
            {
                var iKey = StateKey.FromString(r.item);
                GameStateManager.Instance.PlayerProductCount[iKey.Formatted()] -= r.count;
            }

            GameStateManager.Instance.ChangeFluids(ProductRegistry.Water, -activeRecipe.waterRequirements);

            var amount = ApplyProductivityBonus(activeRecipe.outputCount, Upgrades.Upgrades.MetallurgyProductivity);
            agent.Inventory.Increment(StateKey.FromString(activeRecipe.outputItem), amount);

            var seq = DOTween.Sequence();
            seq.Join(windowLight.DOColor(activeRecipe.productColor, 1f));
            seq.Join(windowLight.DOIntensity(10f, 1f));
            seq.Join(_windowMat.DOColor(activeRecipe.productColor, LiquidColor, 1f));
            seq.Insert(3f, _windowMat.DOColor(Color.black, LiquidColor, 1f));
            seq.Insert(3f, windowLight.DOIntensity(0f, 3f));

            _liquidTween?.Kill();
            _liquidTween = seq.Play();
        }
        
        public override IEnumerator LeaveWorkPlace(CitizenAgent agent)
        {
            yield return base.LeaveWorkPlace(agent);
            if(CitizensInside.Count <= 0)
                _vfx.Stop();
        }


        private bool HasAllIngredients()
        {
            if (GameStateManager.Instance.FluidCount.GetValueOrDefault(ProductRegistry.Water, 0) < activeRecipe.waterRequirements)
                return false;
            
            foreach (var r in activeRecipe.inputs)
            {
                var iKey = StateKey.FromString(r.item);
                var count = GameStateManager.Instance.PlayerProductCount.GetValueOrDefault(iKey.Formatted(), 0);
                if (count < r.count)
                    return false;
            }
            
            return true;
        }

        public bool IsCovered { get; set; }
        public float MaxConsumption => 12_000;
        public List<RecipeData> PossibleRecipes => possibleRecipes;
        public RecipeData ActiveRecipe
        {
            get => activeRecipe;
            set => activeRecipe = value;
        }
    }
}