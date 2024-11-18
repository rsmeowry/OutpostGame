using UI.POI;
using UnityEngine;

namespace UI {
    public class UIManager: MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public InspectPanelPOI prefabInspectPoi;

        public void Awake()
        {
            Instance = this;
        }
    }
}

