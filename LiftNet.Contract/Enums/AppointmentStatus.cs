using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Enums
{
    public enum AppointmentStatus
    {
        None = 0,
        Pending = 1,
        Accepted = 2,
        Rejected = 3, 
        Canceled = 4, // reject after accepted -> canceled
    }

    public enum RepeatingType
    {
        None = 0,
        Everyday = 1,
        EveryWeek = 2,
        EveryMonth = 3,
        EveryYear = 4,
    }
}
