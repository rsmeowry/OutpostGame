using UnityEngine;

namespace Game.Sound
{
    public class SoundManager: MonoBehaviour
    {
        private AudioSource _asProto;
        
        public static SoundManager Instance { get; private set; }
        
        public void Awake()
        {
            Instance = this;
            _asProto = transform.GetComponentInChildren<AudioSource>();
        }

        public AudioSource AddAudioSource(Transform obj)
        {
            var o = Instantiate(_asProto, obj);
            o.enabled = true;
            return o;
        }
    }
}