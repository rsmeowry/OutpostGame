using DG.Tweening;
using External.Util;
using UnityEngine;

namespace Game.Sound
{
    public class MusicFadingChannel: CrossfadeChannel
    {
        public static MusicFadingChannel Instance { get; private set; }

        protected override float MaxVolume => 0.25f;

        public override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        public override void QueueCrossfade(AudioClip toClip)
        {
            base.QueueCrossfade(toClip);
            this.Delayed(toClip.length - 3, FadeOut, true);
        }

        public void FadeOut()
        {
            Base.DOFade(0f, 3f).OnComplete(() =>
            {
                Base.Stop();
                Base.clip = null;
            });
        }
    }
}