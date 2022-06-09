using Dapper;

using Npgsql;

namespace Khaos.Avalanche.Npgsql;

public class NpgsqlFastCopySink : Sink<object[]>
{
    private readonly NpgsqlConnection _connection;
    private readonly string _tableFieldsSpecification;
    
    private NpgsqlBinaryImporter? _importer;

    public NpgsqlFastCopySink(NpgsqlConnection connection, string tableFieldsSpecification)
    {
        _connection = connection;
        _tableFieldsSpecification = tableFieldsSpecification;
    }

    protected override async Task Begin(CancellationToken cancellationToken)
    {
        await _connection.OpenAsync(cancellationToken);
        _importer = await _connection.BeginBinaryImportAsync(
            $"COPY {_tableFieldsSpecification} FROM STDIN (FORMAT BINARY)",
            cancellationToken);
    }

    protected override async Task SinkElement(object[] element, CancellationToken cancellationToken)
    {
        await _importer!.WriteRowAsync(cancellationToken);

        foreach (var field in element)
        {
            await _importer.WriteAsync(field, cancellationToken);
        }
    }

    protected override async Task End(CancellationToken cancellationToken)
    {
        await _importer.CompleteAsync(cancellationToken);
        await _connection.CloseAsync();

        await _importer.DisposeAsync();
        _importer = null;
    }
}