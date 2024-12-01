using OrderManagementService.Application.DTOs;

namespace OrderManagementService.Application.Ports
{
    public interface IPaymentService
    {
        PaymentResponse ProcessPayment(PaymentRequest request);
    }

}
