using MediatR;

namespace MicroRabbit.Domain.core.Events;

public abstract class Message : IRequest<bool> 
{
    public string MessageType { get; protected set; }

    protected Message()
    {
        MessageType = GetType().Name;
    }
}