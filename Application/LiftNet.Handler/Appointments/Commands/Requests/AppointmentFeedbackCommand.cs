using LiftNet.Domain.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands.Requests
{
    public class AppointmentFeedbackCommand : IRequest<LiftNetRes>
    {
        public string CallerId
        {
            get; set;
        }

        public string AppointmentId
        {
            get; set;
        }

        public IFormFile? Image
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
