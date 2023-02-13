namespace Khaos.Avalanche;

public class LocalFileSink : ISink<FileContent>
{
    private readonly bool _createDirectories;
    
    private IAsyncEnumerable<FileContent>? _enumerable;

    public LocalFileSink(bool createDirectories = false)
    {
        _createDirectories = createDirectories;
    }

    public void SetEnumerable(IAsyncEnumerable<FileContent> enumerable)
    {
        _enumerable = enumerable;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        await foreach (var element in _enumerable!.WithCancellation(cancellationToken))
        {
            await element.WriteLocal(_createDirectories, cancellationToken);
        }
    }
}