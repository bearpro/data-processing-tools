using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

static async Task<int> WaitSigterm()
{
    var cts = new CancellationTokenSource();
    AppDomain.CurrentDomain.ProcessExit += (s, e) => {
        Console.WriteLine("Recieved SIGTERM");
        cts.Cancel();
    };
    await Task.Delay(-1, cts.Token);
    Console.WriteLine("Stopped.");
    return 0;
}

var factory = new ConnectionFactory() { 
    HostName = "dpt-rabbit-2", 
    UserName = "guest", 
    Password = "guest",
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

var queueName = channel.QueueDeclare().QueueName;
channel.QueueBind(queue: queueName,
                    exchange: "logs",
                    routingKey: "");

Console.WriteLine(" [*] Waiting for logs.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [x] {0}", message);
};
channel.BasicConsume(queue: queueName,
                        autoAck: true,
                        consumer: consumer);

await WaitSigterm();