using Game.POI;
using UnityEngine;

namespace External.Util
{
    public static class Vectors
    {
        public static Vector3 RemapXYToXZ(Vector3 source)
        {
            return Quaternion.AngleAxis(-90, Vector3.right) * source;
        }

        public static string Formatted(this Vector3 self)
        {
            return $"({self.x} {self.y} {self.z})";
        }

        // CHECKS ONLY X AND Z
        public static bool LessThan(this Vector3Int self, Vector3Int other)
        {
            return self.x < other.x && self.z < other.z;
        }

        public static bool GreaterThan(this Vector3Int self, Vector3Int other)
        {
            return self.x > other.x && self.z > other.z;
        }

        public static Vector3 WithY(this Vector3 self, float y)
        {
            return new Vector3(self.x, y, self.z);
        }

        public static SerVec3 Ser(this Vector3 self)
        {
            return new SerVec3
            {
                X = self.x,
                Y = self.y,
                Z = self.z
            };
        }
    }
}