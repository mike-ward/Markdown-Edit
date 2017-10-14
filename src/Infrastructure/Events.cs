using System;
using Prism.Events;

namespace Infrastructure
{
    public class TextUpdatedEvent : PubSubEvent<string> { }
    public class TextScrollOffsetChanged: PubSubEvent<Tuple<int, int>> { }
    public class DocumentNameChangedEvent: PubSubEvent<string> { }
    public class DocumentModifiedChangedEvent: PubSubEvent<bool> { }
    public class DisplaySettingsEvent: PubSubEvent<bool> { }
}
