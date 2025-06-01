using LiftNet.Contract.Enums.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Appointments
{
    public class AppointmentConfirmationRequestView
    {
        public int Id
        {
            get; set;
        }

        public string? Img
        {
            get; set;
        }

        public string? Content
        {
            get; set;
        }

        public AppointmentConfirmationStatus Status
        {
            get; set;
        }

        public DateTimeOffset CreatedAt
        {
            get; set;
        }

        public DateTimeOffset ModifiedAt
        {
            get; set;
        }

        public DateTimeOffset ExpiresdAt
        {
            get; set;
        }
    }
}
