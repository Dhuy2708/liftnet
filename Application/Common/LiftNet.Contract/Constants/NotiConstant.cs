﻿using LiftNet.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Constants
{
    public static class NotiBodies
    {
        public const string BookAppointment = "{0} has booked a new appointment with you.";
        public const string AcceptAppointment = "{0} has accepted your appointment.";
        public const string CancelAppointment = "{0} has cancelled the appointment scheduled on.";
        public const string RejectAppointment = "{0} has rejected the appointment scheduled on.";

        public const string ApplyFinder = "{0} has applied to your finder request.";
        public const string AcceptFinder = "Your finder application has been accepted by {0}.";
        public const string RejectFinder = "Your finder application has been rejected by {0}.";

        public const string Follow = "{0} has started following you.";

        public const string RecommendSeekerToPT = "You have a new seeker recommendation: {0}.";

        public const string Default = "You have a new notification.";

        public static Dictionary<NotiEventType, string> NotiBodyMapping = new Dictionary<NotiEventType, string>
        {
            { NotiEventType.BookAppointment, BookAppointment },
            { NotiEventType.AcceptAppointment, AcceptAppointment },
            { NotiEventType.CancelAppointment, CancelAppointment },
            { NotiEventType.RejectAppointment, RejectAppointment },
            { NotiEventType.ApplyFinder, ApplyFinder },
            { NotiEventType.AcceptFinder, AcceptFinder },
            { NotiEventType.RejectFinder, RejectFinder },
            { NotiEventType.Follow, Follow },
            { NotiEventType.RecommendSeekerToPt, RecommendSeekerToPT }
        };

        public static Dictionary<NotiEventType, NotiRefernceLocationType> NotiLocationMapping = new Dictionary<NotiEventType, NotiRefernceLocationType>
        {
            { NotiEventType.BookAppointment, NotiRefernceLocationType.Appointment },
            { NotiEventType.AcceptAppointment, NotiRefernceLocationType.Appointment },
            { NotiEventType.CancelAppointment, NotiRefernceLocationType.Appointment },
            { NotiEventType.ApplyFinder, NotiRefernceLocationType.Finder },
            { NotiEventType.AcceptFinder, NotiRefernceLocationType.Finder },
            { NotiEventType.RejectFinder, NotiRefernceLocationType.Finder },
            { NotiEventType.Follow, NotiRefernceLocationType.Profile },
            { NotiEventType.RecommendSeekerToPt, NotiRefernceLocationType.SeekerRecommendation } 
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
        public const string BookAppointment = "New Appointment Booked with You";
        public const string AcceptAppointment = "Appointment Accepted";
        public const string CancelAppointment = "Appointment Cancelled";
        public const string RejectAppointment = "Appointment Rejected";

        public const string ApplyFinder = "New Finder Application";
        public const string AcceptFinder = "Finder Application Accepted";
        public const string RejectFinder = "Finder Application Rejected";

        public const string Follow = "New Follower";

        public const string RecommendSeekerToPT = "New seeker recommendation";


        public const string Default = "Notification";

        public static Dictionary<NotiEventType, string> NotiTitleMapping = new Dictionary<NotiEventType, string>
        {
            { NotiEventType.BookAppointment, BookAppointment },
            { NotiEventType.AcceptAppointment, AcceptAppointment },
            { NotiEventType.CancelAppointment, CancelAppointment },
            { NotiEventType.RejectAppointment, RejectAppointment },
            { NotiEventType.ApplyFinder, ApplyFinder },
            { NotiEventType.AcceptFinder, AcceptFinder },
            { NotiEventType.RejectFinder, RejectFinder },
            { NotiEventType.Follow, Follow },
            { NotiEventType.RecommendSeekerToPt, RecommendSeekerToPT },
        };

        public static string GetTitle(NotiEventType eventType)
        {
            return NotiTitleMapping.TryGetValue(eventType, out var title) ? title : Default;
        }
    }
}
