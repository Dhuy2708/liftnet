using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.MapSDK.Contracts
{
    public class AutocompleteRes
    {
        public List<Prediction> Predictions { get; set; }
        public string Status { get; set; }
    }

    public class Prediction
    {
        public string Description { get; set; }
        public string PlaceId { get; set; }
        public string Reference { get; set; }
        public StructuredFormatting StructuredFormatting { get; set; }
        public PlusCode PlusCode { get; set; }
        public double Score { get; set; }
        public bool HasChildren { get; set; }
        public string DisplayType { get; set; }
    }

    public class StructuredFormatting
    {
        public string MainText { get; set; }
        public string SecondaryText { get; set; }
    }

    public class PlusCode
    {
        public string CompoundCode { get; set; }
        public string GlobalCode { get; set; }
    }
}
