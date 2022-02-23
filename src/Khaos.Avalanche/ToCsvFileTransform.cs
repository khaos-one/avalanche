using Csv;

namespace Khaos.Avalanche;

public class ToCsvFileTransform : ITransform<string[], string>
{
    private readonly string[] _header;
    private readonly char _separator;
    private readonly List<string[]> _lines = new();

    public ToCsvFileTransform(IEnumerable<string> header, char separator = ';')
    {
        _header = header.ToArray();
        _separator = separator;
    }
    
    public async IAsyncEnumerable<string> Expand(IAsyncEnumerable<string[]> enumerable)
    {
        await foreach (var element in enumerable)
        {
            _lines.Add(element);
        }

        var csvFileContent = CsvWriter.WriteToText(_header, _lines, _separator);

        yield return csvFileContent;
    }
}