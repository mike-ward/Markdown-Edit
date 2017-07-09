using Prism.Events;

namespace Infrastructure
{
    public class TextUpdatedEvent : PubSubEvent<string> { }
    public class FileNameChangedEvent: PubSubEvent<string> { }
    public class DocumentModifiedChangedEvent: PubSubEvent<bool> { }
    public class OpenCommandEvent: PubSubEvent { }
    public class SaveCommandEvent: PubSubEvent { }
    public class NewCommandEvent: PubSubEvent { }
    public class HelpCommandEvent: PubSubEvent { }
}
