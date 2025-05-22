using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Abre o canal de comunicação com a fila esperada
channel.QueueDeclare(
    queue: "Basic Test", 
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

// Cria o consumidor da mensagem
var consumer = new EventingBasicConsumer(channel);

// recebe a mensagem
consumer.Received+= (model, ea) =>
{
    var body = ea.Body;
    var message = Encoding.UTF8.GetString(body.ToArray());
    Console.WriteLine($"[x]Recebeu mensagem {message}");
};

// marca a mensagem como consumida
channel.BasicConsume("Basic Test", true, consumer);


Console.WriteLine("Press [enter] to exit ");

Console.ReadLine();