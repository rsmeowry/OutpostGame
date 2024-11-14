namespace External.Util
{
    public class CoroutineOutput<T>
    {
        public T Value { get; private set; }
        public bool Done { get; private set; }

        public void End(T value)
        {
            Value = value;
            Done = true;
        }
    }
}