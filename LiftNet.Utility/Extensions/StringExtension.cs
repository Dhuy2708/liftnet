using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string? str)
        {
            return String.IsNullOrEmpty(str);
        }

        public static bool IsNotNullOrEmpty(this string? str)
        {
            return !String.IsNullOrEmpty(str);
        }

        public static bool Eq(this string? str, string strToCompare)
        {
            if (str == null || strToCompare == null)
            {
                return false;
            }
            return str.Equals(strToCompare, StringComparison.OrdinalIgnoreCase);
        }
    }
}
