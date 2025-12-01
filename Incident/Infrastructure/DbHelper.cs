using Npgsql;

namespace Incident.Infrastructure;

public class DbHelper : IDbHelper
{
    private readonly string _connectionString;

    public string ConnectionString => _connectionString;

    public DbHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<int> ExecuteNonQueryAsync(string sql, CancellationToken ct = default, params NpgsqlParameter[] parameters)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        
        await using var command = new NpgsqlCommand(sql, connection);
        if (parameters != null && parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }
        
        return await command.ExecuteNonQueryAsync(ct);
    }

    public async Task<List<T>> ExecuteReaderAsync<T>(string sql, Func<NpgsqlDataReader, T> mapper, CancellationToken ct = default, params NpgsqlParameter[] parameters)
    {
        var results = new List<T>();
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        
        await using var command = new NpgsqlCommand(sql, connection);
        if (parameters != null && parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }
        
        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            results.Add(mapper(reader));
        }
        
        return results;
    }

    public async Task<T?> ExecuteReaderSingleAsync<T>(string sql, Func<NpgsqlDataReader, T> mapper, CancellationToken ct = default, params NpgsqlParameter[] parameters) where T : class
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        
        await using var command = new NpgsqlCommand(sql, connection);
        if (parameters != null && parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }
        
        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            return mapper(reader);
        }
        
        return null;
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, CancellationToken ct = default, params NpgsqlParameter[] parameters)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        
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