using Microsoft.AspNetCore.Mvc;
using CustomerProject.Services;
using PayPal.Api;
using System.Linq;

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
        public IActionResult GetPayment(string paymentId)
        {
            var payment = _payPalService.GetPayment(paymentId);

            if (payment == null)
                return NotFound("Payment not found or error occurred.");

            return Ok(payment);
        }

        [HttpPost("create-payment")]
        public IActionResult CreatePayment([FromBody] PaymentRequest request)
        {
            var payment = _payPalService.CreatePayment(request.Total, request.Currency, request.Description, request.ReturnUrl, request.CancelUrl);

            if (payment == null)
                return BadRequest("Failed to create payment. Check the PayPal settings or parameters.");

            var approvalUrl = payment.links.FirstOrDefault(l => l.rel == "approval_url")?.href;

            if (string.IsNullOrEmpty(approvalUrl))
                return BadRequest("Could not get PayPal approval URL.");

            return Ok(new { approvalUrl });
        }
    }

    public class PaymentRequest
    {
        public decimal Total { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
