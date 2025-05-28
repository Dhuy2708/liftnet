using LiftNet.Contract.Constants;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Contracts;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.CosmosDb.Services
{
    public class EventIndexService : IndexBaseService<EventIndexData>, IEventIndexService
    {
        public EventIndexService(CosmosCredential cred) 
                : base(cred, CosmosContainerId.Schedule)
        {
        }

        /// <summary>
        /// Inserts an event, splitting it into multiple events if it crosses midnight
        /// </summary>
        /// <param name="eventData">The event to insert</param>
        /// <returns>Number of records inserted</returns>
        public async Task<int> InsertEvent(EventIndexData eventData)
        {
            // Validate start and end times
            if (eventData.StartTime >= eventData.EndTime)
            {
                throw new ArgumentException("Start time must be before end time");
            }

            var events = new List<EventIndexData>();
            
            // If event doesn't cross midnight, just insert as is
            if (eventData.StartTime.Date == eventData.EndTime.Date)
            {
                await UpsertAsync(eventData);
                return 1;
            }

            // Calculate the number of days the event spans
            var currentDate = eventData.StartTime.Date;
            var endDate = eventData.EndTime.Date;
            
            while (currentDate <= endDate)
            {
                var newEvent = new EventIndexData
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = eventData.UserId,
                    Schema = eventData.Schema,
                    Title = eventData.Title,
                    Description = eventData.Description,
                    Color = eventData.Color,
                    AppointmentId = eventData.AppointmentId,
                    Rule = eventData.Rule,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                // Set start time for the current day
                if (currentDate == eventData.StartTime.Date)
                {
                    newEvent.StartTime = eventData.StartTime;
                }
                else
                {
                    newEvent.StartTime = currentDate.Date;
                }

                // Set end time for the current day
                if (currentDate == eventData.EndTime.Date)
                {
                    newEvent.EndTime = eventData.EndTime;
                }
                else
                {
                    newEvent.EndTime = currentDate.Date.AddDays(1).AddTicks(-1); 
                }

                events.Add(newEvent);
                currentDate = currentDate.AddDays(1);
            }

            // Bulk insert all events
            await BulkUpsertAsync(events);
            return events.Count;
        }
    }
}
