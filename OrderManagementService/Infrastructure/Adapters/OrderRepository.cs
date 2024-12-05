using Microsoft.EntityFrameworkCore;
using OrderManagementService.Application.Ports;
using OrderManagementService.Domain.Entities;
using System;

namespace OrderManagementService.Infrastructure.Adapters
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderManagementServiceDbContext _context;

        public OrderRepository(OrderManagementServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByRestaurantAndStatusAsync(Guid restaurantId, string status)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.RestaurantId == restaurantId && o.OrderStatus == status)
                .ToListAsync();
        }

        public async Task AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.OrderItems) // Include related data if necessary
                .ToListAsync();
        }

    }

}
