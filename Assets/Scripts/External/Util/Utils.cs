using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace External.Util
{
    public static class Utils
    {
        #if UNITY_EDITOR
        public static object GetValue(this UnityEditor.SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;

            FieldInfo field = null;
            foreach( var path in property.propertyPath.Split( '.' ) )
            {
                var type = obj.GetType();
                field = type.GetField( path );
                obj = field.GetValue( obj );
            }
            return obj;
        }
        #endif

        public static IEnumerable<Transform> EnumerateChildren(this Transform tf)
        {
            for (var i = 0; i < tf.childCount; i++)
            {
                yield return tf.GetChild(i);
            }
        }
    }
}