using System.Runtime.CompilerServices;

namespace Game.State
{
    // Used to manage various product types in game
    public readonly struct StateKey
    {
        public readonly string Namespace;
        public readonly string Path;

        public StateKey(string path)
        {
            Namespace = "outpost";
            Path = path;
        }

        public StateKey(string ns, string path)
        {
            Namespace = ns;
            Path = path;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Formatted()
        {
            return $"{Namespace}:{Path}";
        }
        
        public static StateKey FromString(string st)
        {
            if (!st.Contains(":"))
                return new StateKey(st);
            
            var sp = st.Split(":");
            return new StateKey(sp[0], sp[1]);
        }
    }
}