namespace OrderManagementService.Application.DTOs
{
    public class CreateOrderRequest
    {
        public Guid CustomerId { get; set; }
        public Guid RestaurantId { get; set; }
        public List<OrderItemRequest> OrderItems { get; set; } = new();
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }


}
