using System;
using System.Collections;
using DG.Tweening;
using External.Util;
using UnityEngine;

namespace Game.Sound
{
    public abstract class CrossfadeChannel: MonoBehaviour
    {
        protected AudioSource Base;
        
        public virtual void Awake()
        {
            Base = GetComponent<AudioSource>();
            Base.loop = true;
        }

        private Coroutine _previousCrossfade;
        public virtual void QueueCrossfade(AudioClip toClip)
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
            if (Base.clip == null)
            {
                Base.clip = toClip;
                Base.volume = 0f;
                Base.Play();
                yield return Base.DOFade(1f, 3f).Play().WaitForCompletion();
                yield break;
            }
            var fadeOutSource = gameObject.AddComponent<AudioSource>();
            fadeOutSource.clip = Base.clip;
            fadeOutSource.time = Base.time;
            fadeOutSource.volume = Base.volume;
            fadeOutSource.outputAudioMixerGroup = GetComponent<AudioSource>().outputAudioMixerGroup;

            //make it start playing
            fadeOutSource.Play();

            //set original audiosource volume and clip
            Base.volume = 0f;
            Base.clip = toClip;
            var t = 0;
            var v = fadeOutSource.volume;
            Base.Play();

            var seq = DOTween.Sequence();
            seq.Join(fadeOutSource.DOFade(0f, 3f));
            seq.Join(Base.DOFade(1f, 3f));
            yield return seq.Play().WaitForCompletion();
            Destroy(fadeOutSource);
        }
    }
}