using System.Collections.Concurrent;
using System.Collections.Immutable;

using Khaos.Avalanche.Watchers;

namespace Khaos.Avalanche.Worker;

public class JobsDispatcher
{
    private readonly ConcurrentDictionary<Guid, JobHandle> _jobs = new();

    private readonly Timer _monitorTimer;

    public JobsDispatcher()
    {
        // _monitorTimer = new Timer(
        //     CheckJobs,
        //     null,
        //     TimeSpan.FromSeconds(1),
        //     TimeSpan.FromSeconds(1));
    }

    public void StartNew(JobHandle job)
    {
        _jobs.TryAdd(job.Id, job);
        job.RunDeferred();
    }

    public IReadOnlyDictionary<Guid, JobStatus> GetStatuses() =>
        _jobs.ToImmutableDictionary(x => x.Key, x => new JobStatus(x.Value.Status, x.Value.Exception?.Message));

    private void CheckJobs(object? state)
    {
        foreach (var job in _jobs.Values.ToImmutableArray())
        {
            var status = job.Status;

            if (status is TaskStatus.RanToCompletion or TaskStatus.Faulted)
            {
                _jobs.Remove(job.Id, out _);
            }
        }
    }

    public IEnumerable<IWatcher> GetJobWatchers(Guid id)
    {
        if (!_jobs.TryGetValue(id, out var job))
        {
            throw new Exception();
        }

        return job.Instance.GetWatchers();
    }
}