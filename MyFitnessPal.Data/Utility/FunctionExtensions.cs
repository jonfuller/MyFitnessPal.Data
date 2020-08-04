using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFitnessPal.Data.Utility
{
    public static class FunctionExtensions
    {
        public static T2 Then<T1, T2>(this T1 x, Func<T1, T2> f)
        {
            return f(x);
        }
        public static IEnumerable<T2> ThenMap<T1, T2>(this IEnumerable<T1> x, Func<T1, T2> f)
        {
            return x.Select(f);
        }

        public static T Then<T>(this T x, Action<T> f)
        {
            f(x);
            return x;
        }
    }
}