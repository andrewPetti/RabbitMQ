using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker
{
	internal class Worker
	{
		static void Main( string[] args )
		{
			var factory = new ConnectionFactory() { HostName = "192.168.99.100" };

			using( var connection = factory.CreateConnection() )
			{
				using( var channel = connection.CreateModel() )
				{
					channel.QueueDeclare( queue: "hello", durable: false, exclusive: false,
					                      autoDelete: false, arguments: null );

					channel.BasicQos( prefetchSize: 0, prefetchCount: 1, global: false );

					Console.WriteLine( " [*] Waiting for message." );

					var consumer = new EventingBasicConsumer( channel );

					consumer.Received += ( model, ea ) =>
					{
						var body = ea.Body;
						var message = Encoding.UTF8.GetString( body );
						Console.WriteLine( " [x] Recieved {0}", message );

						var dots = message.Split( '.' ).Length - 1;
						Thread.Sleep( dots * 1000 );

						Console.WriteLine( " [x] Done" );
						channel.BasicAck( deliveryTag: ea.DeliveryTag, multiple: false );
					};

					channel.BasicConsume( queue: "task_queue",
					                      autoAck: false,
					                      consumer: consumer );

					Console.WriteLine( " Press [enter] to exit." );
					Console.ReadLine();
				}
			}
		}
	}
}
