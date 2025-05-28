using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Dtos.Query
{
    public class RawQueryParam
    {
        public string Query
        {
            get; set;
        } = string.Empty;

        public Dictionary<string, (Type type, object value)> Params
        {
            get; set;
        } = new Dictionary<string, (Type type, object value)>();
    }
}
