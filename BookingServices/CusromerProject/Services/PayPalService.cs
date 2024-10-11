using PayPal.Api;
using Microsoft.Extensions.Configuration;
using System;
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

        public async Task<Payment> CreatePaymentAsync(decimal total, string currency, string description, string returnUrl, string cancelUrl)
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

                // Await the async creation of the payment
                return await Task.Run(() => payment.Create(apiContext));
            }
            catch (PaymentsException ex)
            {
                Console.WriteLine($"Error creating PayPal payment: {ex.Message}");
                return null;
            }
        }

        public async Task<Payment> GetPaymentAsync(string paymentId)
        {
            var apiContext = GetAPIContext();

            try
            {
                // Await the async retrieval of the payment
                return await Task.Run(() => Payment.Get(apiContext, paymentId));
            }
            catch (PaymentsException ex)
            {
                Console.WriteLine($"Error retrieving PayPal payment: {ex.Message}");
                return null;
            }
        }

        public async Task<PayoutBatch> CreatePayoutAsync(string receiverEmail, decimal totalAmount, decimal platformPercentage)
        {
            // Calculate the payout after deducting the platform percentage
            decimal payoutAmount = totalAmount * (1 - platformPercentage / 100);

            var apiContext = GetAPIContext();

            var payout = new Payout()
            {
                sender_batch_header = new PayoutSenderBatchHeader
                {
                    email_subject = "You have received a payment!",
                    recipient_type = PayoutRecipientType.EMAIL
                },
                items = new List<PayoutItem>()
                {
                    new PayoutItem()
                    {
                        receiver = receiverEmail,
                        amount = new Currency()
                        {
                            value = payoutAmount.ToString("F2"),
                            currency = "USD"
                        },
                        note = "Payout for service rendered."
                    }
                }
            };

            try
            {
                var payoutBatch = await Task.Run(() => payout.Create(apiContext, false));
                Console.WriteLine($"Payout created with Batch ID: {payoutBatch.batch_header.payout_batch_id}");
                return payoutBatch;
            }
            catch (PayPalException ex)
            {
                Console.WriteLine($"Error creating PayPal payout: {ex.Message}");
                // Log more details from the PayPal exception
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return null;
            }

        }
    }
}
