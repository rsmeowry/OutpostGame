using UnityEngine;
using UnityEngine.Audio;

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

        public void PlaySound2D(AudioResource sound, float volume, float pitch = 1f)
        {
            _executor.PlaySound2D((AudioClip) sound, volume, pitch);
        }

        public void PlaySoundAt(AudioResource sound, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if(sound is AudioClip clp)
                _executor.PlaySoundAt(clp, position, volume, pitch);
            else
                _executor.PlayResourceAt(sound, position);
        }
    }
}