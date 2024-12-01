using Newtonsoft.Json.Linq;
using OrderManagementService.Application.DTOs;
using OrderManagementService.Application.Ports;

namespace OrderManagementService.Infrastructure.Subscribers
{
    public class ReadyToPickupSubscriber
    {
        private readonly IMessageBus _messageSubscriber;
        private readonly IServiceProvider _serviceProvider;

        public ReadyToPickupSubscriber(IMessageBus messageSubscriber, IServiceProvider serviceProvider)
        {
            _messageSubscriber = messageSubscriber;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync()

        {
            await _messageSubscriber.SubscribeAsync("ready-to-pickup-sub", async (messageData) =>
                    {
                        Console.WriteLine($"Order sub kørt --444-------------------------------333333------------");
                        // Deserialize the message
                        var jsonObject = JObject.Parse(messageData);
                        var readyToPickup = jsonObject["readyToPickup"]?.ToObject<ReadyToPickup>();
                        Console.WriteLine(messageData);
                        // Create a scope to resolve scoped services
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                            // Process the order delivery message
                            var order = await orderRepository.GetOrderByIdAsync(readyToPickup.OrderId);
                            Console.WriteLine(order);

                            Console.WriteLine($"-----DB---DB------------------333333------------");
                            if (order != null && order.OrderStatus == "Pending")
                            {
                                order.MarkAsReadyToPickup();
                                await orderRepository.UpdateOrderAsync(order);
                                Console.WriteLine($"Order {readyToPickup.OrderId} marked as Ready to pickup.");
                            }
                            else
                            {
                                Console.WriteLine($"Order {readyToPickup.OrderId} not found or already ready to pickup.");
                            }
                        }

                    });
        }
    }
}
