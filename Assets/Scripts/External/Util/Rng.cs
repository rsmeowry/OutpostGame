using System.Collections.Generic;
using UnityEngine;

namespace External.Util
{
    public static class Rng
    {
        public static bool Bool()
        {
            return Random.Range(0f, 1f) < 0.5f;
        }

        public static T Choice<T>(T[] values)
        {
            return values[Random.Range(0, values.Length)];
        }
        
        public static T Choice<T>(List<T> values)
        {
            return values[Random.Range(0, values.Count)];
        }

    }
}