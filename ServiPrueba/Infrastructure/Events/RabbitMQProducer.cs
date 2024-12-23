using RabbitMQ.Client;
using ServiPrueba.Infraestructure.Configurations;
using System.Text.Json;
using System.Text;

namespace ServiPrueba.Infraestructure.Events
{
    public class RabbitMQProducer
    {
        private readonly RabbitMQConfiguration _config;

        public RabbitMQProducer(RabbitMQConfiguration config)
        {
            _config = config;
        }

        public void Publish<T>(T message)
        {

            // Creacion de instancia y conexion a RabbitMQ
            var factory = new ConnectionFactory
            {
                HostName = _config.HostName,
                UserName = _config.UserName,
                Password = _config.Password
            };

            // Conexion
            using var connection = factory.CreateConnection();
            
            // Apertura de un canal en la conexion
            using var channel = connection.CreateModel();

            // Declaracion de la cola
            channel.QueueDeclare(queue: _config.QueueName, // nombre de la cola
                durable: true, // Si los mensajes persisten tras reinicio del servidor
                exclusive: false,  // Si la cola es exclusiva para la conexion
                autoDelete: false, // Si la cola se elimina cuando no hay consumidores
                arguments: null); // 

            // Convierte el objeto generico a Json, lo codifica usando UTF8
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish( // Envia el mensaje a la cola con el canal abierto
                exchange: "", // Indica el insercambio predeterminado
                routingKey: _config.QueueName, // Indica el nombre de la cola
                basicProperties: null, // Metadados, aqui no se usa
                body: body); // Indica el mensaje ya serializado y codificado
        }
    }
}
