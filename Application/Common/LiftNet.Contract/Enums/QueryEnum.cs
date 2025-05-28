using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Enums
{
    public enum QueryOperator
    {
        Equal = 0,
        NotEqual = 1,
        GreaterThan = 2,
        LessThan = 3,
        GreaterThanOrEqual = 4,
        LessThanOrEqual = 5,
        Contains = 6,
        StartsWith = 7,
        NotContains = 8,
    }

    public enum QueryLogic
    {
        None = 0,
        And = 1,
        Or = 2
    }

    public enum FilterType
    {
        String = 0,
        Integer = 1,
        DateTime = 2,
        Boolean = 3,
        Float = 4,
    }

    public enum SortType
    {
        None = 0,
        Asc = 1,
        Desc = 2
    }
}
