public static class MathExtensions
{
    public static float remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (toMax - toMin) * ((value - fromMin) / (fromMax - fromMin));
    }
}
