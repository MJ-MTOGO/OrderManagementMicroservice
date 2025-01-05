using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementService;
using OrderManagementService.Application.DTOs;
using OrderManagementService.Application.Ports;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.ValueObjects;
using OrderManagementService.Infrastructure;
using OrderManagementServiceTests.Mock;
using Xunit;
using Xunit.Abstractions;

namespace OrderManagementServiceTests.IntegrationTest
{
    public class OrdersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly MockMessagePublisher _mockPublisher;
        private readonly ITestOutputHelper _output;

        public OrdersControllerIntegrationTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _output = output;

            string contentRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../OrderManagementService"));
            _output.WriteLine($"Resolved Content Root Path: {contentRoot}");
            _output.WriteLine($"Current Directory: {AppContext.BaseDirectory}");

            if (!Directory.Exists(contentRoot))
            {
                throw new DirectoryNotFoundException($"Content root directory does not exist: {contentRoot}");
            }

            _mockPublisher = new MockMessagePublisher();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    // Replace IMessagePublisher with MockMessagePublisher
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMessagePublisher));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddSingleton<IMessagePublisher, MockMessagePublisher>();

                    // Replace DbContext with in-memory database
                    var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<OrderManagementServiceDbContext>));
                    if (dbDescriptor != null)
                        services.Remove(dbDescriptor);

                    services.AddDbContext<OrderManagementServiceDbContext>(options => options.UseInMemoryDatabase("TestDb"));
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateOrder_WithValidInput_ShouldPublishMessage()
        {
            var request = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                RestaurantId = Guid.NewGuid(),
                OrderItems = new List<OrderItemRequest>
                {
                    new OrderItemRequest("Pizza", 10m),
                    new OrderItemRequest("Soda", 5m)
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

            // Verify message was published
            Assert.Single(_mockPublisher.PublishedMessages);
            var publishedMessage = _mockPublisher.PublishedMessages.First();
            Assert.Equal(request.RestaurantId, publishedMessage.RestaurantId);
            Assert.Equal(request.Street, publishedMessage.DeliveryAddress.Street);
        }

        [Fact]
        public async Task CreateOrder_WithInvalidInput_ShouldNotPublishMessage()
        {
            var request = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                RestaurantId = Guid.NewGuid(),
                Street = string.Empty, // Invalid input
                City = "SomeCity",
                PostalCode = "12345",
                OrderItems = new List<OrderItemRequest>
                {
                    new OrderItemRequest("Pizza", 10m)
                }
            };

            var response = await _client.PostAsJsonAsync("/api/orders", request);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _output.WriteLine($"Response Body: {responseBody}");
            }

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            // Verify no message was published
            Assert.Empty(_mockPublisher.PublishedMessages);
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
                    new OrderItemRequest("Pizza", 10m),
                    new OrderItemRequest("Soda", 5m)
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
