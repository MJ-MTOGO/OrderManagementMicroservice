using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.ValueObjects;

namespace OrderManagementService.Application.Ports
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByRestaurantAndStatusAsync(Guid restaurantId, string status);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
    }


}
