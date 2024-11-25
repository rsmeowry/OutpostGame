using UnityEditor;
using UnityEngine;

namespace Game.POI
{
    [CreateAssetMenu(fileName = "PointOfInterest", menuName = "Outpost/POI Data")]
    public class POIData: ScriptableObject
    {
        public string keyId;
        public string title;
        [TextArea(minLines: 4, maxLines: 8)]
        public string description;
    }
}