using LiftNet.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Constants
{
    public static class NotiBodies
    {
        public const string BookAppointment = "You have booked a new appointment with {0} on {1}.";
        public const string AcceptAppointment = "{0} has accepted your appointment request for {1}.";
        public const string CancelAppointment = "{0} has cancelled the appointment scheduled on {1}.";

        public const string ApplyFinder = "{0} has applied to your finder request.";
        public const string AcceptFinder = "Your finder application has been accepted by {0}.";
        public const string RejectFinder = "Your finder application has been rejected by {0}.";

        public const string Follow = "{0} has started following you.";

        public const string Default = "You have a new notification.";

        public static Dictionary<NotiEventType, string> NotiBodyMapping = new Dictionary<NotiEventType, string>
        {
            { NotiEventType.BookAppointment, BookAppointment },
            { NotiEventType.AcceptAppointment, AcceptAppointment },
            { NotiEventType.CancelAppointment, CancelAppointment },
            { NotiEventType.ApplyFinder, ApplyFinder },
            { NotiEventType.AcceptFinder, AcceptFinder },
            { NotiEventType.RejectFinder, RejectFinder },
            { NotiEventType.Follow, Follow }
        };

        public static Dictionary<NotiEventType, NotiRefernceLocationType> NotiLocationMapping = new Dictionary<NotiEventType, NotiRefernceLocationType>
        {
            { NotiEventType.BookAppointment, NotiRefernceLocationType.Appointment },
            { NotiEventType.AcceptAppointment, NotiRefernceLocationType.Appointment },
            { NotiEventType.CancelAppointment, NotiRefernceLocationType.Appointment },
            { NotiEventType.ApplyFinder, NotiRefernceLocationType.Finder },
            { NotiEventType.AcceptFinder, NotiRefernceLocationType.Finder },
            { NotiEventType.RejectFinder, NotiRefernceLocationType.Finder },
            { NotiEventType.Follow, NotiRefernceLocationType.Profile } 
        };

        public static string GetBody(NotiEventType eventType, params object[] args)
        {
            if (NotiBodyMapping.TryGetValue(eventType, out var bodyTemplate))
            {
                return string.Format(bodyTemplate, args);
            }
            return Default;
        }
    }

    public static class NotiTitles
    {
        public const string BookAppointment = "Appointment Booked";
        public const string AcceptAppointment = "Appointment Accepted";
        public const string CancelAppointment = "Appointment Cancelled";

        public const string ApplyFinder = "New Finder Application";
        public const string AcceptFinder = "Finder Application Accepted";
        public const string RejectFinder = "Finder Application Rejected";

        public const string Follow = "New Follower";

        public const string Default = "Notification";

        public static Dictionary<NotiEventType, string> NotiTitleMapping = new Dictionary<NotiEventType, string>
        {
            { NotiEventType.BookAppointment, BookAppointment },
            { NotiEventType.AcceptAppointment, AcceptAppointment },
            { NotiEventType.CancelAppointment, CancelAppointment },
            { NotiEventType.ApplyFinder, ApplyFinder },
            { NotiEventType.AcceptFinder, AcceptFinder },
            { NotiEventType.RejectFinder, RejectFinder },
            { NotiEventType.Follow, Follow }
        };

        public static string GetTitle(NotiEventType eventType)
        {
            return NotiTitleMapping.TryGetValue(eventType, out var title) ? title : Default;
        }
    }
}
