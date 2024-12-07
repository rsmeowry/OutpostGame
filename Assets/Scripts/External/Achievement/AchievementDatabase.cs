using System;
using System.Collections.Generic;
using UnityEngine;

namespace External.Achievement
{
    [CreateAssetMenu(menuName = "Outpost/AchievementDB", fileName = "AchievementDatabase")]
    public class AchievementDatabase: ScriptableObject
    {
        public List<SingleAchievementData> achievements;
    }

    [Serializable]
    public class SingleAchievementData
    {
        public string id;
        public string name;
        public string description;
        public Sprite icon;
    }
}