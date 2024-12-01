using Microsoft.EntityFrameworkCore;
using OrderManagementService.Infrastructure;
using OrderManagementService.Infrastructure.Publishers;
using OrderManagementService.Application.Ports;
using OrderManagementService.Infrastructure.Adapters;
using OrderManagementService.Infrastructure.Subscribers;
using OrderManagementService.Application.Services;

namespace OrderManagementService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Register DbContext with the connection string
            builder.Services.AddDbContext<OrderManagementServiceDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()
                ));

            // Register IMessageBus with GooglePubSubMessageBus
            builder.Services.AddSingleton<IMessageBus, GooglePubSubMessageBus>(sp =>
                new GooglePubSubMessageBus(builder.Configuration["GoogleCloud:ProjectId"]));

            // Register IMessagePublisher with MessagePublisher
            builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();

            // Register IOrderRepository with its implementation
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            // Register OrderDeliverySubscriber as a singleton
            builder.Services.AddSingleton<OrderDeliverySubscriber>();
            builder.Services.AddSingleton<ReadyToPickupSubscriber>();


            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000") // Your frontend URL
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Add if credentials like cookies are needed
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Use the CORS policy
            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            // Start the OrderDeliverySubscriber in a background task
            var orderDeliverySubscriber = app.Services.GetRequiredService<OrderDeliverySubscriber>();
            var readyToPikcupSubscriber = app.Services.GetRequiredService<ReadyToPickupSubscriber>();
            Task.Run(() => orderDeliverySubscriber.StartAsync());
            Task.Run(() => readyToPikcupSubscriber.StartAsync());

            app.Run();
        }
    }
}
