using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Dtos.Appointment
{
    public class AppointmentFeedbackRequestDto
    {
        public string ReviewerId
        {
            get; set;
        }

        public string AppointmentId
        {
            get; set;
        }

        public IFormFile? Img
        {
            get; set;
        }

        public string? Content
        {
            get; set;
        }

        public int Star
        {
            get; set;
        }
    }
}
