using Microsoft.AspNetCore.Mvc;
using CustomerProject.Services;
using Microsoft.AspNetCore.Authorization;

namespace CustomerProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayPalController : ControllerBase
    {
        private readonly PayPalService _payPalService;

        public PayPalController(PayPalService payPalService)
        {
            _payPalService = payPalService;
        }
        
        [HttpGet("payment/{paymentId}")]
        [Authorize]
        public async Task<IActionResult> GetPayment(string paymentId)
        {
            var payment = await _payPalService.GetPaymentAsync(paymentId);

            if (payment == null)
                return NotFound("Payment not found or error occurred.");

            return Ok(payment);
        }

        [HttpPost("create-payment")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            var payment = await _payPalService.CreatePaymentAsync(request.Total, request.Currency, request.Description, request.ReturnUrl, request.CancelUrl);

            if (payment == null)
                return BadRequest("Failed to create payment. Check the PayPal settings or parameters.");

            var approvalUrl = payment.links.FirstOrDefault(l => l.rel == "approval_url")?.href;

            if (string.IsNullOrEmpty(approvalUrl))
                return BadRequest("Could not get PayPal approval URL.");

            return Ok(new { approvalUrl });
        }

        // New: Async Payout Endpoint for Providers
        [HttpPost("payout")]
        public async Task<IActionResult> PayoutToProvider([FromBody] PayoutRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ServiceProviderEmail))
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                var payoutBatch = await _payPalService.CreatePayoutAsync(request.ServiceProviderEmail, request.TotalAmount, request.PlatformPercentage);

                if (payoutBatch == null)
                    return BadRequest("Failed to create payout. Check PayPal settings or parameters.");

                return Ok(new
                {
                    message = "Payout successful!",
                    payoutBatchId = payoutBatch.batch_header.payout_batch_id,
                    payoutStatus = payoutBatch.batch_header.batch_status
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during payout: {ex.Message}");
                return StatusCode(500, "Internal server error during payout.");
            }
        }
    }

    // Existing PaymentRequest Class
    public class PaymentRequest
    {
        public decimal Total { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }

    // New PayoutRequest Class
    public class PayoutRequest
    {
        public string ServiceProviderEmail { get; set; } // This should be the PayPal email of the provider
        public decimal TotalAmount { get; set; } // The total amount paid by the customer
        public decimal PlatformPercentage { get; set; } // The platform's commission percentage
    }

}
