﻿using UnityEditor;
using UnityEngine;

namespace Game.POI
{
    [CreateAssetMenu(fileName = "PointOfInterest", menuName = "Interest/POI Data")]
    public class POIData: ScriptableObject
    {
        public string title;
        [TextArea(minLines: 4, maxLines: 8)]
        public string description;
    }
}