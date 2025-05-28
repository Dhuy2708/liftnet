using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Extensions
{
    public static class IEnumerableExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            return source == null || !source.Any();
        }
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            return !source.IsNullOrEmpty();
        }
    }
}
