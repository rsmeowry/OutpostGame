using System.Collections.Generic;

namespace External.Util
{
    public static class Collections
    {
        public static void Increment<TKey>(this Dictionary<TKey, int> self, TKey key, int by = 1)
        {
            self[key] = self.TryGetValue(key, out var og) ? og + by : by;
        }
    }
}