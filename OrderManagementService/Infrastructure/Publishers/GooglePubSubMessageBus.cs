using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Newtonsoft.Json;
using OrderManagementService.Application.Ports;

namespace OrderManagementService.Infrastructure.Publishers
{
    public class GooglePubSubMessageBus : IMessageBus
    {
        private readonly PublisherServiceApiClient _publisher;
        private readonly string _projectId;

        public GooglePubSubMessageBus(string projectId)
        {
            _projectId = projectId;

            // Initialize the Google Pub/Sub publisher client
            _publisher = PublisherServiceApiClient.Create();
        }

        public async Task PublishAsync(string topicName, object message)
        {
            // Convert the message object to JSON
            var jsonMessage = JsonConvert.SerializeObject(message);

            // Build the topic name
            var topic = TopicName.FromProjectTopic(_projectId, topicName);

            // Create the Pub/Sub message
            var pubsubMessage = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(jsonMessage)
            };

            // Publish the message
            await _publisher.PublishAsync(topic, new[] { pubsubMessage });
        }
    }
}
