using Khaos.Avalanche.Watchers;

namespace Khaos.Avalanche.Hosting.CommandLine;

public class CommandLineJobsHost : IDisposable
{
    private readonly JobsHost _jobsHost = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    private Task _interfaceTask;
    private bool _isRunning;
    private bool _isDisposed;

    public void Begin(CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("Cannot begin a disposed host.");
        }

        if (_isRunning)
        {
            throw new InvalidOperationException("Cannot begin already started host.");
        }

        var localToken = _cancellationTokenSource.Token;
        var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(localToken, cancellationToken);

        _interfaceTask = Task.Run(() => MainLoop(linkedTokenSource.Token), linkedTokenSource.Token);
        _isRunning = true;
    }

    public Task Wait(CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("Cannot wait a disposed host.");
        }

        if (!_isRunning)
        {
            return Task.CompletedTask;
        }

        return _interfaceTask.WaitAsync(cancellationToken);
    }

    public Task BeginAndWait(CancellationToken cancellationToken = default)
    {
        Begin(cancellationToken);

        return Wait(cancellationToken);
    }

    public void AddJobs(params Type[] jobTypes)
    {
        foreach (var jobType in jobTypes)
        {
            _jobsHost.AddJob(
                SerializedJobStartInfo.FromJobType(jobType));
        }
    }

    private async Task MainLoop(CancellationToken cancellationToken)
    {
        _isRunning = true;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            await Console.Out.WriteAsync("$ ");
            var command = await Console.In.ReadLineAsync()
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            if (command is not null)
            {
                command = command.ToLowerInvariant().Trim();
                var parts = command.Split(' ');

                if (parts[0] == "quit" || parts[0] == "exit")
                {
                    break;
                }

                switch (parts[0])
                {
                    case "list":
                        var jobInfos = _jobsHost.GetAllJobs();

                        foreach (var jobInfo in jobInfos)
                        {
                            await Console.Out.WriteAsync($"{jobInfo.Key}\t{jobInfo.Value.TypeName}\t{jobInfo.Value.Status.ToString()}");

                            if (jobInfo.Value.Exception is not null)
                            {
                                await Console.Out.WriteAsync($"\t{jobInfo.Value.Exception.Message}");
                            }

                            await Console.Out.WriteLineAsync();
                        }
                        
                        break;
                    
                    case "run":
                        if (parts.Length > 1)
                        {
                            var id = Guid.Parse(parts[1]);

                            try
                            {
                                _jobsHost.BeginJob(id);
                            }
                            catch (Exception e)
                            {
                                await Console.Out.WriteLineAsync($"Exception: {e.Message}");
                            }
                        }
                        
                        break;
                    
                    case "get":
                        if (parts.Length > 1)
                        {
                            var id = Guid.Parse(parts[1]);

                            try
                            {
                                var jobInfo = _jobsHost.GetJobInfo(id);

                                await Console.Out.WriteLineAsync(
                                    $"{jobInfo.Id}\t{jobInfo.Status.TypeName}\t{jobInfo.Status.Status.ToString()}");

                                if (jobInfo.Watchers is not null)
                                {
                                    await Console.Out.WriteLineAsync("Counters:");
                                    
                                    foreach (var watcher in jobInfo.Watchers)
                                    {
                                        if (watcher is ISimpleCounter counter)
                                        {
                                            await Console.Out.WriteLineAsync($"\t{counter.Name}\t{counter.Value}");
                                        }
                                    }
                                }

                                if (jobInfo.Status.Exception is not null)
                                {
                                    await Console.Out.WriteLineAsync("Exception:");

                                    await Console.Out.WriteLineAsync($"\t{jobInfo.Status.Exception.Type}");
                                    await Console.Out.WriteLineAsync($"\t{jobInfo.Status.Exception.Message}");
                                }
                            }
                            catch (Exception e)
                            {
                                await Console.Out.WriteLineAsync($"Exception: {e.Message}");
                            }
                        }

                        break;
                }
            }
        }

        _isRunning = false;
    }

    public void Dispose()
    {
        _isDisposed = true;
        _cancellationTokenSource.Cancel();
        _interfaceTask.WaitAsync(TimeSpan.FromMilliseconds(100));
        _interfaceTask.Dispose();
        _jobsHost.Dispose();
        _cancellationTokenSource.Dispose();
    }
}