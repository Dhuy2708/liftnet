using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Logger.Model
{
    public class LifLogModel
    {
        public string UserId { get; set; }
        public SortedDictionary<DateTime, string> Logs { get; set; } = new SortedDictionary<DateTime, string>();
    }
}
