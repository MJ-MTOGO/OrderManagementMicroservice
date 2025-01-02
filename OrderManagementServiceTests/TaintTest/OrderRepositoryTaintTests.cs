using Microsoft.EntityFrameworkCore;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Infrastructure;
using OrderManagementService.Infrastructure.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OrderManagementServiceTests.TaintTest
{
    public class OrderRepositoryTaintTests
    {
        private OrderManagementServiceDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderManagementServiceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
                .Options;

            return new OrderManagementServiceDbContext(options);
        }

        [Fact]
        public async Task GetOrdersByRestaurantAndStatusAsync_ShouldPreventSqlInjection()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            var restaurantId = Guid.NewGuid();
            var maliciousStatus = "Pending'; DROP TABLE Orders; --"; // SQL Injection attempt

            // Seed data
            var order = new Order(Guid.NewGuid(), restaurantId, new List<OrderItem>
        {
            new OrderItem("Pizza", 10m)
        });
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetOrdersByRestaurantAndStatusAsync(restaurantId, maliciousStatus);

            // Assert
            Assert.Empty(result); // Malicious input should not match any orders
            Assert.Single(context.Orders); // Ensure the Orders table is intact
        }

        [Fact]
        public async Task AddOrderAsync_ShouldNotAllowDangerousInput()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            var maliciousOrder = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem("Pizza'; DROP TABLE Orders; --", 10m) // Malicious input
        });

            // Act
            await repository.AddOrderAsync(maliciousOrder);

            // Assert
            var savedOrder = await context.Orders.FirstOrDefaultAsync();
            Assert.NotNull(savedOrder); // Ensure the order is saved
            Assert.Single(savedOrder.OrderItems); // Ensure no unintended side effects
            Assert.Equal("Pizza'; DROP TABLE Orders; --", savedOrder.OrderItems.First().Name); // Ensure data integrity
        }
    }
}
