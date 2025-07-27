using BaylongoApi.DTOs.Stripe;
using BaylongoApi.Services;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaylongoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(IPaymentService paymentService) : ControllerBase
    {
        [HttpPost("create-payment-stripe")]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment([FromBody] PaymentRequestDto dto)
        {
            var response = await paymentService.CreatePaymentIntentAsync(dto);
            return Ok(response);
        }
    }
}
