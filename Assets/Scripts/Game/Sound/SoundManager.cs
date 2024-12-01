using UnityEngine;

namespace Game.Sound
{
    public class SoundManager: MonoBehaviour
    {
        [SerializeField]
        private AudioSource asProto;

        private IAudioExecutor _executor;
        
        public static SoundManager Instance { get; private set; }
        
        public void Awake()
        {
            Instance = this;
            _executor = GetComponent<IAudioExecutor>();
        }

        public AudioSource AddAudioSource(Transform obj)
        {
            var o = Instantiate(asProto, obj);
            o.playOnAwake = false;
            o.loop = false;
            o.enabled = true;
            return o;
        }

        public void PlaySound2D(AudioClip sound, float volume, float pitch = 1f)
        {
            _executor.PlaySound2D(sound, volume, pitch);
        }

        public void PlaySoundAt(AudioClip sound, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            _executor.PlaySoundAt(sound, position, volume, pitch);
        }
    }
}