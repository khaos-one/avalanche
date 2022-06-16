using Cassandra;

namespace Khaos.Avalanche.Cassandra;

public class CassandraQuerySource : Source<Row>
{
    private readonly ISession _session;
    private readonly string _query;
    private readonly object[] _queryParameters;

    public CassandraQuerySource(ISession session, string query, params object[] queryParameters)
    {
        _session = session;
        _query = query;
        _queryParameters = queryParameters;
    }

    protected override async IAsyncEnumerable<Row> GetEnumerable(CancellationToken cancellationToken = default)
    {
        var statement = await _session.PrepareAsync(_query);
        var resultSet = await _session.ExecuteAsync(
            statement.Bind(_queryParameters));

        foreach (var row in resultSet)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            yield return row;
        }
    }
}