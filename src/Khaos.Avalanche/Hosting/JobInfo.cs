using Khaos.Avalanche.Watchers;

namespace Khaos.Avalanche.Hosting;

public record JobInfo(Guid Id, JobStatus Status, IWatcher[]? Watchers);