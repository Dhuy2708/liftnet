using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Enums
{
    public enum NotiTarget
    {
        None = 0,
        User = 1,
        Admin = 2,
        AllUser = 3,
        AllAdmin = 4,
        All = 5,
    }

    public enum NotiEventType
    {
        None = 0,
        // appointment
        BookAppointment = 1,
        AcceptAppointment = 2,
        CancelAppointment = 3,
        RejectAppointment = 4,

        // finder
        ApplyFinder = 10,
        AcceptFinder = 11,
        RejectFinder = 12,
        RecommendSeekerToPt = 13,

        // social
        Follow = 20,
    }


    public enum NotiRefernceLocationType
    {
        Appointment = 1,
        Finder = 2,
        Profile = 3,
        SeekerRecommendation = 4,
    }
}
