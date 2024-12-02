using DG.Tweening;
using UnityEngine;

namespace Game.Sound
{
    public class MusicFadingChannel: CrossfadeChannel
    {
        public static MusicFadingChannel Instance { get; private set; }

        public override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        public override void QueueCrossfade(AudioClip toClip)
        {
            base.QueueCrossfade(toClip);
            Invoke(nameof(FadeOut), toClip.length - 3);
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