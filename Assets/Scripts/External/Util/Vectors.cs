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
    }
}