using System.Collections;
using System.Collections.Generic;
using External.Util;
using Game.Citizens;
using Game.State;
using Game.Upgrades;
using UnityEngine;

namespace Game.Production.POI
{
    public abstract class UtilityResourcePOI: ResourceContainingPOI
    {
        private Dictionary<int, int> _ticks = new();

        public override string PoiDesc => data.description;

        protected int ApplyProductivityBonus(int baseAmount, StateKey upgradeKey)
        {
            var amount = UpgradeTreeManager.Instance.Upgrades.GetValueOrDefault(upgradeKey, 0) * 25;
            var modifier = amount / 100f;
            var guaranteed = baseAmount * Mathf.FloorToInt(modifier);
            var additionalBonus = Random.Range(0f, 1f) < modifier ? baseAmount : 0;
            return baseAmount + guaranteed + additionalBonus;
        }

        protected bool ShouldSubtick(CitizenAgent agent, int subtickCount)
        {
            var t = _ticks.Increment(agent.citizenId);
            if (t % subtickCount == 0)
            {
                return true;
            }

            return false;
        }

        protected int GetTick(CitizenAgent agent)
        {
            return _ticks.GetValueOrDefault(agent.citizenId);
        }

        public override IEnumerator LeaveWorkPlace(CitizenAgent agent)
        {
            yield return base.LeaveWorkPlace(agent);
            _ticks[agent.citizenId] = 0;
        }
    }
}