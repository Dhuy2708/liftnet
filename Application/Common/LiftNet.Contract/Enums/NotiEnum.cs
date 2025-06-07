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
        Seeker = 1,
        Trainer = 2,
        Admin = 3,
        AllSeeker = 4,
        AllTrainer = 5,
        AllAdmin = 6,
        All = 7,
    }

    public enum NotiEventType
    {
        None = 0,
        // appointment
        BookAppointment = 1,
        AcceptAppointment = 2,
        CancelAppointment = 3,

        // finder
        ApplyFinder = 10,
        AcceptFinder = 11,
        RejectFinder = 12,

        // social
        Follow = 20,
    }


    public enum NotiRefernceLocationType
    {
        Appointment = 1,
        Finder = 2,
        Profile = 3,
    }
}
