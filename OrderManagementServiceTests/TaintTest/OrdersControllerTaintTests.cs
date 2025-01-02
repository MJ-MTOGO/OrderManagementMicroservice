using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagementService.Application.DTOs;
using OrderManagementService.Application.Ports;
using OrderManagementService.Controllers;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OrderManagementServiceTests.TaintTest
{
    public class OrdersControllerTaintTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IMessagePublisher> _mockMessagePublisher;
        private readonly OrdersController _controller;

        public OrdersControllerTaintTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockMessagePublisher = new Mock<IMessagePublisher>();
            _controller = new OrdersController(_mockOrderRepository.Object, _mockMessagePublisher.Object);
        }

        [Fact]
        public async Task CreateOrder_WithMaliciousInput_ShouldReturnBadRequest()
        {
            // Arrange
            var maliciousRequest = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                RestaurantId = Guid.NewGuid(),
                Street = "123 Main St'; DROP TABLE Orders; --",
                City = "",
                PostalCode = "12345",
                OrderItems = new List<OrderItemRequest>
        {
            new OrderItemRequest { Name = "Pizza'; DROP TABLE Orders; --", Price = 1m }
        }
            };

            // Act
            var result = await _controller.CreateOrder(maliciousRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Address fields cannot be empty.", badRequestResult.Value);
            _mockOrderRepository.Verify(repo => repo.AddOrderAsync(It.IsAny<Order>()), Times.Never);
            _mockMessagePublisher.Verify(pub => pub.PublishOrderCreatedAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DeliveryAddress>()), Times.Never);
        }


        [Fact]
        public async Task CreateOrder_WithValidInput_ShouldReturnCreated()
        {
            // Arrange
            var validRequest = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                RestaurantId = Guid.NewGuid(),
                Street = "123 Main St",
                City = "SomeCity",
                PostalCode = "12345",
                OrderItems = new List<OrderItemRequest>
            {
                new OrderItemRequest { Name = "Pizza", Price = 10m }
            }
            };

            _mockOrderRepository.Setup(repo => repo.AddOrderAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            _mockMessagePublisher.Setup(pub => pub.PublishOrderCreatedAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DeliveryAddress>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateOrder(validRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(OrdersController.GetOrderById), createdResult.ActionName);
            _mockOrderRepository.Verify(repo => repo.AddOrderAsync(It.IsAny<Order>()), Times.Once);
            _mockMessagePublisher.Verify(pub => pub.PublishOrderCreatedAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DeliveryAddress>()), Times.Once);
        }
    }
}