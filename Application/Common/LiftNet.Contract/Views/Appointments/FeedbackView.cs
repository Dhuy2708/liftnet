using LiftNet.Contract.Views.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Appointments
{
    public class FeedbackView
    {
        public int Id
        {
            get; set;
        }

        public UserOverview User
        {
            get; set;
        }

        public int Star
        {
            get; set;
        }

        public List<string>? Medias
        {
            get; set;
        }

        public string? Content
        {
            get; set;
        }
    }
}
