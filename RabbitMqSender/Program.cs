using RabbitMQ.Client;

Console.WriteLine("Hello, World!");

var factory = new ConnectionFactory()
{
    HostName= "localhost",
    VirtualHost= "/",
    UserName= "guest",
    Password= "guest",
    Port = 5672
};

var connection = factory.CreateConnection();

var channel = connection.CreateModel();

while (true)
{
    Console.WriteLine("enter Message : ");
    var messageString = Console.ReadLine();

    var messageByte = System.Text.Encoding.UTF8.GetBytes(messageString);
    channel.BasicPublish(exchange:"",routingKey: "test_queue", basicProperties: null,messageByte);

}
