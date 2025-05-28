using LiftNet.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Utils
{
    public static class UserUtil
    {
        public static UserAgeRange GetAgeRange(int age)
        {
            if (age < 0)
                return UserAgeRange.None;

            if (age < 18)
                return UserAgeRange.Below18;

            if (age <= 24)
                return UserAgeRange.From18To24;

            if (age <= 34)
                return UserAgeRange.From25To34;

            if (age <= 44)
                return UserAgeRange.From35To44;

            if (age <= 54)
                return UserAgeRange.From45To54;

            if (age <= 64)
                return UserAgeRange.From55To64;

            return UserAgeRange.Above65;
        }
    }
}
