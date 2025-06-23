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
    public class FeedbackCommand : IRequest<LiftNetRes>
    {
        public string CallerId
        {
            get; set;
        }

        public string AppointmentId
        {
            get; set;
        }

        public string CoachId
        {
            get; set;
        }

        public List<IFormFile>? Medias
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
