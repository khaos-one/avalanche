namespace Khaos.Avalanche;

public class LocalFileSource : Source<FileContent>
{
    private readonly string _directoryPath;
    private readonly string _filePattern;

    public LocalFileSource(string directoryPath, string filePattern)
    {
        _directoryPath = directoryPath;
        _filePattern = filePattern;
    }

    protected override async IAsyncEnumerable<FileContent> GetEnumerable(CancellationToken cancellationToken = default)
    {
        foreach (var file in Directory.GetFiles(_directoryPath, _filePattern))
        {
            var content = await File.ReadAllBytesAsync(file, cancellationToken);

            yield return new FileContent(file, content);
        }
    }
}