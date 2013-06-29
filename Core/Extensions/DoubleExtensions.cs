using System;

namespace Vroksjab.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static bool IsFinite(this double d)
        {
            return !(double.IsNaN(d) && !double.IsInfinity(d));
        }

        public static bool IsFinite(this Nullable<double> d)
        {
            return d.HasValue && d.Value.IsFinite();
        }

        public static Nullable<double> NonFiniteToNull(this double d)
        {
            return d.IsFinite() ? (Nullable<double>)d : null;
        }

        public static Nullable<double> NonFiniteToNull(this Nullable<double> d)
        {
            return (d.HasValue && d.Value.IsFinite()) ? d : null;
        }

        public static double NullToNan(Nullable<double> d)
        {
            return d.HasValue ? d.Value : double.NaN;
        }

        public static double Clamp(this double d, double min, double max)
        {
            return Math.Min(Math.Max(d, min), max);
        }
    }
}
