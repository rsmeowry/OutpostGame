using System.Collections.Generic;
using UnityEngine;

namespace Game.Building
{
    [CreateAssetMenu(fileName = "BuildingDatabase", menuName = "Outpost/Building Database")]
    public class BuildingDatabase : ScriptableObject
    {
        public List<BuildingData> buildings;
    }

}