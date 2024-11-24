namespace OrderManagementService.Application.DTOs
{
    public class OrderDeliveryMessage
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid AgentId { get; set; }
        public DateTime DeliveringDatetime { get; set; }
        public string DeliveringAddress { get; set; }

    }
}
