using LiftNet.Contract.Dtos;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Contract.Views.Schedules;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Schedules.Queries.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftNet.Handler.Schedules.Queries
{
    public class ListEventsHandler : IRequestHandler<ListEventQueryRequest, LiftNetRes<ScheduleView>>
    {
        private readonly ILiftLogger<ListEventsHandler> _logger;
        private readonly IEventIndexService _eventIndexService;

        public ListEventsHandler(ILiftLogger<ListEventsHandler> logger, IEventIndexService eventIndexService)
        {
            _logger = logger;
            _eventIndexService = eventIndexService;
        }

        public async Task<LiftNetRes<ScheduleView>> Handle(ListEventQueryRequest request, CancellationToken cancellationToken)
        {
            _logger.Info("Begin to handle list events query");

            var condition = new QueryCondition
            {
                ConditionItems = new List<ConditionItem>
                {
                    new ConditionItem(
                        "starttime",
                        new List<string> { request.StartDate.ToDateTime(TimeOnly.MinValue).ToString("o") },
                        FilterType.DateTime,
                        QueryOperator.GreaterThanOrEqual
                    ),
                    new ConditionItem(
                        "endtime",
                        new List<string> { request.EndDate.ToDateTime(TimeOnly.MaxValue).ToString("o") },
                        FilterType.DateTime,
                        QueryOperator.LessThanOrEqual,
                        QueryLogic.And
                    )
                },
                PageSize = int.MaxValue
            };

            if (!string.IsNullOrEmpty(request.EventSearch))
            {
                condition.AddCondition(new ConditionItem(
                    "title",
                    new List<string> { request.EventSearch.ToLower() },
                    FilterType.String,
                    QueryOperator.Contains,
                    QueryLogic.And
                ));
            }

            if (request.UserIds != null && request.UserIds.Any())
            {
                condition.AddCondition(new ConditionItem(
                    "userid",
                    request.UserIds,
                    FilterType.String,
                    QueryOperator.Equal,
                    QueryLogic.And
                ));
            }

            var (items, _) = await _eventIndexService.QueryAsync(condition);

            var eventsByDate = items
                .GroupBy(x => DateOnly.FromDateTime(x.StartTime))
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new EventView
                    {
                        Id = x.Id,
                        AppointmentId = x.AppointmentId,
                        Title = x.Title,
                        Description = x.Description,
                        Color = x.Color,
                        StartTime = new DateTimeOffset(x.StartTime),
                        EndTime = new DateTimeOffset(x.EndTime),
                        Rule = x.Rule,
                        Location = x.Location != null ? new PlaceDetailDto
                        {
                            PlaceName = x.Location.PlaceName,
                            FormattedAddress = x.Location.FormattedAddress,
                            Latitude = x.Location.Latitude,
                            Longitude = x.Location.Longitude,
                            PlaceId = x.Location.PlaceId
                        } : null
                    }).ToList()
                );

            var result = new ScheduleView
            {
                Events = eventsByDate
            };

            _logger.Info("List events query handled successfully");
            return LiftNetRes<ScheduleView>.SuccessResponse(result);
        }
    }
}
