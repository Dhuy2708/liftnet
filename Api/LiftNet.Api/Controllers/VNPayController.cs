using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace LiftNet.Api.Controllers
{
    [Route("api/VNPay")]
    public class VNPayController : LiftNetControllerBase
    {
        private IVnpay _vnpay => _serviceProvider.GetRequiredService<IVnpay>();
        public VNPayController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpGet("createPaymentUrl")]
        [Authorize]
        public ActionResult<string> CreatePaymentUrl(double moneyToPay, string description)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext); 

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = moneyToPay,
                    Description = description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = DateTime.UtcNow, 
                    Currency = Currency.VND,
                    Language = DisplayLanguage.English 
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("IPN")]
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

        [HttpGet("callBack")]
        public ActionResult<string> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    var resultDescription = $"{paymentResult.PaymentResponse.Description}. {paymentResult.TransactionStatus.Description}.";

                    if (paymentResult.IsSuccess)
                    {
                        return Ok(resultDescription);
                    }

                    return BadRequest(resultDescription);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Payment information not found.");
        }
    }
}
