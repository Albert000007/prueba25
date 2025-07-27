using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Stripe;
using BaylongoApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Claims;

namespace BaylongoApi.Controllers
{
    [ApiController]
    [Route("api/stripe-connect")]
    public class StripeConnectController(StripeService stripeService, BaylongoContext context, IConfiguration configuration) : ControllerBase
    {
        // Endpoint 1: Iniciar onboarding (para usuarios/organizaciones)
        [HttpPost("onboarding")]
        public async Task<IActionResult> StartOnboarding([FromBody] OnboardingRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Asume autenticación JWT
            var stripeAccountId = await stripeService.CreateExpressAccount(userId, request.OrganizationId);
            var onboardingUrl = await stripeService.GenerateOnboardingLink(stripeAccountId);

            return Ok(new { OnboardingUrl = onboardingUrl });
        }

        // Endpoint 2: Webhook para eventos de Stripe (ej: onboarding completado)
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], configuration["Stripe:WebhookSecret"]);

            // Guardar evento en DB
            context.StripeEvents.Add(new StripeEvent
            {
                StripeEventId = stripeEvent.Id,
                EventType = stripeEvent.Type,
                Payload = json
            });
            await context.SaveChangesAsync();

            // Procesar eventos relevantes
            switch (stripeEvent.Type)
            {
                case "account.updated": // ← ¡Así se usa ahora!
                    var account = stripeEvent.Data.Object as Account;
                    await UpdateOnboardingStatus(account.Id, account.ChargesEnabled);
                    break;

                case "payment_intent.succeeded":
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
         
                    break;

                default:
                    //_logger.LogInformation($"Evento no manejado: {stripeEvent.Type}");
                    break;
            }


            return Ok();
        }

        private async Task UpdateOnboardingStatus(string stripeAccountId, bool chargesEnabled)
        {
            var account = await context.StripeAccounts
                .FirstOrDefaultAsync(sa => sa.StripeAccountId == stripeAccountId);

            if (account != null)
            {
                account.OnboardingStatus = chargesEnabled ? "completed" : "pending";
                account.LastUpdated = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
        }
    }
}
