using System;

namespace Vroksjab.Core.Extensions
{
    public static class Cloneable
    {
        public static T CloneAndTypecast<T>(this T original)
            where T : ICloneable
        {
            return (T)original.Clone();
        }

        public static T CloneAndTypecastNullSafe<T>(this T original)
            where T : class, ICloneable
        {
            if (original == null)
            {
                return null;
            }
            else
            {
                return (T)original.Clone();
            }
        }
    }
}
