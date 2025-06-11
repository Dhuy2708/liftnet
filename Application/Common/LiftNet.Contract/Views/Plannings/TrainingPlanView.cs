using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Plannings
{
    public class TrainingPlanView
    {
        public int Id
        {
            get; set;
        }

        public int DayOfWeek
        {
            get; set;
        }

        public List<ExerciseView> Exercises
        {
            get; set;
        }
    }
}
