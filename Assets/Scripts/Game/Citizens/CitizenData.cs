using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Citizens
{
    public class CitizenData
    {
        public string Name;
        public CitizenCaste Profession;
        public List<string> Awards;
    }

    public enum CitizenCaste
    {
        Creator,
        Explorer,
        Beekeeper,
        Engineer,
    }
    
    public struct SerializedCitizenData
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("position")]
        public Vector3 Position;
        [JsonProperty("inventory")]
        public Dictionary<string, int> Inventory;
        [JsonProperty("profession")]
        public CitizenCaste Caste;
        [JsonProperty("awards")]
        public List<string> Awards;
    }
}