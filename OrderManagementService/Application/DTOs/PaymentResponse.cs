namespace OrderManagementService.Application.DTOs
{
    public class PaymentResponse
    {
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public bool  IsAccepted { get; set; }
    }
}   
