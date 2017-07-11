using Prism.Events;

namespace Infrastructure
{
    public class TextUpdatedEvent : PubSubEvent<string> { }
    public class DocumentNameChangedEvent: PubSubEvent<string> { }
    public class DocumentModifiedChangedEvent: PubSubEvent<bool> { }
}
