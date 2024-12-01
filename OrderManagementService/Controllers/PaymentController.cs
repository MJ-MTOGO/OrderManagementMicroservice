using Microsoft.AspNetCore.Mvc;
using OrderManagementService.Application.DTOs;
using OrderManagementService.Application.Ports;

namespace OrderManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public IActionResult ProcessPayment([FromBody] PaymentRequest request)
        {
            var response = _paymentService.ProcessPayment(request);
            return Ok(response);
        }
    }
}
