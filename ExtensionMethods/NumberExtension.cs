using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    public static class NumberExtension
    {
        public static bool IsBetweenExclusive<T>(this T actual, T lower, T upper) where T: IComparable<T>
        {
            return actual.CompareTo(lower) > 0 && actual.CompareTo(upper) < 0;
        }

        public static bool IsBetweenInclusive<T> (this T actual, T lower, T upper) where T: IComparable<T>
        {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) <= 0;
        }

    }
}
