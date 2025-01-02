using System;
using System.Collections.Generic;
using OrderManagementService.Domain.Entities;
using Xunit;

namespace OrderManagementServiceTests.UnitTest
{
    public class OrderTests
    {
        //Test for Creating an Order
        [Fact]
        public void CreatingOrder_WithValidData_ShouldInitializeCorrectly()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();
            var orderItems = new List<OrderItem>
        {
            new OrderItem("Pizza", 10m),
            new OrderItem("Soda", 2m)
        };

            // Act
            var order = new Order(customerId, restaurantId, orderItems);

            // Assert
            Assert.Equal(customerId, order.CustomerId);
            Assert.Equal(restaurantId, order.RestaurantId);
            Assert.Equal("Pending", order.OrderStatus);
            Assert.Equal(12m, order.TotalPrice);
        }

        //Test for Creating an Order
        [Fact]
        public void CreatingOrder_WithNoOrderItems_ShouldThrowArgumentException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Order(customerId, restaurantId, new List<OrderItem>()));
        }

        // Test for Updating Order Items
        [Fact]
        public void AddingOrderItem_ShouldIncreaseOrderItemsCount()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>
    {
        new OrderItem("Pizza", 10m)
    });
            var newItem = new OrderItem("Burger", 15m);

            // Act
            order.AddOrderItem(newItem);

            // Assert
            Assert.Equal(2, order.OrderItems.Count);
            Assert.Contains(newItem, order.OrderItems);
        }
        // Test for Updating Order Items
        [Fact]
        public void RemovingOrderItem_ShouldDecreaseOrderItemsCount()
        {
            // Arrange
            var item = new OrderItem("Pizza", 10m);
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem> { item });

            // Act
            order.RemoveOrderItem(item);

            // Assert
            Assert.Empty(order.OrderItems);
        }
        
        
        // Test for Changing Order Status
        [Fact]
        public void MarkAsDelivered_ShouldUpdateOrderStatus()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem("Pizza", 10m)
        });

            // Act
            order.MarkAsDelivered();

            // Assert
            Assert.Equal("Delivered", order.OrderStatus);
        }
        // Test for Changing Order Status
        [Fact]
        public void MarkAsDelivered_AlreadyDelivered_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem("Pizza", 10m)
        });
            order.MarkAsDelivered();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => order.MarkAsDelivered());
        }
    }
}