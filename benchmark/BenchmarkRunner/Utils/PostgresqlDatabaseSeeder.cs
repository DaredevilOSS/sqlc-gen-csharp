using Microsoft.EntityFrameworkCore;
using PostgresEFCoreImpl;

namespace BenchmarkRunner.Utils;

public class PostgresqlDatabaseSeeder(string connectionString)
{
    private readonly SalesDbContext _efCoreContext = new(connectionString);

    public async Task SeedAsync(int customerCount = 100, int productsPerCategory = 100, int ordersPerCustomer = 50, int itemsPerOrder = 3)
    {
        // Seed customers
        var customers = await SeedCustomersAsync(customerCount);
        if (customers.Count > 0)
            Console.WriteLine($"Seeded {customers.Count} customers");

        // Seed products
        var products = await SeedProductsAsync(productsPerCategory);
        if (products.Count > 0)
            Console.WriteLine($"Seeded {products.Count} products");

        // Seed orders and order items
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
        var customers = new List<Customer>();
        var random = new Random(42); // Fixed seed for reproducibility

        for (int i = 0; i < count; i++)
        {
            customers.Add(new Customer
            {
                Name = $"Customer {i + 1}",
                Email = $"customer{i + 1}@example.com",
                Phone = $"+1-555-{1000 + i:D4}",
                Address = $"{random.Next(100, 9999)} Main St, City {i % 10}",
                RegisteredAt = DateTime.UtcNow.AddDays(-random.Next(0, 365))
            });
        }

        _efCoreContext.Customers.AddRange(customers);
        await _efCoreContext.SaveChangesAsync();

        return customers.Select(c => c.CustomerId).ToList();
    }

    private async Task<List<int>> SeedProductsAsync(int perCategory)
    {
        var categories = new[] { "Electronics", "Clothing", "Books", "Food", "Toys", "Home", "Sports", "Automotive", "Health", "Beauty" };
        var products = new List<Product>();
        var random = new Random(42);

        foreach (var category in categories)
        {
            for (int i = 0; i < perCategory; i++)
            {
                products.Add(new Product
                {
                    Name = $"{category} Product {i + 1}",
                    Category = category,
                    UnitPrice = (decimal)(random.NextDouble() * 1000 + 10),
                    StockQuantity = random.Next(0, 1000),
                    Description = $"Description for {category} Product {i + 1}",
                    AddedAt = DateTime.UtcNow.AddDays(-random.Next(0, 180))
                });
            }
        }

        _efCoreContext.Products.AddRange(products);
        await _efCoreContext.SaveChangesAsync();

        return products.Select(p => p.ProductId).ToList();
    }

    private async Task<List<Guid>> SeedOrdersAsync(List<int> customerIds, int ordersPerCustomer)
    {
        var orders = new List<Order>();
        var random = new Random(42);
        var orderStates = new[] { "Pending", "Delivered", "Cancelled" };

        foreach (var customerId in customerIds)
        {
            for (int i = 0; i < ordersPerCustomer; i++)
            {
                orders.Add(new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = customerId,
                    OrderState = orderStates[random.Next(orderStates.Length)],
                    TotalAmount = (decimal)(random.NextDouble() * 5000 + 10),
                    OrderedAt = DateTime.UtcNow.AddDays(-random.Next(0, 90))
                });
            }
        }

        _efCoreContext.Orders.AddRange(orders);
        await _efCoreContext.SaveChangesAsync();

        return orders.Select(o => o.OrderId).ToList();
    }

    private async Task SeedOrderItemsAsync(List<Guid> orderIds, List<int> productIds, int itemsPerOrder)
    {
        var orderItems = new List<OrderItem>();
        var random = new Random(42);

        foreach (var orderId in orderIds)
        {
            // Get the order to know its total amount
            var order = await _efCoreContext.Orders.FirstAsync(o => o.OrderId == orderId);
            var remainingAmount = order.TotalAmount;
            var selectedProducts = productIds.OrderBy(_ => random.Next()).Take(itemsPerOrder).ToList();

            for (int i = 0; i < selectedProducts.Count; i++)
            {
                var productId = selectedProducts[i];
                var product = await _efCoreContext.Products.FirstAsync(p => p.ProductId == productId);
                
                decimal unitPrice;
                int quantity;

                if (i == selectedProducts.Count - 1)
                {
                    // Last item gets the remaining amount
                    quantity = random.Next(1, 10);
                    unitPrice = remainingAmount / quantity;
                }
                else
                {
                    quantity = random.Next(1, 10);
                    unitPrice = product.UnitPrice;
                    remainingAmount -= unitPrice * quantity;
                }

                orderItems.Add(new OrderItem
                {
                    OrderItemId = Guid.NewGuid(),
                    OrderId = orderId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
            }
        }

        _efCoreContext.OrderItems.AddRange(orderItems);
        await _efCoreContext.SaveChangesAsync();
    }
}

