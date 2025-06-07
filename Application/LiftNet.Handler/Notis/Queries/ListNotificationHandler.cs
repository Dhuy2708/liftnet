using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Notis;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Notis.Queries.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Notis.Queries
{
    public class ListNotificationHandler : IRequestHandler<ListNotificationsQuery, PaginatedLiftNetRes<NotificationView>>
    {
        private readonly ILiftLogger<ListNotificationHandler> _logger;
        private readonly INotificationService _notificationService;

        public ListNotificationHandler(ILiftLogger<ListNotificationHandler> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task<PaginatedLiftNetRes<NotificationView>> Handle(ListNotificationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _notificationService.GetNotifications(request.CallerId, request.PageNumber, request.PageSize);
                var count = await _notificationService.GetNotiCount(request.CallerId);
                if (result.IsNotNullOrEmpty())
                {
                    return PaginatedLiftNetRes<NotificationView>.SuccessResponse(result, pageNumber: request.PageNumber, pageSize: request.PageSize, totalCount: count);
                }
                
                return PaginatedLiftNetRes<NotificationView>.ErrorResponse();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error while listing notifications for user {request.CallerId}");
                return PaginatedLiftNetRes<NotificationView>.ErrorResponse("An error occurred while fetching notifications.");
            }
        }
    }
}
