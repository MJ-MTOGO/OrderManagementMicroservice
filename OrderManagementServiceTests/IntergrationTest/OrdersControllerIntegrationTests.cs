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

namespace OrderManagementServiceTests.IntergrationTest
{
    public class OrdersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public OrdersControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            Console.WriteLine("TEs TEst!123123");
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace DbContext with an in-memory database
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<OrderManagementServiceDbContext>));
                    services.Remove(descriptor);

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
            // Arrange
            //var request = new CreateOrderRequest
            //{
            //    CustomerId = Guid.NewGuid(),
            //    RestaurantId = Guid.NewGuid(),
            //    OrderItems = new List<OrderItemRequest>
            //    {
            //        new OrderItemRequest { Name = "Pizza", Price = 10m },
            //        new OrderItemRequest { Name = "Soda", Price = 5m }
            //    },
            //    Street = "123 Main St",
            //    City = "SomeCity",
            //    PostalCode = "12345",
           
            //};

            //// Act
            //var response = await _client.PostAsJsonAsync("/api/orders", request);

            //// Debugging: Log the response body
            //var jsonResponse = await response.Content.ReadAsStringAsync();
            //Console.WriteLine($"Response JSON: {jsonResponse}");

            //// Assert
            //response.EnsureSuccessStatusCode();
            //Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            //var createdOrder = await response.Content.ReadFromJsonAsync<Order>();
            //Assert.NotNull(createdOrder);
            //Assert.Equal(request.CustomerId, createdOrder.CustomerId);
            //Assert.Equal(request.RestaurantId, createdOrder.RestaurantId);
            //Assert.Equal(2, createdOrder.OrderItems.Count);
        }


        [Fact]
        public async Task CreateOrder_WithInvalidInput_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                RestaurantId = Guid.NewGuid(),
                Street = string.Empty, // Invalid input
                City = "SomeCity",
                PostalCode = "12345",
                OrderItems = new List<OrderItemRequest>
            {
                new OrderItemRequest { Name = "Pizza", Price = 10m }
            }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/orders", request);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Contains("Address fields cannot be empty", errorMessage);
        }
    }
}