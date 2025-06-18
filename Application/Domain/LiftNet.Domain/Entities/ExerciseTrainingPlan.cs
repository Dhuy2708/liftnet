using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("ExerciseTrainingPlan")]
    public sealed class ExerciseTrainingPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get; set;
        }

        [ForeignKey(nameof(Exercise))]
        public string ExercisesSelfId
        {
            get; set;
        }

        [ForeignKey(nameof(TrainingPlan))]
        public int TrainingPlansId
        {
            get; set;
        }

        //public int? SetCount
        //{
        //    get; set;
        //}

        //public int? RepCount
        //{
        //    get; set;
        //}

        //public int? RestMinute
        //{
        //    get; set;
        //}

        //public int? Duration // in second
        //{
        //    get; set;
        //}

        public float Order
        {
            get; set;
        }

        // mapping
        public Exercise Exercise
        {
            get; set;
        }

        public TrainingPlan TrainingPlan
        {
            get; set;
        }
    }
}
