using System.Collections.Concurrent;

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

    public void Run(Guid jobId)
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