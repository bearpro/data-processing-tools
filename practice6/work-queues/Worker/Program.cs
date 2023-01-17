using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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

System.Console.WriteLine("Trying attach to 'dpt-rabbit'.");

var factory = new ConnectionFactory() { 
    HostName = "dpt-rabbit", 
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
    arguments: null);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[x] Received '{message}'");

    int dots = message.Split('.').Length - 1;
    Thread.Sleep(dots * 1000);
    
    Console.WriteLine("[x] Done");

    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};

channel.BasicConsume(
    queue: "task_queue",
    autoAck: false,
    consumer: consumer);

Console.WriteLine("[x] Started listening");

// Infinite loop tool SIGTERM
await WaitSigterm();