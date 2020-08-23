using Jason.Examples.MassTransit.Common.Enums;

namespace Jason.Examples.MassTransit.Common.Events
{
    public class MyEvent
    {
        public int Id { get; set; }
        public EventTypeEnum EventType { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(EventType)}: {EventType}, {nameof(Message)}: {Message}";
        }
    }
}
