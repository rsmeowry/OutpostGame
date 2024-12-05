using System;
using External.Util;
using Game.POI;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace External.Editor
{
    [CustomEditor(typeof(TextureBaker))]
    public class TextureBakerEditor: UnityEditor.Editor
    {
        private SerializedProperty _rt;
        
        public void OnEnable()
        {
            _rt = serializedObject.FindProperty("rt");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUI.Button(EditorGUILayout.GetControlRect(false, 25, GUIStyle.none), "Bake RT to file"))
            {
                var rt = (RenderTexture) _rt.objectReferenceValue;
                var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false, true);                
                tex.CopyPixels(rt);
                tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                tex.Apply();
                System.IO.File.WriteAllBytes("baked.png", tex.EncodeToPNG());
            }
        }
    }

}

#endif