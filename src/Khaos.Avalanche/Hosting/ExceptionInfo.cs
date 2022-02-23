namespace Khaos.Avalanche.Hosting;

public record ExceptionInfo(string Type, string Message, string? StackTrace)
{
    public static ExceptionInfo? FromException(Exception? exception) =>
        exception is not null 
            ? new(exception.GetType().FullName!, exception.Message, exception.StackTrace) 
            : null;
}