using ServiPrueba.Application.Models;
using ServiPrueba.Infraestructure.Events;

namespace ServiPrueba.Shared.Log
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly RabbitMQProducer _producer;

        public MessagePublisher(RabbitMQProducer producer)
        {
            _producer = producer;
        }

        public Task PublishLogAsync(LogEntry logEntry)
        {
            _producer.Publish(logEntry);
            return Task.CompletedTask;
        }

        public Task PublishLogAsync(string level, string message, string additionalData)
        {
            LogEntry nuevo = new LogEntry
            {
                Level = level,
                Message = message,
                AdditionalData = additionalData,
                Guid = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                ApplicationName = "ServiPrueba"
            };

            return PublishLogAsync(nuevo);  
        }
    }
}
