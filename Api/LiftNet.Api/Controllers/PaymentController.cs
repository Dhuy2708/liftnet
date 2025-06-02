using CloudinaryDotNet;
using LiftNet.Contract.Enums.Payment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Response;
using LiftNet.Handler.Wallets.Queries.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;
using System.Net;
using System.Threading.Tasks;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace LiftNet.Api.Controllers
{
    public class PaymentController : LiftNetControllerBase
    {
        private IVnpay _vnpay => _serviceProvider.GetRequiredService<IVnpay>();
        private IUnitOfWork _uow => _serviceProvider.GetRequiredService<IUnitOfWork>();
        private string UIBaseUrl
        {
            get
            {
                var url = Environment.GetEnvironmentVariable("UI_URL");
#if DEBUG
                url = "http://localhost:5173";
#endif
                return url;
            }
        }

        public PaymentController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpGet("vnpay/createPaymentUrl")]
        [Authorize]
        public async Task<ActionResult<string>> CreatePaymentUrl(double moneyToPay, string description)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

                var now = DateTime.UtcNow;
                var paymentId = now.Ticks;
                var request = new PaymentRequest
                {
                    PaymentId = paymentId,
                    Money = moneyToPay,
                    Description = description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = now, 
                    Currency = Currency.VND,
                    Language = DisplayLanguage.English
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                var transaction = new Transaction()
                {
                    PaymentId = paymentId,
                    UserId = UserId,
                    Amount = moneyToPay,
                    Description = description,
                    Type = (int)TransactionType.Topup,
                    PaymentMethod = (int)PaymentMethod.VnPay,
                    Status = (int)Contract.Enums.Payment.LiftNetTransactionStatus.Pending,
                    CreatedAt = now,
                    TimeToLive = now.AddMinutes(15),
                };
                await _uow.TransactionRepo.Create(transaction);
                await _uow.CommitAsync();
                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("vnpay/IPN")]
        public IActionResult IpnAction()
        { 
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        return Ok();
                    }

                    return BadRequest("Payment failed");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Payment information not found.");
        }

        [HttpGet("vnpay/callBack")]
        public async Task<ActionResult<string>> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                Dictionary<string, string> dictionary = Request.Query.Where<KeyValuePair<string, StringValues>>((KeyValuePair<string, StringValues> kv) => !string.IsNullOrEmpty(kv.Key) && kv.Key.StartsWith("vnp_")).ToDictionary((KeyValuePair<string, StringValues> kv) => kv.Key, (KeyValuePair<string, StringValues> kv) => kv.Value.ToString());
                var amount = (long.Parse(dictionary.GetValueOrDefault("vnp_Amount", "0"))) / 100;
                
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    await UpdateTransactionAsync(paymentResult, amount);
                    if (paymentResult.IsSuccess)
                    {
                        return base.Redirect(UIBaseUrl + $"/payment-callback?orderId={paymentResult.PaymentId}&status={(int)Contract.Enums.Payment.LiftNetTransactionStatus.Success}");
                    }

                    return base.Redirect(UIBaseUrl + $"/payment-callback?orderId={paymentResult.PaymentId}&status={(int)Contract.Enums.Payment.LiftNetTransactionStatus.Failed}");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Payment information not found.");
        }

        private async Task<int> UpdateTransactionAsync(PaymentResult? paymentResult, double amount)
        {
            if (paymentResult == null)
            {
                return 0;
            }

            var paymentId = paymentResult.PaymentId;
            var description = paymentResult.Description;

            var transaction = await _uow.TransactionRepo.GetQueryable()
                                              .FirstOrDefaultAsync(x => x.PaymentId == paymentId &&
                                                                   x.Amount == amount &&
                                                                   x.Type == (int)TransactionType.Topup &&
                                                                   x.Description == description);
            if (transaction == null || transaction.Status != (int)Contract.Enums.Payment.LiftNetTransactionStatus.Pending)
            {
                return 0;
            }

            await _uow.BeginTransactionAsync();

            transaction.TransactionId = paymentResult.VnpayTransactionId.ToString();
            transaction.Status = paymentResult.IsSuccess ? (int)Contract.Enums.Payment.LiftNetTransactionStatus.Success
                                                         : (int)Contract.Enums.Payment.LiftNetTransactionStatus.Failed;
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.TimeToLive = null;
            await _uow.TransactionRepo.Update(transaction);
            
            var wallet = await _uow.WalletRepo.GetQueryable()
                                              .FirstOrDefaultAsync(x => x.UserId == transaction.UserId);
            if (wallet != null)
            {
                wallet.Balance += amount / 1000;
                wallet.LastUpdate = DateTime.UtcNow;
                await _uow.WalletRepo.Update(wallet);
            }
            else
            {
                wallet = new Wallet()
                {
                    UserId = transaction.UserId,
                    Balance = amount / 1000,
                    LastUpdate = DateTime.UtcNow
                };
                await _uow.WalletRepo.Create(wallet);
            }

            var result = await _uow.CommitAsync();
            return result;
        }
    }
}
