using System.Runtime.CompilerServices;

using Dapper;

using Npgsql;

namespace Khaos.Avalanche.Npgsql;

public class NpgsqlQuerySource<T> : Source<T>
{
    private readonly NpgsqlConnection _connection;
    private readonly string _query;
    private readonly object? _queryParameter;

    public NpgsqlQuerySource(NpgsqlConnection connection, string query, object? queryParameter = null)
    {
        _connection = connection;
        _query = query;
        _queryParameter = queryParameter;
    }

    protected override async IAsyncEnumerable<T> GetEnumerable(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var reader = await _connection.ExecuteReaderAsync(_query, _queryParameter);
        var parseRow = reader.GetRowParser<T>();

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return parseRow(reader);
        }
    }
}