using LiftNet.Contract.Views.Notis;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Notis.Queries.Requests
{
    public class ListNotificationsQuery : IRequest<PaginatedLiftNetRes<NotificationView>>
    {
        public string CallerId
        {
            get; set;
        }

        public int PageNumber
        {
            get; set;
        } = 1;

        public int PageSize
        {
            get; set;
        } = 10;
    }
}
