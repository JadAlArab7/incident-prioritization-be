using Npgsql;

namespace Incident.Infrastructure;

public class DbHelper : IDbHelper
{
    private readonly string _connectionString;

    public DbHelper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<NpgsqlConnection> GetConnectionAsync(CancellationToken ct = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        return connection;
    }

    public async Task<NpgsqlDataReader> ExecuteReaderAsync(string sql, CancellationToken ct = default, params NpgsqlParameter[] parameters)
    {
        var connection = await GetConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection);
        
        if (parameters != null && parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        // CommandBehavior.CloseConnection ensures connection is closed when reader is disposed
        return await command.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection, ct);
    }

    public async Task<int> ExecuteNonQueryAsync(string sql, CancellationToken ct = default, params NpgsqlParameter[] parameters)
    {
        await using var connection = await GetConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection);
        
        if (parameters != null && parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        return await command.ExecuteNonQueryAsync(ct);
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, CancellationToken ct = default, params NpgsqlParameter[] parameters)
    {
        await using var connection = await GetConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection);
        
        if (parameters != null && parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        var result = await command.ExecuteScalarAsync(ct);
        
        if (result == null || result == DBNull.Value)
        {
            return default;
        }

        return (T)result;
    }
}