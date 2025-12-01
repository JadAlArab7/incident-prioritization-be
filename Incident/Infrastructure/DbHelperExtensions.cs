using Npgsql;

namespace Incident.Infrastructure;

public static class DbHelperExtensions
{
    public static string GetConnectionString(this IDbHelper dbHelper)
    {
        // This assumes the DbHelper has access to the connection string
        // You may need to modify your IDbHelper interface to expose this
        if (dbHelper is DbHelper concreteHelper)
        {
            return concreteHelper.ConnectionString;
        }
        throw new InvalidOperationException("Unable to get connection string from DbHelper");
    }
}