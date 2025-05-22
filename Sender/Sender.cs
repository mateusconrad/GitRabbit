// Sender/ Producer

using System.Text;
using RabbitMQ.Client;


var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Declara a fila
channel.QueueDeclare(
    queue: "Basic Test", // Precisa ser igual ao routing key
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

string message = "Começando a entender esa bagaça";

// declara o corpo da mensagem, transformando em bytes
var body = Encoding.UTF8.GetBytes(message);

// envia a mensagem
channel.BasicPublish(
    exchange: "", 
    routingKey: "Basic Test", // Precisa ser igual ao nome da queue
    basicProperties: null,
    body: body);

Console.WriteLine($" [x] Sent : {message}");


Console.WriteLine("Press [enter] to exit");

// Console.ReadLine();