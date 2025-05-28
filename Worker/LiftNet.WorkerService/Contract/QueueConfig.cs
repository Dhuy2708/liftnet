using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.WorkerService.Contract
{
    public class QueueConfig
    {
        public List<string> QueueNames
        {
            get; set;
        } = [];
    }
}
