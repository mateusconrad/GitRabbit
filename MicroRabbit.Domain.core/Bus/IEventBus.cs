using MicroRabbit.Domain.core.Commands;
using MicroRabbit.Domain.core.Events;

namespace MicroRabbit.Domain.core.Bus;

public interface IEventBus
{
   Task SendCommand<T>(T command) where T : Command;
   
   void PublishCommand<T>(T @event) where T : Event;
   
   //Event Type and EventHandler
   void Subscribe<T,TH>() 
      where T : Event
      where TH : IEventHandler;
   
}