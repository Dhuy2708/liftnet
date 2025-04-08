using LiftNet.Contract.Enums.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Dtos
{
    public class AppointmentDto
    {
        public string Id
        {
            get; set;
        }
        public UserDto Booker
        {
            get; set;
        }
        public List<UserDto> Participants
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
        public PlaceDetailDto? PlaceDetail
        {
            get; set;
        }
        public DateTime StartTime
        {
            get; set;
        }
        public DateTime EndTime
        {
            get; set;
        }
        public AppointmentStatus Status
        {
            get; set;
        }
        public RepeatingType RepeatingType
        {
            get; set;
        }
        public DateTime Created
        {
            get; set;
        }
        public DateTime Modified
        {
            get; set;
        }
    }
}
