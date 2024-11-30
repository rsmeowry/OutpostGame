using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Upgrades
{
    [CreateAssetMenu(menuName = "Outpost/Upgrade", fileName = "UpgradeData")]
    public class UpgradeData: ScriptableObject
    {
        public string id;
        public string title;
        public string description;
        public int baseExperienceCost;
        public bool isInfinite;
        public List<UpgradeData> unlocks;
        public UpgradeGroup group;
    }

    public enum UpgradeGroup
    {
        TopRight,
        TopLeft,
        Bottom,
        Other
    }
}