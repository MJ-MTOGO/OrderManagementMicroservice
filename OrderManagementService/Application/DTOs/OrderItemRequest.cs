namespace OrderManagementService.Application.DTOs
{
    public class OrderItemRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public OrderItemRequest(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
    }
}
