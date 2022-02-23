using Dapper;

using Npgsql;

namespace Khaos.Avalanche;

public class NpgsqlQuerySink : Sink<object>
{
    private readonly NpgsqlConnection _connection;
    private readonly string _query;

    public NpgsqlQuerySink(NpgsqlConnection connection, string query)
    {
        _connection = connection;
        _query = query;
    }

    protected override async Task SinkElement(object element, CancellationToken cancellationToken)
    {
        await _connection.ExecuteAsync(_query, element);
    }
}