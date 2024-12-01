﻿using System.Collections.Generic;
using System.Linq;
using External.Util;
using UnityEngine;

namespace Game.Sound
{
    public class PooledAudioExecutor: MonoBehaviour, IAudioExecutor
    {
        [SerializeField]
        private int poolSize;
        [SerializeField]
        private float audioCutoff;
        private AudioSource[] _audioPool;
        private List<int> _free = new();
        private float _audioCutoffSqr;

        public void Start()
        {
            _audioCutoffSqr = audioCutoff * audioCutoff;
            _audioPool = new AudioSource[poolSize];
            for (var i = 0; i < poolSize; i++)
            {
                _audioPool[i] = gameObject.AddComponent<AudioSource>();
            }

            _free = Enumerable.Range(0, poolSize).ToList();
        }

        private (AudioSource, int) GetNextFree()
        {
            var idx = _free.FirstOrDefault();
            return (_audioPool[_free.FirstOrDefault()], idx);
        }
        
        public void PlaySound2D(AudioClip sound, float volume, float pitch)
        {
            var nxt = GetNextFree();
            _free.Remove(nxt.Item2);
            var next = nxt.Item1;
            
            next.Stop();
            next.clip = sound;
            next.volume = volume; // TODO: preferences setting volume
            next.pitch = pitch;
            
            next.Play();

            StartCoroutine(new WaitUntil(() => !next.isPlaying).Callback(() => _free.Add(nxt.Item2)));
        }

        public void PlaySound2D(AudioClip sound, float volume)
        {
            PlaySound2D(sound, volume, 1f);
        }

        public void PlaySoundAt(AudioClip sound, Vector3 position, float volume = 1, float pitch = 1)
        {
            var listener = Camera.main!.transform.position;
            if ((listener - position).sqrMagnitude > _audioCutoffSqr)
                return;

            var source = SoundManager.Instance.AddAudioSource(transform);
            source.transform.position = position;
            source.volume = volume;
            source.pitch = pitch;
            source.clip = sound;
            source.Play();
            StartCoroutine(new WaitUntil(() => !source.isPlaying).Callback(() => Destroy(source.gameObject)));
        }
    }
}