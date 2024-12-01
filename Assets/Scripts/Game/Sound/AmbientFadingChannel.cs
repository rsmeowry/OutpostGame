namespace Game.Sound
{
    public class AmbientFadingChannel: CrossfadeChannel
    {
        public static AmbientFadingChannel Instance { get; private set; }

        public override void Awake()
        {
            base.Awake();
            Instance = this;
        }
    }
}