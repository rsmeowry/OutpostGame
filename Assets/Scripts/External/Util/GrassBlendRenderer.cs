using System;
using External.Data;
using Game.Production.POI;
using Inside;
using UnityEngine;
using UnityEngine.EventSystems;

namespace External.Util
{
    [RequireComponent(typeof(Camera))]
    public class GrassBlendRenderer : MonoBehaviour {
        public int fps = 20;
        private float _elapsed;
        private Camera _cam;

        private void Start () {
            _cam = GetComponent<Camera>();
            _cam.enabled = false;
            var level = Math.Clamp(Preferences.Instance.Prefs.GrassBlendLevel + 5, 6, 10);
            var textureResolution = (int) Mathf.Pow(2, level);
            _cam.targetTexture.width = textureResolution;
            _cam.targetTexture.height = textureResolution;
        }

        private void Update () {
            _elapsed += Time.deltaTime;
            if (!(_elapsed > 1f / fps)) return;
            
            _elapsed = 0;
            _cam.Render();
        }
    }
}