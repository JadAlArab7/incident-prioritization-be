using Npgsql;

namespace Incident.Infrastructure;

public interface IDbHelper
{
    string ConnectionString { get; }
    
    Task<int> ExecuteNonQueryAsync(string sql, CancellationToken ct = default, params NpgsqlParameter[] parameters);
    
    Task<List<T>> ExecuteReaderAsync<T>(string sql, Func<NpgsqlDataReader, T> mapper, CancellationToken ct = default, params NpgsqlParameter[] parameters);
    
    Task<T?> ExecuteReaderSingleAsync<T>(string sql, Func<NpgsqlDataReader, T> mapper, CancellationToken ct = default, params NpgsqlParameter[] parameters) where T : class;
    
    Task<T?> ExecuteScalarAsync<T>(string sql, CancellationToken ct = default, params NpgsqlParameter[] parameters);
}