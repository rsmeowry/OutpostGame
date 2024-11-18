using Game.Citizens.Navigation;
using UnityEngine;

namespace Game.POI
{
    public abstract class PointOfInterest: MonoBehaviour
    {
        public AudioClip clickedSound;
        public POIData data;
        
        public virtual (float, float) SoundPitchRange => (0.7f, 1.3f);

        public float cameraZoomAmt = 30f;
        public bool shouldDepthOfField = false;
        
        public abstract QueuePosition EntrancePos { get; }
    }
}