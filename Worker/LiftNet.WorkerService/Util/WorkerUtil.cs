using LiftNet.Contract.Constants;
using LiftNet.WorkerService.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.WorkerService.Util
{
    public class WorkerUtil
    {
        public static QueueConfig GetQueueConfig()
        {
            try
            {
                var queueNames = typeof(QueueNames)
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.IsLiteral && !f.IsInitOnly)
                    .Select(f => f.GetValue(null)?.ToString())
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                if (queueNames == null || !queueNames.Any())
                {
                    throw new InvalidOperationException("No queue names found in QueueNames class");
                }

                return new QueueConfig()
                {
                    QueueNames = queueNames!
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to get queue configuration", ex);
            }
        }
    }
}
