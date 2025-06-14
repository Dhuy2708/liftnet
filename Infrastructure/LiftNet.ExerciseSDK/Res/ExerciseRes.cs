using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ExerciseSDK.Res
{
    public class ExerciseRes
    {
        public string Id
        {
            get; set;
        }
        public string? Name
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
        public string? Target
        {
            get; set;
        }
        public List<string>? SecondaryMuscles
        {
            get; set;
        }
        public List<string>? Instructions
        {
            get; set;
        }
        public string? Difficulty
        {
            get; set;
        }
        public string? Category
        {
            get; set;
        }
        public string? Description
        {
            get; set;
        }
    }
}
