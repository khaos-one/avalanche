using Csv;

namespace Khaos.Avalanche;

public static class CsvExtensions
{
    public static Dictionary<string, string> ToDictionary(this ICsvLine line)
    {
        var result = new Dictionary<string, string>(line.ColumnCount);
        
        for (var i = 0; i < line.ColumnCount; i++)
        {
            result.Add(line.Headers[i], line.Values[i]);
        }

        return result;
    }
}