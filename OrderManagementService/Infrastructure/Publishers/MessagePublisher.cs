using OrderManagementService.Application.Ports;
using OrderManagementService.Domain.ValueObjects;

namespace OrderManagementService.Infrastructure.Publishers
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IMessageBus _messageBus;

        public MessagePublisher(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public async Task PublishOrderCreatedAsync(Guid orderId, DeliveryAddress deliveryAddress)
        {
            var message = new
            {
                OrderId = orderId,
                DeliveryAddress = new
                {
                    deliveryAddress.Street,
                    deliveryAddress.City,
                    deliveryAddress.PostalCode
                }
            };

            await _messageBus.PublishAsync("OrderCreated", message);
        }
    }
}
