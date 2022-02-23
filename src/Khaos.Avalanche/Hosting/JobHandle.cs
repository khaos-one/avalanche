using System.Reflection;
using System.Runtime.Loader;

namespace Khaos.Avalanche.Hosting;

internal class JobHandle : IDisposable
{
    public Guid Id { get; }
    public IJob Instance { get; }

    private readonly Assembly _assembly;
    private readonly AssemblyLoadContext _assemblyLoadContext;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private bool _isStarted;
    private bool _isDisposed;
    private Task _task;
    
    public event Action<JobHandle> Completed;

    public JobHandle(Guid id, SerializedJobStartInfo startInfo)
    {
        Id = id;
        
        _assemblyLoadContext = new AssemblyLoadContext($"jobs-{Id}", isCollectible: true);

        using var ms = new MemoryStream(startInfo.Assembly);
        _assembly = _assemblyLoadContext.LoadFromStream(ms);

        var targetType = _assembly
            .GetExportedTypes()
            .SingleOrDefault(
                type => type.FullName == startInfo.EntryType
                    && type.IsAssignableTo(typeof(IJob))
                    && !type.IsAbstract
                    && !type.IsInterface
                    && !type.IsGenericType);

        if (targetType is null)
        {
            throw new Exception("Failed to construct a task.");
        }

        if (Activator.CreateInstance(targetType) is not IJob targetTypeInstance)
        {
            throw new Exception("Failed to construct a task.");
        }

        Instance = targetTypeInstance;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Begin(CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
        {
            throw new InvalidOperationException("Cannot begin a disposed job.");
        }
        
        if (_isStarted)
        {
            throw new InvalidOperationException("Cannot start already started operation.");
        }

        var selfToken = _cancellationTokenSource.Token;
        var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(selfToken, cancellationToken);

        _isStarted = true;
        _task = Task.Run(() => Instance.Run(linkedTokenSource.Token), linkedTokenSource.Token)
            .ContinueWith(task => Completed(this), linkedTokenSource.Token);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            throw new InvalidOperationException("Cannot dispose already disposed object.");
        }

        _cancellationTokenSource.Cancel();

        if (_isStarted)
        {
            if (!_task.IsCompleted)
            {
                _task.Wait(TimeSpan.FromMilliseconds(100));
            }

            if (!_task.IsCompleted)
            {
                throw new Exception("Failed to terminate a task in given time.");
            }
        }

        _assemblyLoadContext.Unload();
        _isDisposed = true;

        GC.SuppressFinalize(this);
    }

    public JobStatus Status
    {
        get
        {
            if (!_isStarted)
            {
                return new(TaskStatus.WaitingToRun, null);
            }

            return new(_task.Status, ExceptionInfo.FromException(_task.Exception));
        }
    }
}