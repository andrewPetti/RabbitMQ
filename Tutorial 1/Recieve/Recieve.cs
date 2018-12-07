using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Recieve
{
	public class Recieve
	{
		static void Main( string[] args )
		{
			var factory = new ConnectionFactory() { HostName = "192.168.99.100"};

			using( var connection = factory.CreateConnection() )
			{
				using( var channel = connection.CreateModel() )
				{
					channel.QueueDeclare( queue: "hello", durable: false, exclusive: false,
					                      autoDelete: false, arguments: null );

					var consumer = new EventingBasicConsumer( channel );

					consumer.Received += ( model, ea ) =>
					{
						var body = ea.Body;
						var message = Encoding.UTF8.GetString( body );
						Console.WriteLine( " [x] Recieved {0}", message );
					};

					channel.BasicConsume( queue: "hello",
					                      autoAck: true,
					                      consumer: consumer );

					Console.WriteLine( " Press [enter] to exit." );
					Console.ReadLine();
				}
			}
		}
	}
}
