using SqliteSqlcImpl;

namespace BenchmarkRunner.Utils;

public class SqliteDatabaseSeeder(string connectionString)
{
    private const int BatchSize = 100;

    private readonly QuerySql _sqlc = new(connectionString);

    public async Task SeedAsync(
        int customerCount,
        int productsPerCategory,
        int ordersPerCustomer,
        int itemsPerOrder)
    {
        var customers = await SeedCustomersAsync(customerCount);
        if (customers.Count > 0)
            Console.WriteLine($"Seeded {customers.Count} customers");

        var products = await SeedProductsAsync(productsPerCategory);
        if (products.Count > 0)
            Console.WriteLine($"Seeded {products.Count} products");

        var orders = await SeedOrdersAsync(customers, ordersPerCustomer);
        if (orders.Count > 0)
            Console.WriteLine($"Seeded {orders.Count} orders");

        if (orders.Count > 0 && products.Count > 0 && itemsPerOrder > 0)
        {
            await SeedOrderItemsAsync(orders, products, itemsPerOrder);
            Console.WriteLine($"Seeded order items");
        }
    }

    private async Task<List<int>> SeedCustomersAsync(int count)
    {
        var customers = new List<QuerySql.AddCustomersArgs>();
        for (int i = 0; i < count; i++)
        {
            var registeredAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 365));
            customers.Add(new QuerySql.AddCustomersArgs(
                Name: $"Customer {i + 1}",
                Email: $"customer{i + 1}@example.com",
                Phone: $"+1-555-{1000 + i:D4}",
                Address: $"{Random.Shared.Next(100, 9999)} Main St, City {i % 10}",
                RegisteredAt: registeredAt.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF")
            ));
        }

        await Helpers.InsertInBatchesAsync(customers, BatchSize, _sqlc.AddCustomersAsync);
        var customerIds = await _sqlc.GetCustomerIdsAsync(new QuerySql.GetCustomerIdsArgs(Limit: count));
        return [.. customerIds.Select(r => r.CustomerId)];
    }

    private async Task<List<int>> SeedProductsAsync(int perCategory)
    {
        var categories = new[] { "Electronics", "Clothing", "Books", "Toys", "Home", "Sports", "Beauty" };
        var products = new List<QuerySql.AddProductsArgs>();

        foreach (var category in categories)
        {
            for (int i = 0; i < perCategory; i++)
            {
                products.Add(new QuerySql.AddProductsArgs(
                    Name: $"{category} Product {i + 1}",
                    Category: category,
                    UnitPrice: (decimal)(Random.Shared.NextDouble() * 1000 + 10),
                    StockQuantity: Random.Shared.Next(0, 1000),
                    Description: $"Description for {category} Product {i + 1}"
                ));
            }
        }

        await Helpers.InsertInBatchesAsync(products, BatchSize, _sqlc.AddProductsAsync);
        var productIds = await _sqlc.GetProductIdsAsync(new QuerySql.GetProductIdsArgs(Limit: products.Count));
        return [.. productIds.Select(r => r.ProductId)];
    }

    private async Task<List<int>> SeedOrdersAsync(List<int> customerIds, int ordersPerCustomer)
    {
        var orderStates = new[] { "Pending", "Delivered", "Cancelled" };
        var orders = new List<QuerySql.AddOrdersArgs>();

        foreach (var customerId in customerIds)
        {
            for (int i = 0; i < ordersPerCustomer; i++)
            {
                orders.Add(new QuerySql.AddOrdersArgs(
                    CustomerId: customerId,
                    OrderState: orderStates[Random.Shared.Next(orderStates.Length)],
                    TotalAmount: (decimal)(Random.Shared.NextDouble() * 5000 + 10)
                ));
            }
        }

        await Helpers.InsertInBatchesAsync(orders, BatchSize, _sqlc.AddOrdersAsync);
        var orderIds = await _sqlc.GetOrderIdsAsync(new QuerySql.GetOrderIdsArgs(Limit: orders.Count));
        return orderIds.Select(r => r.OrderId).ToList();
    }

    private async Task SeedOrderItemsAsync(List<int> orderIds, List<int> productIds, int itemsPerOrder)
    {
        var orderItems = new List<QuerySql.AddOrderItemsArgs>();
        foreach (var orderId in orderIds)
        {
            for (int i = 0; i < itemsPerOrder; i++)
            {
                orderItems.Add(new QuerySql.AddOrderItemsArgs(
                    OrderId: orderId,
                    ProductId: productIds[Random.Shared.Next(1, productIds.Count)],
                    Quantity: Random.Shared.Next(1, 10),
                    UnitPrice: (decimal)(Random.Shared.NextDouble() * 100 + 5)
                ));
            }
        }

        await Helpers.InsertInBatchesAsync(orderItems, BatchSize, _sqlc.AddOrderItemsAsync);
    }
}