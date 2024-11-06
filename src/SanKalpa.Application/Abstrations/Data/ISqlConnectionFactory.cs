using System.Data;

namespace SanKalpa.Application.Abstrations.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
