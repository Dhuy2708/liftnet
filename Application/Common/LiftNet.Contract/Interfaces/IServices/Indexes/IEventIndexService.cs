using LiftNet.Domain.Indexes;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices.Indexes
{
    public interface IEventIndexService : IIndexBaseService<EventIndexData>, IDependency
    {
        /// <summary>
        /// Inserts an event, splitting it into multiple events if it crosses midnight
        /// </summary>
        /// <param name="eventData">The event to insert</param>
        /// <returns>Number of records inserted</returns>
        Task<int> InsertEvent(EventIndexData eventData);
    }
}
