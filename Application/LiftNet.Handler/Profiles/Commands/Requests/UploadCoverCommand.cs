﻿using LiftNet.Domain.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Profiles.Commands.Requests
{
    public class UploadCoverCommand : IRequest<LiftNetRes>
    {
        public string UserId
        {
            get; set;
        }
        public IFormFile File
        {
            get; set;
        }
    }
}
