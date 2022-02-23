using Khaos.Avalanche.Watchers;

namespace Khaos.Avalanche.Remoting;

public interface IJob
{
    Task Run(CancellationToken cancellationToken = default);
    IEnumerable<IWatcher> GetWatchers();
}