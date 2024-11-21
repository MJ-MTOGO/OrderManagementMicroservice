using Microsoft.EntityFrameworkCore;
using OrderManagementService.Infrastructure;
using OrderManagementService.Infrastructure.Publishers;
using OrderManagementService.Application.Ports;
using OrderManagementService.Infrastructure.Adapters;

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
            builder.Services.AddSingleton<IMessageBus>(sp =>
                new GooglePubSubMessageBus(builder.Configuration["GoogleCloud:ProjectId"]));

            // Register IMessagePublisher with MessagePublisher
            builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();

            // Register IOrderRepository with its implementation
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

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

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
