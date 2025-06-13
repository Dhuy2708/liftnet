using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Entities
{
    [Table("Exercises")]
    public sealed class Exercise
    {
        [Key]
        public string SelfId
        {
            get; set;
        }
        public string? BodyPart
        {
            get; set;
        }
        public string? Equipment
        {
            get; set;
        }
        public string? GifUrl
        {
            get; set;
        }
       
        public string? Name
        {
            get; set;
        }
        public string? Target
        {
            get; set;
        }
        public string? SecondaryMuscles // serialize list string
        {
            get; set;
        }
        public string? Instructions // serialize list string
        {
            get; set;
        }
        public string? Difficulty
        {
            get; set;
        }
        public string? Description
        {
            get; set;
        }
        public string? Category
        {
            get; set;
        }
    }
}
