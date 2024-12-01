using UnityEngine;

namespace Game.Sound
{
    public interface IAudioExecutor
    {
        public void PlaySound2D(AudioClip sound, float volume, float pitch);
        public void PlaySound2D(AudioClip sound, float volume);

        public void PlaySoundAt(AudioClip sound, Vector3 position, float volume = 1f, float pitch = 1f);
    }
}