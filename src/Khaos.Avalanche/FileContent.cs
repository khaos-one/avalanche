namespace Khaos.Avalanche;

public record FileContent(string FileName, ReadOnlyMemory<byte> Content);