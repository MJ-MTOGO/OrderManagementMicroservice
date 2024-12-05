using Newtonsoft.Json;
using OrderManagementService.Application.Ports;
using OrderManagementService.Application.DTOs;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Infrastructure.Subscribers
{
    public class CalculatedEarningsSubscriber
    {
        private readonly IMessageBus _messageSubscriber;
        private readonly IServiceProvider _serviceProvider;

        public CalculatedEarningsSubscriber(IMessageBus messageSubscriber, IServiceProvider serviceProvider)
        {
            _messageSubscriber = messageSubscriber;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync()
        {
            await _messageSubscriber.SubscribeAsync("calculated-earnings-sub", async (messageData) =>
            {
                // Deserialize the message
                var earnings = JsonConvert.DeserializeObject<Earning>(messageData);

                if (earnings != null)
                {
                    // Create a scope to resolve scoped services
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        // Resolve a domain service or repository
                        var earningService = scope.ServiceProvider.GetRequiredService<IEarningService>();

                        // Use the service to handle business logic
                        await earningService.ProcessCalculatedEarningsAsync(earnings);

                        Console.WriteLine($"Earning for Order {earnings.OrderId} processed successfully.");
                    }
                }
                else
                {
                    Console.WriteLine("Received an invalid message or failed to deserialize.");
                }
            });
        }
    }
}
