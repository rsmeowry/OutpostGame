using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace External.Util
{
    public struct LoggerHandle
    {
        public const bool EnableLogs = true;
        
        public string Prefix { get; private set; }
#pragma warning disable CS0162 // Unreachable code detected
        public void Log(string value)
        {
            if (!EnableLogs)
                return;
            Debug.Log($"[{Prefix}] :: {value}");
        }

        public void Warn(string value)
        {
            if (!EnableLogs)
                return;
            Debug.Log($"[{Prefix}] :: <#ff0>{value}");
        }

        // ReSharper disable once PureAttributeOnVoidMethod
        [Pure]
        public void Dbg(params object[] objects)
        {
            if (!EnableLogs)
                return;
            Debug.Log($"[{Prefix}] :: {objects.Select(it => it == null ? "<NULL>" : it.ToString()).ToSeparatedString(", ")}");
        }
#pragma warning restore CS0162 // Unreachable code detected

        public static LoggerHandle LogHandle(object self)
        {
            return new LoggerHandle { Prefix = self.GetType().Name };
        }
        
        public static LoggerHandle LogHandle<T>()
        {
            return new LoggerHandle { Prefix = typeof(T).Name };
        }
    }
}