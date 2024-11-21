namespace OrderManagementService.Domain.ValueObjects
{
    public class DeliveryAddress
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string PostalCode { get; private set; }

        public DeliveryAddress(string street, string city, string postalCode)
        {
            Street = street;
            City = city;
            PostalCode = postalCode;
        }
    }

}
