using OrderManagementService.Domain.ValueObjects;

namespace OrderManagementService.Application.Ports
{
    public interface IMessagePublisher
    {
        Task PublishOrderCreatedAsync(Guid orderId, Guid RestaurantId, DeliveryAddress deliveryAddress);
    }
}
