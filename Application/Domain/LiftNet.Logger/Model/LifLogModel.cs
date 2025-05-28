using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Logger.Model
{
    public class LifLogModel
    {
        public Dictionary<string, SortedDictionary<DateTime, string>> UserLogs
        {
            get; set;
        }

        public SortedDictionary<DateTime, string> SystemLogs
        {
            get; set;
        }

        public LifLogModel()
        {
            UserLogs = [];
            SystemLogs = [];
        }
        
        public void Clear()
        {
            UserLogs.Clear();
            SystemLogs.Clear();
        }
    }
}
