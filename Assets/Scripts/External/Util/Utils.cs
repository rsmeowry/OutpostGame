using System.Reflection;

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
    }
}