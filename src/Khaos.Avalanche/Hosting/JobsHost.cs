using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Khaos.Avalanche.Hosting;

public class JobsHost : IDisposable
{
    private readonly ConcurrentDictionary<Guid, SerializedJobStartInfo> _jobsReadyToStart = new();
    private readonly ConcurrentDictionary<Guid, JobHandle> _jobsRunning = new();
    private readonly ConcurrentDictionary<Guid, JobStatus> _jobsCompleted = new();

    private bool _isDisposed;

    public Guid AddJob(SerializedJobStartInfo jobStartInfo)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("Cannot add a job to disposed host.");
        }
        
        var jobId = Guid.NewGuid();

        if (!_jobsReadyToStart.TryAdd(jobId, jobStartInfo))
        {
            throw new Exception("Failed to add the job to ready to start list.");
        }

        return jobId;
    }

    public void BeginJob(Guid jobId)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("Cannot add run a job on disposed host.");
        }
        
        if (!_jobsReadyToStart.TryRemove(jobId, out var jobStartInfo))
        {
            throw new Exception("Failed to get job with specified ID.");
        }
        
        var jobHandle = new JobHandle(jobId, jobStartInfo);
        jobHandle.Completed += OnJobCompleted;

        if (!_jobsRunning.TryAdd(jobId, jobHandle))
        {
            throw new Exception("Failed to add the job to running list.");
        }

        jobHandle.Begin();
    }

    public IReadOnlyDictionary<Guid, JobStatus> GetAllJobs() =>
        _jobsReadyToStart.Select(kv => (kv.Key, new JobStatus(kv.Value.EntryType, TaskStatus.WaitingToRun, null)))
            .Concat(_jobsRunning.Select(kv => (kv.Key, kv.Value.Status)))
            .Concat(_jobsCompleted.Select(kv => (kv.Key, kv.Value)))
            .ToImmutableDictionary(kv => kv.Key, kv => kv.Item2);

    public JobInfo GetJobInfo(Guid jobId)
    {
        if (_jobsReadyToStart.TryGetValue(jobId, out var job1))
        {
            return new JobInfo(jobId, new JobStatus(job1.EntryType, TaskStatus.WaitingToRun, null), null);
        }

        if (_jobsRunning.TryGetValue(jobId, out var job2))
        {
            return new JobInfo(jobId, job2.Status, job2.Watchers);
        }

        if (_jobsCompleted.TryGetValue(jobId, out var job3))
        {
            return new JobInfo(jobId, job3, null);
        }

        throw new Exception("Failed to get job with specified ID.");
    }

    private void OnJobCompleted(JobHandle job)
    {
        if (!_jobsRunning.TryRemove(job.Id, out _))
        {
            throw new Exception("Failed to get job with specified ID.");
        }

        job.Completed -= OnJobCompleted;
        job.Dispose();

        var status = job.Status;

        if (!_jobsCompleted.TryAdd(job.Id, status))
        {
            throw new Exception("Failed to add job with to completed list.");
        }
    }

    public void Dispose()
    {
        _isDisposed = true;

        foreach (var runningJob in _jobsRunning.Values)
        {
            runningJob.Dispose();
        }
        
        _jobsReadyToStart.Clear();
        _jobsRunning.Clear();
        _jobsCompleted.Clear();

        GC.SuppressFinalize(this);
    }
}