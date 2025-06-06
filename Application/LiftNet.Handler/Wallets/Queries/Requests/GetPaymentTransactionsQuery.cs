using LiftNet.Contract.Views.Wallets;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Wallets.Queries.Requests
{
    public class GetPaymentTransactionsQuery : IRequest<PaginatedLiftNetRes<PaymentHistoryView>>
    {
        public string UserId
        {
            get; set;
        }
        public int PageNumber
        {
            get; set;
        } = 1;
    }
}
