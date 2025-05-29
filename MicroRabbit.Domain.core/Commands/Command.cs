using MicroRabbit.Domain.core.Events;

namespace MicroRabbit.Domain.core.Commands;

//Quando manda um comando, manda uma mensagem
public abstract class Command : Message
{
    public DateTime Timestamp { get; protected set; }

    protected Command()
    {
        Timestamp = DateTime.Now;
    }
}