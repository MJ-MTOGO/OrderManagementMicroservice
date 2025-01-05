using OrderManagementService.Application.Ports;
using OrderManagementService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementServiceTests.Mock
{
    public class MockMessagePublisher : IMessagePublisher
    {
        public List<(Guid OrderId, Guid RestaurantId, DeliveryAddress DeliveryAddress)> PublishedMessages { get; } = new();

        public Task PublishOrderCreatedAsync(Guid orderId, Guid restaurantId, DeliveryAddress deliveryAddress)
        {
            // Track the messages published now...
            PublishedMessages.Add((orderId, restaurantId, deliveryAddress));
            return Task.CompletedTask;
        }
    }
}
