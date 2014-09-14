namespace Silentor.TB.Server.Tools
{
    public static class Angles
    {
        static public float Wrap(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }
    }
}
