namespace PokeSharp.Core.Utils;

public static class MathExtensions
{
    extension(Math)
    {
        public static float Lerp(float startVal, float endVal, float duration, float delta, float? now = null)
        {
            if (duration <= 0)
                return endVal;

            if (now.HasValue)
                delta = now.Value - delta;

            if (delta <= 0)
                return startVal;

            if (delta >= duration)
                return endVal;

            return startVal + (endVal - startVal) * delta / duration;
        }
    }
}
