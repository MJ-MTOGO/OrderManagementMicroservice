using Newtonsoft.Json;
using OrderManagementService.Application.Ports;
using OrderManagementService.Application.DTOs;

namespace OrderManagementService.Infrastructure.Subscribers
{
    public class OrderDeliverySubscriber
    {
        private readonly IMessageBus _messageSubscriber;
        private readonly IServiceProvider _serviceProvider;

        public OrderDeliverySubscriber(IMessageBus messageSubscriber, IServiceProvider serviceProvider)
        {
            _messageSubscriber = messageSubscriber;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync()
        {
            await _messageSubscriber.SubscribeAsync("order-delivered-sub", async (messageData) =>
            {
                // Deserialize the message
                var deliveryMessage = JsonConvert.DeserializeObject<OrderDeliveryMessage>(messageData);

                // Create a scope to resolve scoped services
                using (var scope = _serviceProvider.CreateScope())
                {
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                    // Process the order delivery message
                    var order = await orderRepository.GetOrderByIdAsync(deliveryMessage.OrderId);
                    if (order != null && order.OrderStatus == "Pending")
                    {
                        order.MarkAsDelivered();
                        await orderRepository.UpdateOrderAsync(order);
                        Console.WriteLine($"Order {deliveryMessage.OrderId} marked as delivered.");
                    }
                    else
                    {
                        Console.WriteLine($"Order {deliveryMessage.OrderId} not found or already delivered.");
                    }
                }
            });
        }
    }
}
