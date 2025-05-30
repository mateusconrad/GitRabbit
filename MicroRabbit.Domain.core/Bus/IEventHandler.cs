using MicroRabbit.Domain.core.Events;

namespace MicroRabbit.Domain.core.Bus;

public interface IEventHandler<in TEvent> : IEventHandler
    where TEvent : Event
{
    Task Handle(TEvent @event);
}

public interface IEventHandler
{
    
}