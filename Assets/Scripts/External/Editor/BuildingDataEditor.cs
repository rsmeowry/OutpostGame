#if UNITY_EDITOR
using External.Util;
using Game.Building;
using UnityEditor;
using UnityEngine;

namespace External.Editor
{
    [CustomEditor(typeof(BuildingData))]
    public class BuildingDataEditor: UnityEditor.Editor
    {
        private SerializedProperty _buildingPrefab;
        private SerializedProperty _prominentColor;
        
        public void OnEnable()
        {
            _buildingPrefab = serializedObject.FindProperty("buildingPrefab");
            _prominentColor = serializedObject.FindProperty("prominentColor");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUI.Button(EditorGUILayout.GetControlRect(false, 25, GUIStyle.none), "Bake prominent colors"))
            {
                var prefab = (GameObject) _buildingPrefab.GetValue();
                var rd = prefab.transform.GetComponentInChildren<Renderer>();
                var main = rd.sharedMaterial.mainTexture;
                var colors = ProminentColor.GetColors32FromImage((Texture2D) main, 1, 75f, 2, 1f);
                _prominentColor.colorValue = colors[0];
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif