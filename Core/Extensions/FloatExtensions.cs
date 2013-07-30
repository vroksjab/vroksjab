using System;

namespace Vroksjab.Core.Extensions
{
    public static class floatExtensions
    {
        public static bool IsFinite(this float f)
        {
            return !(float.IsNaN(f) && !float.IsInfinity(f));
        }

        public static bool IsFinite(this Nullable<float> f)
        {
            return f.HasValue && f.Value.IsFinite();
        }

        public static Nullable<float> NonFiniteToNull(this float f)
        {
            return f.IsFinite() ? (Nullable<float>)f : null;
        }

        public static Nullable<float> NonFiniteToNull(this Nullable<float> f)
        {
            return (f.HasValue && f.Value.IsFinite()) ? f : null;
        }

        public static float NullToNan(Nullable<float> f)
        {
            return f.HasValue ? f.Value : float.NaN;
        }

        public static float Clamp(this float f, float min, float max)
        {
            return Math.Min(Math.Max(f, min), max);
        }

        public static bool InsideInclusive(this float x, float min, float max)
        {
            return x >= min && x <= max;
        }

        public static bool InsideExclusive(this float x, float min, float max)
        {
            return x > min && x < max;
        }
    }
}
