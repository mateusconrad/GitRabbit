using System.Text;
using MediatR;
using MicroRabbit.Domain.core.Bus;
using MicroRabbit.Domain.core.Commands;
using MicroRabbit.Domain.core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace MicroRabbit.Infra.bus;

public sealed class RabbitMQBus : IEventBus
{
    private readonly IMediator _meadiator;
    private readonly Dictionary<string,List<Type>> _handlers;
    private readonly List<Type> _eventTypes;

    public RabbitMQBus(IMediator mediator)
    {
        _meadiator = mediator;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();
    }
    
    public Task SendCommand<T>(T command) where T : Command
    {
        return _meadiator.Send(command);
    }

    public void PublishCommand<T>(T @event) where T : Event
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var eventName = @event.GetType().Name;
        channel.QueueDeclare(eventName, true, false, false, null);
        var message  = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish("", eventName, null, body);
    }

    public void Subscribe<T, TH>() where T : Event where TH : IEventHandler
    {
        throw new NotImplementedException();
    }
}