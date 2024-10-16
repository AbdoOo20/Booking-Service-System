using BookingServices.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Microsoft.Extensions.Logging;

namespace BookingServices.Jobs
{
    public class UpdateServiceProviderBalancesJob : IJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateServiceProviderBalancesJob> _logger;
        private readonly HttpClient _client;

        public UpdateServiceProviderBalancesJob(ApplicationDbContext context, HttpClient client, ILogger<UpdateServiceProviderBalancesJob> logger)
        {
            _context = context;
            _logger = logger;
            _client = client;

        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC UpdateServiceProviderBalances");
                _context.Database.ExecuteSqlRaw("EXEC ProcessPendingBookings");
                var customershaveremainmony = _context.RemainingCustomerBalances.ToList();
                foreach (var customer in customershaveremainmony)
                {
                    var payoutRequest = new
                    {
                        receiverEmail = customer.BankAccount,
                        totalAmount = customer.RemainingAmount,
                        platformPercentage = 0
                    };

                    var response = await _client.PostAsJsonAsync("http://localhost:5285/api/PayPal/payout", payoutRequest);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Failed to send payout for BankAccount {customer.BankAccount}");
                    }
                }
                //Console.WriteLine($"Eslam Waheed => Service Provider Balances Updated at: {DateTime.Now}");
                _logger.LogInformation($"Service Provider Balances Updated at: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating service provider balances");
            }
        }

    }
}
