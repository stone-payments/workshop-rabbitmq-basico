using System;
using System.Text;
using RabbitMQ.Client;

namespace WorkshopRabbitMQ
{
    class Producer
    {
        static void Main(string[] args)
        {

            // Criamos aqui a connection factory.
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 35672 };

            // Instanciamos a conexão e o canal de comunicação com o RabbitMQ.
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
			{

                // Declaramos explicitamente a exchange que iremos utilizar.
                channel.ExchangeDeclare(exchange: "task_exchange",
                                        type: "fanout",
                                        durable: true,
                                        arguments: null);

                // Pegamos a mensagem a ser enviada.
				string message = GetMessage(args);

                // Necessário converter a mensagem de string para bytes.
				byte[] body = Encoding.UTF8.GetBytes(message);

                // Definimos que a mensagem enviada será persistente.
                IBasicProperties properties = channel.CreateBasicProperties();
				properties.Persistent = true;

                // Enviamos, enfim, a mensagem para o RabbitMQ.
				channel.BasicPublish(exchange: "task_exchange",
									 routingKey: "",
									 basicProperties: properties,
									 body: body);

                // Saída no console da mensagem enviada.
				Console.WriteLine(" [x] Sent {0}", message);
			}

			Console.WriteLine(" Press [enter] to exit.");
			Console.ReadLine();
        }

        /// <summary>
        /// Help us to get a message from command line.
        /// </summary>
        /// <returns>The message.</returns>
        /// <param name="args">Arguments.</param>
		private static string GetMessage(string[] args)
		{
			return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
		}
    }
}
