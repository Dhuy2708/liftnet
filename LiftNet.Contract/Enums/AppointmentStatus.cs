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
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4,
    }
}
