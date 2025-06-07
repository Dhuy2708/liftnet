using LiftNet.Contract.Enums.Payment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Wallets;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Wallets.Queries.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            try
            {
                var transactions = await _transactionRepo.GetQueryable()
                                        .Where(x => x.UserId == request.UserId  &&
                                                    x.TransactionId != null)
                                        .OrderByDescending(x => x.CreatedAt)
                                        .Skip((request.PageNumber - 1) * 10)
                                        .Take(10)
                                        .ToListAsync();
                var result = transactions.Select(ToView).ToList();
                return PaginatedLiftNetRes<PaymentHistoryView>.SuccessResponse
                    (
                        result,
                        pageNumber: request.PageNumber,
                        pageSize: 10
                    );
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error occurred while getting payment transactions for user, userId: {request.UserId}");
                return PaginatedLiftNetRes<PaymentHistoryView>.ErrorResponse("An error occurred while fetching payment transactions.");
            }
        }

        private PaymentHistoryView ToView(Transaction transaction)
        {
            return new PaymentHistoryView
            {
                Id = transaction.Id,
                TransactionId = transaction.TransactionId!,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Type = (TransactionType)transaction.Type,
                PaymentMethod = transaction.PaymentMethod == null ? PaymentMethod.None
                                                 : (PaymentMethod)transaction.PaymentMethod,
                Status = (LiftNetTransactionStatus)transaction.Status
            };
        }
    }
}
