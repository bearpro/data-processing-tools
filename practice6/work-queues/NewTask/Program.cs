using System.Text;
using RabbitMQ.Client;

static string GetMessage(string[] args) => args.Length switch
{
    > 0 => string.Join(" ", args),
    _ => "Hello World!"
};

var factory = new ConnectionFactory() { 
    HostName = "localhost",
    UserName = "guest", 
    Password = "guest",
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();


channel.QueueDeclare(
    queue: "task_queue",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

var message = GetMessage(args);
var encodedMessage = Encoding.UTF8.GetBytes(message);

var properties = channel.CreateBasicProperties();
properties.Persistent = true;

channel.BasicPublish(
    exchange: "",
    routingKey: "task_queue",
    basicProperties: null,
    body: encodedMessage
);

Console.WriteLine($"Message '{message}' sent.");
