using OrderManagementService.Application.DTOs;
using OrderManagementService.Application.Ports;

namespace OrderManagementService.Application.Services
{
    public class PaymentService : IPaymentService
    {
        public PaymentResponse ProcessPayment(PaymentRequest request)
        {
            // Mock payment logic: Always accept the payment
            PaymentResponse response = new PaymentResponse();
            response.TransactionId = Guid.NewGuid().ToString();
            response.Message = "Payment Accepted";
            response.IsAccepted = true;

            return response;
        }
    }
}
