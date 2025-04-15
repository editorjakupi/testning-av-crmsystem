using server.Enums;
using Microsoft.Extensions.Configuration;

namespace server;

using Npgsql;

public class Database
{
    private readonly string _connectionString;
    private NpgsqlDataSource _connection;

    public NpgsqlDataSource Connection()
    {
        return _connection;
    }

    public Database(IConfiguration configuration)
    {
        // Hämta anslutningssträngen från appsettings.json
        _connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(_connectionString))
        {
            // Fallback till hårdkodade värden om anslutningssträngen inte finns
            string host = "217.76.56.135";
            string port = "5432";
            string username = "postgres";
            string password = "FlickeringCustomerMoves29";
            string database = "crmdb";

            _connectionString = $"Host={host};Port={port};Username={username};Password={password};Database={database}";
        }

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
        dataSourceBuilder.MapEnum<Role>("role");
        dataSourceBuilder.MapEnum<IssueState>("issue_state");
        dataSourceBuilder.MapEnum<Sender>("sender");
        _connection = dataSourceBuilder.Build();
    }
}