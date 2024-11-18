using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Citizens
{
    public class PersistentCitizenData
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
    
    [Serializable]
    public struct StoredPersistentCitizenData
    {
        [FormerlySerializedAs("Name")] [JsonProperty("name")]
        public string name;
        [FormerlySerializedAs("Caste")] [JsonProperty("profession")]
        public CitizenCaste caste;
        [FormerlySerializedAs("Awards")] [JsonProperty("awards")]
        public List<string> awards;
    }
}