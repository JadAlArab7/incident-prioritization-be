using Npgsql;

namespace Incident.Infrastructure;

public interface IDbHelper
{
    Task<NpgsqlConnection> GetConnectionAsync();
    Task<T?> QuerySingleOrDefaultAsync<T>(string sql, Func<NpgsqlDataReader, T> mapper, params NpgsqlParameter[] parameters);
    Task<List<T>> QueryAsync<T>(string sql, Func<NpgsqlDataReader, T> mapper, params NpgsqlParameter[] parameters);
    Task<int> ExecuteAsync(string sql, params NpgsqlParameter[] parameters);
    Task<T?> ExecuteScalarAsync<T>(string sql, params NpgsqlParameter[] parameters);
}