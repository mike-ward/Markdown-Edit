using Prism.Events;

namespace Infrastructure
{
    public class TextUpdatedEvent : PubSubEvent<string> { }
    public class FileNameChangedEvent: PubSubEvent<string> { }
}
