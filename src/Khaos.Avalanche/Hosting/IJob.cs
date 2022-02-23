using Khaos.Avalanche.Watchers;

namespace Khaos.Avalanche.Hosting;

public interface IJob
{
    Task Run(CancellationToken cancellationToken = default);
    IEnumerable<IWatcher> GetWatchers();
}