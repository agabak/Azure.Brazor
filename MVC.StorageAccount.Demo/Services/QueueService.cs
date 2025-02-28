using Azure.Storage.Queues;
using MVC.StorageAccount.Demo.Data;
using Newtonsoft.Json;

namespace MVC.StorageAccount.Demo.Services
{
    public class QueueService : IQueueService
    {
        private readonly QueueClient _queueClient;

        public QueueService(QueueClient queueClient)
        {
              _queueClient = queueClient;

            // Optionally ensure the queue exists at construction time.
            // This is not strictly required if you intend to create
            // the queue only when sending or reading.
            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessageAsync(EmailMessage message)
        {
            // Ensure the queue exists (only if you didn't do so in the constructor).
            await _queueClient.CreateIfNotExistsAsync();

            // Convert your message to JSON and send.
            var jsonMessage = JsonConvert.SerializeObject(message);
            await _queueClient.SendMessageAsync(jsonMessage);
        }

        public async Task<List<string>> ReadMessageAsync()
        {
            // Ensure the queue exists (only if you didn't do so in the constructor).
            await _queueClient.CreateIfNotExistsAsync();

            var messages = new List<string>();
            var properties = await _queueClient.GetPropertiesAsync();

            // Loop until ApproximateMessagesCount is 0. Note that this count may not be exact;
            // in production scenarios you might want a more robust approach.
            while (properties.Value.ApproximateMessagesCount > 0)
            {
                var messageContent = await RetrieveNextMessageAsync();
                if (!string.IsNullOrEmpty(messageContent))
                {
                    messages.Add(messageContent);
                }

                // Update the approximate message count again.
                properties = await _queueClient.GetPropertiesAsync();
            }

            return messages;
        }

        private async Task<string> RetrieveNextMessageAsync()
        {
            // Receive a single message. You can also use `ReceiveMessagesAsync` to batch.
            var response = await _queueClient.ReceiveMessageAsync();
            if (response.Value == null)
                return string.Empty;

            // Because QueueMessageEncoding is Base64, we need to decode it.
          
            // If you want to remove the message from the queue after reading:
             await _queueClient.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt);

             return response.Value.Body.ToString();
        }
    }
}
