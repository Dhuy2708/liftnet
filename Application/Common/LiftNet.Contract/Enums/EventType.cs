using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Enums
{
    public enum EventType
    {
        None = 0,
        Job = 1,
        Noti = 2,
        
        // feed
        LikeFeed = 10,
        UnLikeFeed = 11,

    }
}
