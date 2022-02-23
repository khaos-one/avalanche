using System.Text;

using Csv;

namespace Khaos.Avalanche.Csv;

public class FromCsvFileTransform : ITransform<FileContent, IEnumerable<ICsvLine>>
{
    public IAsyncEnumerable<IEnumerable<ICsvLine>> Expand(IAsyncEnumerable<FileContent> enumerable) =>
        enumerable
            .Select(file => Encoding.UTF8.GetString(file.Content.Span))
            .Select(content => CsvReader.ReadFromText(content));
}