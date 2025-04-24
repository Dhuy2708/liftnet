using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Feeds.Commands.Requests;
using LiftNet.Domain.Indexes;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;

namespace LiftNet.Handler.Feeds.Commands
{
    public class ListFeedHandler : IRequestHandler<ListFeedCommand, LiftNetRes<List<FeedIndexData>>>
    {
        private readonly IFeedIndexService _feedService;
        private readonly ILiftLogger<ListFeedHandler> _logger;

        public ListFeedHandler(
            IFeedIndexService feedService,
            ILiftLogger<ListFeedHandler> logger)
        {
            _feedService = feedService;
            _logger = logger;
        }

        public async Task<LiftNetRes<List<FeedIndexData>>> Handle(ListFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var condition = request.QueryCondition;
                condition.AddCondition(new ConditionItem("userid", new List<string> { request.UserId }, FilterType.String));
                condition.AddCondition(new ConditionItem("schema", new List<string> { $"{(int)DataSchema.Feed}" }, FilterType.Integer, QueryOperator.Equal, QueryLogic.And));

                if (condition.Sort == null)
                {
                    condition.Sort = new SortCondition
                    {
                        Name = "created",
                        Type = SortType.Desc
                    };
                }

                var (feeds, nextPageToken) = await _feedService.QueryAsync(condition);
                return LiftNetRes<List<FeedIndexData>>.SuccessResponse(feeds, nextPageToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in ListFeedHandler");
                return LiftNetRes<List<FeedIndexData>>.ErrorResponse("Internal server error");
            }
        }
    }
} 