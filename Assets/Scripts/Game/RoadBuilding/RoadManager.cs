using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.RoadBuilding
{
    public class RoadManager: MonoBehaviour
    {
        public static RoadManager Instance { get; private set; }

        private List<RoadIntersection> _roadPoints;

        private void Awake()
        {
            Instance = this;
        }

        public void PlaceIntersection(float x, float z)
        {
            // 0 0 - is our middle point intersection
            // so 1 0 will be on tile to the right
            // like this
            //  _ _ _ _ _ 
            // |_|_|_|_|_| 
            // |_|_|_|_|_| 
            // |_|_|_|_|_| 
            // |_|_|_|_|_| 
            // |_|_|_|_|_| 
            //
            
        }
    }
}