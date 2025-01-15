using Microsoft.EntityFrameworkCore;
using OrderManagementService.Infrastructure;
using OrderManagementService.Infrastructure.Publishers;
using OrderManagementService.Application.Ports;
using OrderManagementService.Infrastructure.Adapters;
using OrderManagementService.Infrastructure.Subscribers;
using OrderManagementService.Application.Services;
using Google.Api;
using Prometheus;
using System.Text.Json;

namespace OrderManagementService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Register DbContext with connection string
            builder.Services.AddDbContext<OrderManagementServiceDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()
                ));

            // Register Google Pub/Sub Message Bus
            builder.Services.AddSingleton<IMessageBus, GooglePubSubMessageBus>(sp =>
                new GooglePubSubMessageBus(builder.Configuration["GoogleCloud:ProjectId"]));

            // Register Publisher and other required services
            builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IEarningService, EarningService>();
            builder.Services.AddScoped<IEarningRepository, EarningRepository>();

            // Register subscribers
            builder.Services.AddSingleton<OrderDeliverySubscriber>();
            builder.Services.AddSingleton<ReadyToPickupSubscriber>();
            builder.Services.AddSingleton<CalculatedEarningsSubscriber>();

            // Configure CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000") // Replace with your frontend URL
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Add Controllers and configure JSON options
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

            // Add Swagger/OpenAPI support
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Build the application
            var app = builder.Build();

            // Enable Prometheus metrics
            app.UseMetricServer(); // Exposes /metrics endpoint
            app.UseHttpMetrics(); // Monitors HTTP requests
            app.MapGet("/", () => "Hello, Prometheus!");

            // Configure HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Use the configured CORS policy
            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Conditionally start subscribers (except in Testing environment)
            if (!app.Environment.IsEnvironment("Testing"))
            {
                var orderDeliverySubscriber = app.Services.GetRequiredService<OrderDeliverySubscriber>();
                var readyToPickupSubscriber = app.Services.GetRequiredService<ReadyToPickupSubscriber>();
                var calculatedEarningsSubscriber = app.Services.GetRequiredService<CalculatedEarningsSubscriber>();

                Task.Run(() => orderDeliverySubscriber.StartAsync());
                Task.Run(() => readyToPickupSubscriber.StartAsync());
                Task.Run(() => calculatedEarningsSubscriber.StartAsync());
            }

            app.Run();
        }
    }
}
