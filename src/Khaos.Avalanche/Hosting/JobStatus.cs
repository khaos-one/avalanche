namespace Khaos.Avalanche.Hosting;

public record JobStatus(string TypeName, TaskStatus Status, ExceptionInfo? Exception);