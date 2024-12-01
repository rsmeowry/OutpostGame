namespace External.Util
{
    public class Mathu
    {
        public static int BSign(bool v)
        {
            return v ? 1 : -1;
        }

        public static float Remap(float v, float iMin, float iMax, float oMin, float oMax)
        {
            return oMin + (v - iMin) * (oMax - oMin) / (iMax - iMin);
        }
    }
}