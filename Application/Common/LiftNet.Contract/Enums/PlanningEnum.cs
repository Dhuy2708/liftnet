using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Enums
{
    public enum ActivityLevel
    {
        None = 0,
        Light = 1, // 1-2 days a week
        Moderate = 2, // 3-5 days a week
        Heavy = 3, // 6-7 days a week
        Athlete = 4 // 2+ times a day, heavy training
    }

    public enum TrainingGoal
    {
        None = 0,
        LoseFat = 1,
        MaintainWeight = 2,
        GainMuscle = 3,
    }

    public enum UserIntent
    {
        None = 0,
        GeneralKnowledge = 1,
        PersonalAdvice = 2,
        UpdatePlan = 3,
    }
}
