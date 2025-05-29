namespace MicroRabbit.Domain.core.Events;

public abstract class Event
{
    public DateTime Timestamp { get; set; }

    protected Event()
    {
        Timestamp = DateTime.Now;
    }
}