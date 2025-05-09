using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Schedules
{
    public class ScheduleView
    {
        public Dictionary<DateOnly, List<EventView>> Events
        {
            get; set;
        }
    }
}
