using LiftNet.Domain.Enums;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Profiles.Commands
{
    public class UpdateProfileCommand : IRequest<LiftNetRes>
    {
        public string UserId
        {
            get; set;
        }
        public string FirstName
        {
            get; set;
        }
        public string LastName
        {
            get; set;
        }
        public string Bio
        {
            get; set;
        }
        public string Location
        {
            get; set;
        }
        public LiftNetGender Gender
        {
            get; set;
        }
        public int Age
        {
            get; set;
        }
    }
}
