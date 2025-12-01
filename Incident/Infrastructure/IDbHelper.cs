using Npgsql;

namespace Incident.Infrastructure;

public interface IDbHelper
{
    Task<NpgsqlDataReader> ExecuteReaderAsync(string sql, CancellationToken ct = default, params NpgsqlParameter[] parameters);
    Task<int> ExecuteNonQueryAsync(string sql, CancellationToken ct = default, params NpgsqlParameter[] parameters);
    Task<T?> ExecuteScalarAsync<T>(string sql, CancellationToken ct = default, params NpgsqlParameter[] parameters);
    Task<NpgsqlConnection> GetConnectionAsync(CancellationToken ct = default);
}