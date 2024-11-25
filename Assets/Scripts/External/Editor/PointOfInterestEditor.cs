#if UNITY_EDITOR
using System;
using Game.POI;
using UnityEditor;
using UnityEngine;

namespace External.Editor
{
    [CustomEditor(typeof(PointOfInterest))]
    public class PointOfInterestEditor: UnityEditor.Editor
    {
        private SerializedProperty _pointId;
        
        public void OnEnable()
        {
            _pointId = serializedObject.FindProperty("pointId");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUI.Button(EditorGUILayout.GetControlRect(false, 25, GUIStyle.none), "Bake POI GUID"))
            {
                _pointId.stringValue = Guid.NewGuid().ToString();
            }
        }
    }
}
#endif