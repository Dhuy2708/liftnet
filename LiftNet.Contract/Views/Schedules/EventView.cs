using LiftNet.Contract.Dtos;
using LiftNet.Domain.Enums.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Schedules
{
    public class EventView
    {
        public string Id
        {
            get; set;
        }
        public string AppointmentId
        {
            get; set;
        }
        public string Title
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
        public string Color
        {
            get; set;
        }
        public DateTimeOffset StartTime
        {
            get; set;
        }
        public DateTimeOffset EndTime
        {
            get; set;
        }
        public RepeatRule Rule
        {
            get; set;
        }
        public PlaceDetailDto? Location
        {
            get; set;
        }
    }
}
