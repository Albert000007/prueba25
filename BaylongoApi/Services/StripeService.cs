using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using Stripe;
using System;

namespace BaylongoApi.Services
{
    public class StripeService
    {
        private readonly IConfiguration _config;
        private readonly BaylongoContext _db;

        public StripeService(IConfiguration config, BaylongoContext db)
        {
            _config = config;
            _db = db;
        }
        // Crea una cuenta Express y guarda el ID en la DB
        public async Task<string> CreateExpressAccount(int userId, int? organizationId)
        {
            var options = new AccountCreateOptions
            {
                Type = "express",
                Country = "MX", // País por defecto (ajustable)
                Capabilities = new AccountCapabilitiesOptions
                {
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                },
            };

            var service = new AccountService();
            var account = await service.CreateAsync(options);

            // Guardar en DB
            var stripeAccount = new StripeAccount
            {
                UserId = userId,
                OrganizationId = organizationId,
                StripeAccountId = account.Id,
                AccountType = "express",
                OnboardingStatus = "pending"
            };
            _db.StripeAccounts.Add(stripeAccount);
            await _db.SaveChangesAsync();

            return account.Id;
        }

        // Genera link de onboarding
        public async Task<string> GenerateOnboardingLink(string stripeAccountId)
        {
            var options = new AccountLinkCreateOptions
            {
                Account = stripeAccountId,
                RefreshUrl = $"{_config["Frontend:Url"]}/reauth",
                ReturnUrl = $"{_config["Frontend:Url"]}/onboarding-complete",
                Type = "account_onboarding",
                Collect = "eventually_due" // ¡Clave! Fuerza la solicitud de documentos pendientes
            };
            var service = new AccountLinkService();
            var accountLink = await service.CreateAsync(options);
            return accountLink.Url;
        }

    }
}
