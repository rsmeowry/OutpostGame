using System;
using System.Collections;
using DG.Tweening;
using External.Util;
using UnityEngine;

namespace Game.Sound
{
    public abstract class CrossfadeChannel: MonoBehaviour
    {
        private AudioSource _base;
        
        public virtual void Awake()
        {
            _base = GetComponent<AudioSource>();
            _base.loop = true;
        }

        private Coroutine _previousCrossfade;
        public void QueueCrossfade(AudioClip toClip)
        {
            _previousCrossfade = StartCoroutine(_previousCrossfade == null
                ? Crossfade(toClip)
                : Chained(_previousCrossfade, Crossfade(toClip)));
        }

        private static IEnumerator Chained(Coroutine first, IEnumerator then)
        {
            yield return first;
            yield return then;
        }

        private IEnumerator Crossfade(AudioClip toClip)
        {
            if (_base.clip == null)
            {
                _base.clip = toClip;
                _base.volume = 0f;
                yield return _base.DOFade(1f, 3f).Play().WaitForCompletion();
                yield break;
            }
            var fadeOutSource = gameObject.AddComponent<AudioSource>();
            fadeOutSource.clip = _base.clip;
            fadeOutSource.time = _base.time;
            fadeOutSource.volume = _base.volume;
            fadeOutSource.outputAudioMixerGroup = GetComponent<AudioSource>().outputAudioMixerGroup;

            //make it start playing
            fadeOutSource.Play();

            //set original audiosource volume and clip
            _base.volume = 0f;
            _base.clip = toClip;
            var t = 0;
            var v = fadeOutSource.volume;
            _base.Play();

            var seq = DOTween.Sequence();
            seq.Join(fadeOutSource.DOFade(0f, 3f));
            seq.Join(_base.DOFade(1f, 3f));
            yield return seq.Play().WaitForCompletion();
            Destroy(fadeOutSource);
        }
    }
}