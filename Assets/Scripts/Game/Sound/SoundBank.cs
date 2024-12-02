using System;
using System.Collections.Generic;
using System.Linq;
using External.Util;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Sound
{
    public class SoundBank: MonoBehaviour
    {
        public static SoundBank Instance { get; private set; }
        
        private Dictionary<string, ISoundInfo> _clips;

        [SerializeField]
        private List<SingleSoundInfo> sounds;
        [SerializeField]
        private List<VariableSoundInfo> variableSounds;
        
        public void Awake()
        {
            _clips = sounds.ToDictionary(it => it.soundId, it => (ISoundInfo) it);
            foreach (var variable in variableSounds)
            {
                _clips[variable.soundId] = variable;
            }
            Instance = this;
        }

        public AudioResource GetSound(string id)
        {
            return _clips.GetValueOrDefault(id, null)?.GetClip();
        }
    }

    public interface ISoundInfo
    {
        AudioResource GetClip();
    }
    
    [Serializable]
    public class SingleSoundInfo: ISoundInfo
    {
        public string soundId;
        public AudioResource clip;
        public AudioResource GetClip()
        {
            return clip;
        }
    }

    [Serializable]
    public class VariableSoundInfo: ISoundInfo
    {
        public string soundId;
        public AudioClip[] clips;
        
        public AudioResource GetClip()
        {
            return Rng.Choice(clips);
        }
    }
}