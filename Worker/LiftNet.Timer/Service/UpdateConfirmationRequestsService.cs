using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Enums.Payment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Timer.Service.Common;
using LiftNet.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Timer.Service
{
    public class UpdateConfirmationRequestsService : BaseSystemJob
    {
        private ILiftLogger<UpdateConfirmationRequestsService> _logger => _provider.GetRequiredService<ILiftLogger<UpdateConfirmationRequestsService>>();
        private IUnitOfWork _uow => _provider.GetRequiredService<IUnitOfWork>();

        public UpdateConfirmationRequestsService(IServiceProvider provider) 
                : base(JobType.UpdateConfirmationRequest, provider, TimeSpan.FromMinutes(1))
        {
        }

        protected override async Task<JobStatus> KickOffJobServiceAsync()
        {
            try
            {
                _logger.Info("begin to handle expired confirmation requests");

                await _uow.BeginTransactionAsync();
                var time = DateTime.UtcNow;
                var expiredRequests = await _uow.AppointmentConfirmationRepo.GetQueryable()
                                        .Where(x => x.ExpiredAt >= DateTime.UtcNow && x.Status != (int)AppointmentConfirmationStatus.Confirmed)
                                        .ToListAsync();

                if (expiredRequests.IsNullOrEmpty())
                {
                    _logger.Info("no expired confirmation requests found");
                    return JobStatus.Finished;
                }

                foreach (var request in expiredRequests)
                {
                    request.Status = (int)AppointmentConfirmationStatus.Confirmed;
                }
                await _uow.AppointmentConfirmationRepo.UpdateRange(expiredRequests);

                var appointmentIds = expiredRequests.Select(r => r.AppointmentId).ToList();
                var transactions = await _uow.LiftNetTransactionRepo.GetQueryable()
                                        .Include(x => x.Appointment)
                                        .Where(x => appointmentIds.Contains(x.AppointmentId!))
                                        .ToListAsync();

                if (transactions.IsNullOrEmpty())
                {
                    return JobStatus.Finished;
                }

                Dictionary<string, double> userIdIncreasementDict = [];
                var bookerIds = transactions.Select(r => r.Appointment?.BookerId).Distinct().ToList();
                foreach (var bookerId in bookerIds)
                {
                    userIdIncreasementDict.TryAdd(bookerId!, 0); 
                }

                foreach (var transaction in transactions)
                {
                    var bookerId = transaction.Appointment?.BookerId ?? string.Empty;
                    if (userIdIncreasementDict.TryGetValue(bookerId!, out var increasement))
                    {
                        increasement += transaction.Amount;
                    }
              
                    transaction.ToUserId = bookerId!;
                    transaction.Status = (int)LiftNetTransactionStatus.Success;
                    transaction.LastUpdate = time;
                }
                await _uow.LiftNetTransactionRepo.UpdateRange(transactions);

                var wallets = await _uow.WalletRepo.GetQueryable()
                                        .Where(x => bookerIds.Contains(x.UserId))
                                        .ToListAsync();

                foreach (var wallet in wallets)
                {
                    if (userIdIncreasementDict.TryGetValue(wallet.UserId, out var increasement))
                    {
                        wallet.Balance += increasement;
                        wallet.LastUpdate = time;
                    }
                }
                await _uow.WalletRepo.UpdateRange(wallets);

                var missingUserIds = bookerIds.Except(wallets.Select(x => x.UserId)).ToList();
                if (missingUserIds.IsNotNullOrEmpty())
                {
                    List<Wallet> newWallets = [];

                    foreach (var userId in missingUserIds)
                    {
                        newWallets.Add(new Wallet
                        {
                            UserId = userId!,
                            Balance = userIdIncreasementDict.GetValueOrDefault(userId ?? string.Empty, 0),
                            LastUpdate = time
                        });
                    }
                    await _uow.WalletRepo.CreateRange(newWallets);
                }

                var result = await _uow.CommitAsync();
                return JobStatus.Finished;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while handling expired confirmation requests");
                return JobStatus.Failed;
            }
        }
    }
}
