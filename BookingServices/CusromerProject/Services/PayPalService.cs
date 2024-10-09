using PayPal.Api;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using PayPal;

namespace CustomerProject.Services
{
    public class PayPalService
    {
        private readonly IConfiguration _config;

        public PayPalService(IConfiguration config)
        {
            _config = config;
        }

        private APIContext GetAPIContext()
        {
            var clientId = _config["PayPal:ClientId"];
            var clientSecret = _config["PayPal:ClientSecret"];
            var config = new Dictionary<string, string>
            {
                { "mode", _config["PayPal:Mode"] }
            };

            var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
            return new APIContext(accessToken);
        }

        public Payment CreatePayment(decimal total, string currency, string description, string returnUrl, string cancelUrl)
        {
            var apiContext = GetAPIContext();

            try
            {
                var payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            description = description,
                            amount = new Amount
                            {
                                currency = currency,
                                total = total.ToString("F2")
                            }
                        }
                    },
                    redirect_urls = new RedirectUrls
                    {
                        cancel_url = cancelUrl,
                        return_url = returnUrl
                    }
                };

                return payment.Create(apiContext);
            }
            catch (PaymentsException ex)
            {
                Console.WriteLine($"Error creating PayPal payment: {ex.Message}");
                return null; // or throw a custom exception to handle it elsewhere
            }
        }

        public Payment GetPayment(string paymentId)
        {
            var apiContext = GetAPIContext();

            try
            {
                // Fetch the payment using the payment ID
                var payment = Payment.Get(apiContext, paymentId);
                return payment;
            }
            catch (PaymentsException ex)
            {
                // Log the exception and return null or handle it as needed
                Console.WriteLine($"Error retrieving PayPal payment: {ex.Message}");
                return null;
            }
        }
    }
}
