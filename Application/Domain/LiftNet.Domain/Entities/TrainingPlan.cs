﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("TrainingPlans")]
    public sealed class TrainingPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get; set;
        }

        [ForeignKey(nameof(User))]
        public string UserId
        {
            get; set;
        }

        public int DayOfWeek
        {
            get; set;
        }

        public List<ExerciseTrainingPlan> Exercises
        {
            get; set;
        }

        // mapping
        public User User
        {
            get; set;
        }
    }
}
