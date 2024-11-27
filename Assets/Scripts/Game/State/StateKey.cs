using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Game.State
{
    // Used to manage various product types in game
    public readonly struct StateKey: IEquatable<StateKey>, IComparable<StateKey>, IEqualityComparer<StateKey>
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

        public bool Equals(StateKey other)
        {
            return Namespace == other.Namespace && Path == other.Path;
        }

        public override bool Equals(object obj)
        {
            return obj is StateKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Namespace, Path);
        }

        public int CompareTo(StateKey other)
        {
            var namespaceComparison = string.Compare(Namespace, other.Namespace, StringComparison.Ordinal);
            if (namespaceComparison != 0) return namespaceComparison;
            return string.Compare(Path, other.Path, StringComparison.Ordinal);
        }

        public bool Equals(StateKey x, StateKey y)
        {
            return x.Namespace == y.Namespace && x.Path == y.Path;
        }

        public int GetHashCode(StateKey obj)
        {
            return HashCode.Combine(obj.Namespace, obj.Path);
        }

        public static bool operator ==(StateKey lhs, StateKey rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(StateKey lhs, StateKey rhs)
        {
            return !(lhs == rhs);
        }
    }
}