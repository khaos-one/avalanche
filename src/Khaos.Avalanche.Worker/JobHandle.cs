using System.Reflection;
using System.Runtime.Loader;

using Khaos.Avalanche.Remoting;

namespace Khaos.Avalanche.Worker;

public record JobHandle(
    Guid Id,
    Assembly Assembly,
    AssemblyLoadContext AssemblyLoadContext,
    IJob Instance,
    CancellationTokenSource CancellationTokenSource)
{
    private Task? _task;
    
    public static JobHandle FromSerializedJobStartInfo(SerializedJobStartInfo startInfo)
    {
        var id = Guid.NewGuid();
        var loadContext = new AssemblyLoadContext($"jobs-{id}", isCollectible: true);

        using var ms = new MemoryStream(startInfo.Assembly);
        var assembly = loadContext.LoadFromStream(ms);

        var targetType = assembly
            .GetExportedTypes()
            .SingleOrDefault(
                type => type.FullName == startInfo.EntryType
                    && type.IsAssignableTo(typeof(IJob))
                    && !type.IsAbstract
                    && !type.IsInterface
                    && !type.IsGenericType);

        if (targetType is null)
        {
            throw new Exception();
        }

        if (Activator.CreateInstance(targetType) is not IJob targetTypeInstance)
        {
            throw new Exception();
        }

        return new JobHandle(
            id,
            assembly,
            loadContext,
            targetTypeInstance,
            new CancellationTokenSource());
    }

    public void RunDeferred()
    {
        if (_task is not null)
        {
            throw new InvalidOperationException();
        }

        _task = Task.Run(() => Instance.Run(CancellationTokenSource.Token), CancellationTokenSource.Token);
    }

    public TaskStatus Status
    {
        get
        {
            if (_task is null)
            {
                return TaskStatus.WaitingToRun;
            }

            return _task.Status;
        }
    }

    public AggregateException? Exception => _task?.Exception;
}