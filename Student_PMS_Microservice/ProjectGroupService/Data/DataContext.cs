using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace ProjectGroupService.Data;

public class DataContext(IConfiguration configuration)
{

    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    public SqlConnection CreateConnection() => new(_connectionString);
}
