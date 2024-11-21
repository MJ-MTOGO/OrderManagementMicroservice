namespace OrderManagementService.Domain.Entities
{
    public class OrderItem
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        private OrderItem() { } // For EF Core

        public OrderItem(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.");

            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            Name = name;
            Price = price;
        }
    }
}
