namespace OrderManagementService.Application.DTOs
{
    public class CreateOrderRequest
    {
        public Guid CustomerId { get; set; }
        public Guid RestaurantId { get; set; }
        public List<OrderItemRequest> OrderItems { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }


}
