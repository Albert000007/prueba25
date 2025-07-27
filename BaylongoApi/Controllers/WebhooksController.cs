using BaylongoApi.DTOs.Stripe;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace BaylongoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhooksController(IConfiguration configuration, IPaymentService paymentService) : ControllerBase
    {


        [HttpPost("stripe-webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    configuration["Stripe:WebhookSecret"] ?? throw new InvalidOperationException("Missing webhook secret.")
                );

                // Deserializar con expansión
                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var service = new PaymentIntentService();
                    var original = stripeEvent.Data.Object as PaymentIntent;

                    if (original == null)
                        return BadRequest("Invalid event object.");

                    var paymentIntent = await service.GetAsync(original.Id, new PaymentIntentGetOptions
                    {
                        Expand = new List<string> { "charges" }
                    });

                    var dto = new PaymentConfirmationDto
                    {
                        StripePaymentIntentId = paymentIntent.Id,
                        EventId = int.Parse(paymentIntent.Metadata["event_id"]),
                        UserId = int.Parse(paymentIntent.Metadata["user_id"]),
                        OrganizationId = int.Parse(paymentIntent.Metadata["organization_id"]),
                        Amount = (decimal)paymentIntent.AmountReceived / 100,
                        Currency = paymentIntent.Currency,
                        Status = paymentIntent.Status,
                    };

                    await paymentService.SaveConfirmedPaymentAsync(dto);
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"General error: {ex.Message}");
            }
        }

    }
}
