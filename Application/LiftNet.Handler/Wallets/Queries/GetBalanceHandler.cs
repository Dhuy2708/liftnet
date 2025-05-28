using LiftNet.Contract.Interfaces.IRepos;
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
    public class GetBalanceHandler : IRequestHandler<GetBalanceQuery, LiftNetRes<double>>
    {
        private readonly ILiftLogger<GetBalanceHandler> _logger;
        private readonly ITransactionRepo _transactionRepo;

        public GetBalanceHandler(ILiftLogger<GetBalanceHandler> logger, ITransactionRepo transactionRepo)
        {
            _logger = logger;
            _transactionRepo = transactionRepo;
        }

        public async Task<LiftNetRes<double>> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var queryable = _transactionRepo.GetQueryable();

                var balance = await queryable
                                        .Where(t => t.UserId == request.UserId)
                                        .SumAsync(t => t.Amount);

                return LiftNetRes<double>.SuccessResponse(balance);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while getting balance for user {request.UserId}");
                return LiftNetRes<double>.ErrorResponse("An error occurred while retrieving the balance.");
            }
        }
    }
}
