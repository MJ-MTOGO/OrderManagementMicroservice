namespace OrderManagementService.Application.Ports
{
    public interface IMessageBus
    {
        Task PublishAsync(string topic, object message);
    }

}
