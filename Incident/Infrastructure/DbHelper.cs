using Npgsql;

namespace Incident.Infrastructure;

public class DbHelper : IDbHelper
{
    private readonly string _connectionString;

    public DbHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<NpgsqlConnection> GetConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, Func<NpgsqlDataReader, T> mapper, params NpgsqlParameter[] parameters)
    {
        await using var connection = await GetConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        
        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        await using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return mapper(reader);
        }

        return default;
    }

    public async Task<List<T>> QueryAsync<T>(string sql, Func<NpgsqlDataReader, T> mapper, params NpgsqlParameter[] parameters)
    {
        var results = new List<T>();

        await using var connection = await GetConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        
        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            results.Add(mapper(reader));
        }

        return results;
    }

    public async Task<int> ExecuteAsync(string sql, params NpgsqlParameter[] parameters)
    {
        await using var connection = await GetConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        
        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        return await command.ExecuteNonQueryAsync();
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, params NpgsqlParameter[] parameters)
    {
        await using var connection = await GetConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        
        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        var result = await command.ExecuteScalarAsync();
        
        if (result == null || result == DBNull.Value)
        {
            return default;
        }

        return (T)result;
    }
}