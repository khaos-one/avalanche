using System.Text;

namespace Khaos.Avalanche;

public class LocalFileTextLinesSink : ISink<string>
{
    private readonly string _path;

    private IAsyncEnumerable<string>? _enumerable;

    public LocalFileTextLinesSink(string path)
    {
        _path = path;
    }
    
    public void SetEnumerable(IAsyncEnumerable<string> enumerable)
    {
        _enumerable = enumerable;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        var writer = default(FileStream);

        try
        {
            await foreach (var element in _enumerable!.WithCancellation(cancellationToken))
            {
                writer ??= File.Open(_path, FileMode.Create);

                await writer.WriteAsync(
                    Encoding.UTF8.GetBytes(element + Environment.NewLine), 
                    cancellationToken);
            }
        }
        finally
        {
            if (writer is not null)
            {
                writer.Close();
                await writer.DisposeAsync();
            }
        }
    }
}