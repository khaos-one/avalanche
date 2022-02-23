using System.Runtime.CompilerServices;

using Google.Cloud.BigQuery.V2;

namespace Khaos.Avalanche.BigQuery;

public class BigQuerySource : Source<BigQueryRow>
{
    private readonly BigQueryClient _client;
    private readonly string _query;

    public BigQuerySource(BigQueryClient client, string query)
    {
        _client = client;
        _query = query;
    }

    protected override async IAsyncEnumerable<BigQueryRow> GetEnumerable(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = await _client.ExecuteQueryAsync(
            _query,
            Array.Empty<BigQueryParameter>(),
            new QueryOptions(),
            new GetQueryResultsOptions { PageSize = 10_000, Timeout = TimeSpan.FromMinutes(15) },
            cancellationToken);

        await foreach (var row in result.GetRowsAsync().WithCancellation(cancellationToken))
        {
            yield return row;
        }
    }
}