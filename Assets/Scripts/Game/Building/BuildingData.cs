using System;
using System.Collections.Generic;
using System.Linq;
using Game.State;
using UnityEngine;

namespace Game.Building
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Outpost/Building Data")]
    public class BuildingData: ScriptableObject
    {
        public string keyId;
        public string name;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        public GameObject buildingPrefab;
        public Vector2Int size;
        public Color prominentColor;
        public bool requiresWater;

        public List<ResourceRequirement> requirements;
        
        public StateKey Id => StateKey.FromString(keyId);
    }

    [Serializable]
    public class ResourceRequirement
    {
        public string key;
        public int count;

        public static Dictionary<StateKey, int> ToDict(List<ResourceRequirement> reqs)
        {
            return reqs.Select(it => (StateKey.FromString(it.key), it.count))
                .ToDictionary(it => it.Item1, it => it.count);
        }
    }
}