using LiftNet.Contract.Enums.Feed;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.ServiceBus.Contracts;
using LiftNet.Utility.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ServiceBus.Processor
{
    public class ReactFeedProcessor : EventBaseProcessor
    {
        private IFeedIndexService feedService;
        private readonly ReactType _type;
        private ILiftLogger<ReactFeedProcessor> logger => _serviceProvider.GetRequiredService<ILiftLogger<ReactFeedProcessor>>();

        public ReactFeedProcessor(EventMessage eventMessage, IServiceProvider serviceProvider, ReactType type)
            : base(eventMessage, serviceProvider)
        {
            this._type = type;
        }

        protected override async Task HandleAsync(IServiceScope scope)
        {
            try
            {
                feedService = scope.ServiceProvider.GetRequiredService<IFeedIndexService>();
                if (string.IsNullOrEmpty(_eventMessage.Context))
                {
                    logger.Error("Context is null or empty");
                    return;
                }

                var context = JsonConvert.DeserializeObject<dynamic>(_eventMessage.Context);
                if (context == null)
                {
                    logger.Error("Failed to deserialize context");
                    return;
                }

                string? feedId = context.FeedId?.ToString();
                string? userId = context.UserId?.ToString();
                string? feedOwnerId = context.FeedOwnerId?.ToString();

                if (feedId.IsNullOrEmpty() || userId.IsNullOrEmpty() || feedOwnerId.IsNullOrEmpty())
                {
                    logger.Error("FeedId or UserId is null or empty");
                    return;
                }

                bool result;
                if (_type == ReactType.Like)
                {
                    result = await feedService.LikeFeedAsync(feedId!, feedOwnerId!, userId!);
                }
                else
                {
                    result = await feedService.UnlikeFeedAsync(feedId!, feedOwnerId!, userId!);
                }

                if (!result)
                {
                    logger.Error($"Failed to {_type.ToString().ToLower()} feed {feedId} for user {userId}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error processing feed reaction");
            }
        }
    }
}
