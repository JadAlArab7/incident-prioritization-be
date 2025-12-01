using System.Data;

namespace Incident.Infrastructure;

public interface IDbHelper
{
    IDbConnection CreateConnection();
}