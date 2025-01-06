using Moq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Infrastructure;
using OrderManagementService.Infrastructure.Adapters;

namespace OrderManagementServiceTests.UnitTest
{
    public class OrderRepositoryTests
    {
        // test add order 
        [Fact]
        public async Task AddOrderAsync_ShouldAddOrderToDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using var context = new OrderManagementServiceDbContext(options);
            var repository = new OrderRepository(context);

            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem("Pizza", 10m)
        });

            // Act
            await repository.AddOrderAsync(order);

            // Assert
            Assert.Contains(order, context.Orders);
        }
        // update order
        [Fact]
        public async Task UpdateOrderAsync_ShouldUpdateOrderInDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_UpdateOrder")
                .Options;

            using var context = new OrderManagementServiceDbContext(options);
            var repository = new OrderRepository(context);

            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>
    {
        new OrderItem("Pizza", 10m)
    });

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Act
            order.MarkAsDelivered();
            await repository.UpdateOrderAsync(order);

            // Assert
            var updatedOrder = await context.Orders.FindAsync(order.OrderId);
            Assert.Equal("Delivered", updatedOrder.OrderStatus);
        }
    }

}
