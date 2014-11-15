using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkdownEdit
{
    public interface IRemoteManager
    {
        string[] RemoteProviders();

        IEnumerable<IRemoteProvider> LinkedRemoteProviders();

        Task LinkRemoteProvider(IRemoteProvider remote);

        Task UnlinkRemoteProvider(IRemoteProvider remote);
    }
}
