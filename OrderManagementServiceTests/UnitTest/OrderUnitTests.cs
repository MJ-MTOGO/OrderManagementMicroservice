using System;
using System.Collections.Generic;
using System.Text.Json;
using OrderManagementService.Domain.Entities;
using Xunit;

namespace OrderManagementServiceTests.UnitTest
{
    public class OrderTests
    {
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

        [Fact]
        public void CreatingOrder_WithNoOrderItems_ShouldThrowArgumentException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Order(customerId, restaurantId, new List<OrderItem>()));
        }

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

        [Fact]
        public void TotalPrice_ShouldCalculateCorrectly()
        {
            // Arrange
            var orderItems = new List<OrderItem>
            {
                new OrderItem("Pizza", 10m),
                new OrderItem("Burger", 15m)
            };
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), orderItems);

            // Act
            var totalPrice = order.TotalPrice;

            // Assert
            Assert.Equal(25m, totalPrice);
        }

        [Fact]
        public void MarkAsReadyToPickup_ShouldUpdateOrderStatus()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>
            {
                new OrderItem("Pizza", 10m)
            });

            // Act
            order.MarkAsReadyToPickup();

            // Assert
            Assert.Equal("ReadyToPickup", order.OrderStatus);
        }

        [Fact]
        public void MarkAsReadyToPickup_AlreadyReady_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>
            {
                new OrderItem("Pizza", 10m)
            });
            order.MarkAsReadyToPickup();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => order.MarkAsReadyToPickup());
        }

     

        [Fact]
        public void RemovingNonExistentOrderItem_ShouldNotChangeOrderItems()
        {
            // Arrange
            var item = new OrderItem("Pizza", 10m);
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem> { item });
            var nonExistentItem = new OrderItem("Burger", 15m);

            // Act
            order.RemoveOrderItem(nonExistentItem);

            // Assert
            Assert.Single(order.OrderItems);
        }

        [Fact]
        public void OrderItem_WithZeroPrice_ShouldBeAllowed()
        {
            // Arrange
            var item = new OrderItem("Water", 0m);

            // Act & Assert
            Assert.Equal(0m, item.Price);
            Assert.Equal("Water", item.Name);
        }
        [Fact]
        public void TotalPrice_ShouldHandleNegativeAndZeroPrices()
        {
            // Arrange
            var orderItems = new List<OrderItem>
    {
        new OrderItem("Free Item", 0m),
        new OrderItem("Discounted Item", -5m)
    };
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), orderItems);

            // Act
            var totalPrice = order.TotalPrice;

            // Assert
            Assert.Equal(-5m, totalPrice); // Edge case behavior
        }

     



    }
}
