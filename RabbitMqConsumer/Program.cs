using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("Hello, World!");

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    VirtualHost = "/",
    UserName = "guest",
    Password = "guest",
    Port = 5672
};

var connection = factory.CreateConnection();

var channel = connection.CreateModel();

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    var body = args.Body.ToArray();

    var result = Encoding.UTF8.GetString(body);

    channel.BasicAck(args.DeliveryTag,multiple:false);

    Console.WriteLine(result);
};

var consumerTag = channel.BasicConsume(
    queue: "test_queue",
    autoAck: false,
    consumerTag: "",
    noLocal: false,
    exclusive: false,
    arguments: null,
    consumer: consumer

    );

Console.ReadLine();
