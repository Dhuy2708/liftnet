using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Enums.Finder
{
    public enum FinderPostStatus
    {
        None = 0,
        Open = 1,
        Closed = 2,
    }

    public enum FinderApplyingStatus
    {
        None = 0,
        Applying = 1,
        Canceled = 2,
    }

    public enum FinderPostResponseType
    {
        None = 0,
        Accept = 1,
        Reject = 2,
    }
}
