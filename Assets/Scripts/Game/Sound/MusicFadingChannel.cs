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
    }
}