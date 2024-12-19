using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;

namespace UserSvc.Persistence;

internal static class DbUtils
{
    // In real world should be read from settings
    private const string ConnString = "DataSource=InMemoryDb;Mode=Memory;Cache=Shared";

    public static DbConnection AddDatabaseServices(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(new DateHandler()); // add DateOnly handler
        
        // This is the main db connection
        var conn = new SqliteConnection(ConnString);
        conn.Open(); // keep it open so the in-memory DB keeps the contents
        new UserRepository(conn).InitRepository(); // don't dispose so the connection remains open

        // This is the request scoped db connection; let the client open it when needed
        services.AddScoped<DbConnection>(_ => new SqliteConnection(ConnString));
        services.AddScoped<UserRepository>();
        
        return conn;
    }

}

/// <summary>
/// Converter between string and DateOnly classes
/// </summary>
file class DateHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value) =>
        DateOnly.Parse(value.ToString()!);

    public override void SetValue(IDbDataParameter parameter, DateOnly value) =>
        parameter.Value = value.ToString();
}
