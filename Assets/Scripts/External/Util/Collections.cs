using System.Collections.Generic;

namespace External.Util
{
    public static class Collections
    {
        public static void Increment<TKey>(this Dictionary<TKey, int> self, TKey key, int by = 1)
        {
            self[key] = self.TryGetValue(key, out var og) ? og + by : by;
        }

        public static string ToLineSeparatedString(this IEnumerable<string> strings)
        {
            return string.Join("\n", strings);
        }

        public static string ToCommaSeparatedString(this IEnumerable<string> strings)
        {
            return string.Join(",", strings);
        }
    }
}