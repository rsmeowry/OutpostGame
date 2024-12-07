using UnityEngine;

namespace Menu
{
    [ExecuteInEditMode]
    public class MainMenuLight: MonoBehaviour
    {
        private static readonly int SunDirection = Shader.PropertyToID("_SunDirection");

        public void Update()
        {
            Shader.SetGlobalVector(SunDirection, transform.forward);
        }
    }
}