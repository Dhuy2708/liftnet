using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Wallets;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Wallets.Queries.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Wallets.Queries
{
    public class GetPaymentTransactionsHandler : IRequestHandler<GetPaymentTransactionsQuery, PaginatedLiftNetRes<PaymentHistoryView>>
    {
        private readonly ILiftLogger<GetPaymentTransactionsHandler> _logger;
        private readonly ITransactionRepo _transactionRepo;

        public GetPaymentTransactionsHandler(ILiftLogger<GetPaymentTransactionsHandler> logger, ITransactionRepo transactionRepo)
        {
            _logger = logger;
            _transactionRepo = transactionRepo;
        }

        public async Task<PaginatedLiftNetRes<PaymentHistoryView>> Handle(GetPaymentTransactionsQuery request, CancellationToken cancellationToken)
        {
            //try
            //{
            //    var query = _transactionRepo.GetQueryable()
            //        .Where(x => x.UserId == request.UserId && x. == Contract.Enums.TransactionType.Payment)
            //        .OrderByDescending(x => x.CreatedAt)
            //        .AsQueryable();
            //}
            //catch (Exception e)
            //{
            //    _logger.Error(e, $"Error occurred while getting payment transactions for user, userId: {request.UserId}");
            //    return PaginatedLiftNetRes<PaymentHistoryView>.ErrorResponse("An error occurred while fetching payment transactions."); 
            //}
            throw new NotImplementedException();
        }
    }
}
