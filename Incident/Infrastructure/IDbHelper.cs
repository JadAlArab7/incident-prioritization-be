using System.Data;

namespace Incident.Infrastructure;

public interface IDbHelper
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null);
    Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null);
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null);
    Task<T> QuerySingleAsync<T>(string sql, object? parameters = null);
    Task<T> QueryFirstAsync<T>(string sql, object? parameters = null);
    Task<int> ExecuteAsync(string sql, object? parameters = null);
    Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null);
    IDbConnection CreateConnection();
}