namespace OrderManagementService.Domain.Entities
{
    public class OrderItem
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        private OrderItem() { } // Parameterless constructor for deserialization

        public OrderItem(string name, decimal price)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Price = price;
        }
    }

}
