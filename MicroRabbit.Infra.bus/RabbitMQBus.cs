using System.Text;
using MediatR;
using MicroRabbit.Domain.core.Bus;
using MicroRabbit.Domain.core.Commands;
using MicroRabbit.Domain.core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);
        
        // Se o eventType não está na lista, então o adiciona
        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        // Se o dicionário de handlers não contém o evento, então o adiciona
        if (!_handlers.ContainsKey(eventName))
        {
            _handlers.Add(eventName, new List<Type>());
        }

        if (_handlers[eventName].Any(s => s.GetType()== handlerType))
        {
            throw new ArgumentException(
                $"Handler Type {handlerType.Name} já está registrado para o eventName {eventName}!",nameof(handlerType));
        }
        
        _handlers[eventName].Add(handlerType);

        StartBasicConsume<T>();


    }

    private void StartBasicConsume<T>() where T : Event
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            DispatchConsumersAsync = true
        };
        
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();


        var eventName = typeof(T).Name;
        channel.QueueDeclare(eventName, true, false, false, null);
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        //sintaxe para delegate
        consumer.Received += Consumer_Received;
        channel.BasicConsume(eventName, true, consumer);
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
    {
        var eventName = @event.RoutingKey;
        var message = Encoding.UTF8.GetString(@event.Body.ToArray());

        try
        {
            await ProcessEvent(eventName, message).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (_handlers.ContainsKey(eventName))
        {
            var subscriptions = _handlers[eventName];
            foreach (var subscription in subscriptions)
            {
                var handler = Activator.CreateInstance(subscription);
                if (handler == null)
                {
                    continue;
                }
                
                var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                var @event = JsonConvert.DeserializeObject(message, eventType);
                var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                
                await (Task)concreteType
                    .GetMethod("Handle")
                    .Invoke(handler, new object[] { @event });
            }
        }
    }
    
}







