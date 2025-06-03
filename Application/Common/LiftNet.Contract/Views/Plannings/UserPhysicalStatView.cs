using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Plannings
{
    public class UserPhysicalStatView
    {
        public int Id
        {
            get; set;
        }

        public int? Age
        {
            get; set;
        }

        public int? Gender
        {
            get; set;
        }

        public int? Height // cm
        {
            get; set;
        }

        public float? Mass // kg
        {
            get; set;
        }

        public float? Bdf
        {
            get; set;
        }

        public int? ActivityLevel
        {
            get; set;
        }

        public int? Goal
        {
            get; set;
        }
    }
}
