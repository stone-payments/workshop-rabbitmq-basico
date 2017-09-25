using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

namespace WorkshopRabbitMQ
{
    class Consumer
    {
        public static void Main()
        {
			// Criamos aqui a connection factory.
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 35672 };

			// Instanciamos a conexão e o canal de comunicação com o RabbitMQ.
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                // Criamos a fila caso não exista.
                channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                // Fazemos o binding (a ligação) entre a exchange e a fila.
                channel.QueueBind(queue: "task_queue", exchange: "task_exchange", routingKey: "", arguments: null);

                Console.WriteLine(" [*] Waiting for messages.");

                // Aqui declaramos o processo de consumo da mensagens.
                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                // Iniciamos o processo de consumo das mensagens.
                channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}