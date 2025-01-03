using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementService;
using OrderManagementService.Application.DTOs;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace OrderManagementServiceTests.IntegrationTest
{
    public class OrdersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public OrdersControllerIntegrationTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _output = output;

            // Set up the test server with an in-memory database
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<OrderManagementServiceDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<OrderManagementServiceDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("IntegrationTestDb");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateOrder_WithValidInput_ShouldReturnCreated()
        {
            var request = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                RestaurantId = Guid.NewGuid(),
                OrderItems = new List<OrderItemRequest>
                {
                    new OrderItemRequest("Pizza", 10m),
                    new OrderItemRequest ("Soda", 5m)
                },
                Street = "123 Main St",
                City = "SomeCity",
                PostalCode = "12345"
            };

            var response = await _client.PostAsJsonAsync("/api/orders", request);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _output.WriteLine($"Response Body: {responseBody}");
            }

            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var orderResponse = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
            Assert.NotNull(orderResponse);
            Assert.Equal(request.CustomerId, orderResponse.CustomerId);
            Assert.Equal(request.RestaurantId, orderResponse.RestaurantId);
            Assert.Equal(2, orderResponse.OrderItems.Count);
        }

        [Fact]
        public async Task CreateOrder_WithInvalidInput_ShouldReturnBadRequest()
        {
            var request = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                RestaurantId = Guid.NewGuid(),
                Street = string.Empty,
                City = "SomeCity",
                PostalCode = "12345",
                OrderItems = new List<OrderItemRequest>
                {
                    new OrderItemRequest ("Pizza", 10m)
                }
            };

            var response = await _client.PostAsJsonAsync("/api/orders", request);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _output.WriteLine($"Response Body: {responseBody}");
            }

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Contains("Address fields cannot be empty", errorMessage);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnOrderDetails()
        {
            var request = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                RestaurantId = Guid.NewGuid(),
                OrderItems = new List<OrderItemRequest>
                {
                    new OrderItemRequest ("Pizza", 10m),
                    new OrderItemRequest ("Soda", 5m)
                },
                Street = "123 Main St",
                City = "SomeCity",
                PostalCode = "12345"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/orders", request);
            createResponse.EnsureSuccessStatusCode();
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderResponseDto>();

            var response = await _client.GetAsync($"/api/orders/{createdOrder.OrderId}");

            response.EnsureSuccessStatusCode();
            var orderResponse = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
            Assert.NotNull(orderResponse);
            Assert.Equal(createdOrder.OrderId, orderResponse.OrderId);
        }

        [Fact]
        public async Task GetAllOrders_ShouldReturnAllOrders()
        {
            var request1 = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                RestaurantId = Guid.NewGuid(),
                OrderItems = new List<OrderItemRequest>
                {
                    new OrderItemRequest ("Pizza", 10m)
                },
                Street = "123 Main St",
                City = "SomeCity",
                PostalCode = "12345"
            };

            var request2 = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                RestaurantId = Guid.NewGuid(),
                OrderItems = new List<OrderItemRequest>
                {
                    new OrderItemRequest ("Soda", 5m)
                },
                Street = "456 Main St",
                City = "OtherCity",
                PostalCode = "67890"
            };

            await _client.PostAsJsonAsync("/api/orders", request1);
            await _client.PostAsJsonAsync("/api/orders", request2);

            var response = await _client.GetAsync("/api/orders");

            response.EnsureSuccessStatusCode();
            var orders = await response.Content.ReadFromJsonAsync<List<OrderResponseDto>>();
            Assert.NotNull(orders);
            Assert.True(orders.Count >= 2);
        }

        [Fact]
        public void MarkAsReadyToPickup_ShouldOnlyWorkWhenPending()
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

            // Try marking as ReadyToPickup again
            Assert.Throws<InvalidOperationException>(() => order.MarkAsReadyToPickup());
        }






    }

    public class OrderResponseDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RestaurantId { get; set; }
        public DateTime OrderedTime { get; set; }
        public string OrderStatus { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }

        public class OrderItemDto
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
        }
    }
}
