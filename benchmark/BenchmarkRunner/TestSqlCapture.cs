using System;
using System.Threading.Tasks;
using BenchmarkRunner.Utils;
using MysqlEFCoreImpl;

namespace BenchmarkRunner;

public class TestSqlCapture
{
    public static async Task CaptureEfCoreSql()
    {
        var connectionString = Config.GetMysqlConnectionString();
        using var dbContext = new SalesDbContext(connectionString);
        var queries = new Queries(dbContext);
        
        var args = new Queries.GetCustomerOrdersArgs(
            CustomerId: 1,
            Offset: 0,
            Limit: 100
        );
        
        // This will print the SQL
        var result = await queries.GetCustomerOrders(args);
        
        Console.WriteLine($"Retrieved {result.Count} rows");
    }
}

