namespace Khaos.Avalanche;

public record FileContent(string FileName, ReadOnlyMemory<byte> Content)
{
    public async ValueTask WriteLocal(bool createDirectories = false, CancellationToken cancellationToken = default)
    {
        if (createDirectories)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName)!);
        }

        await using var writer = File.Open(FileName, FileMode.Create);
        await writer.WriteAsync(Content, cancellationToken);
    }
}