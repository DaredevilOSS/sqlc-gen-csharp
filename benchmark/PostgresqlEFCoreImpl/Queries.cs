using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgresEFCoreImpl;

public class Queries(SalesDbContext dbContext, bool useTracking = false)
{
    private readonly SalesDbContext _dbContext = dbContext;
    private readonly bool _useTracking = useTracking;

    public DbContext DbContext => _dbContext;

    public record GetCustomerOrdersRow(
        Guid OrderId,
        DateTime OrderedAt,
        string OrderState,
        decimal TotalAmount,
        Guid OrderItemId,
        int Quantity,
        decimal UnitPrice,
        int ProductId,
        string ProductName,
        string ProductCategory
    );

    public record GetCustomerOrdersArgs(int CustomerId, int Offset, int Limit);

    public async Task<List<GetCustomerOrdersRow>> GetCustomerOrders(GetCustomerOrdersArgs args)
    {
        var query = _dbContext.Orders
            .Where(o => o.CustomerId == args.CustomerId)
            .OrderByDescending(o => o.OrderedAt)
            .Skip(args.Offset)
            .Take(args.Limit);

        if (!_useTracking)
            query = query.AsNoTracking();

        var results = await query
            .SelectMany(o => o.OrderItems.Select(i => new
            {
                Order = o,
                OrderItem = i,
                Product = i.Product,
            }))
            .Select(x => new GetCustomerOrdersRow(
                x.Order.OrderId,
                x.Order.OrderedAt,
                x.Order.OrderState,
                x.Order.TotalAmount,
                x.OrderItem.OrderItemId,
                x.OrderItem.Quantity,
                x.OrderItem.UnitPrice,
                x.Product.ProductId,
                x.Product.Name,
                x.Product.Category
            ))
            .ToListAsync();
        return results;
    }

    public record AddProductsArgs(string Name, string Category, decimal UnitPrice, int StockQuantity, string? Description);

    public async Task AddProducts(List<AddProductsArgs> args)
    {
        var products = args.Select(a => new Product
        {
            Name = a.Name,
            Category = a.Category,
            UnitPrice = a.UnitPrice,
            StockQuantity = a.StockQuantity,
            Description = a.Description,
            AddedAt = DateTime.UtcNow
        }).ToList();

        await _dbContext.Products.AddRangeAsync(products);
        await _dbContext.SaveChangesAsync();
    }

    public record AddOrdersArgs(int CustomerId, string OrderState, decimal TotalAmount);

    /// <summary>
    /// Bulk insert orders
    /// </summary>
    public async Task AddOrders(List<AddOrdersArgs> args)
    {
        var orders = args.Select(a => new Order
        {
            OrderId = Guid.NewGuid(),
            CustomerId = a.CustomerId,
            OrderState = a.OrderState,
            TotalAmount = a.TotalAmount,
            OrderedAt = DateTime.UtcNow
        }).ToList();

        await _dbContext.Orders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();
    }

    public record AddOrderItemsArgs(Guid OrderId, int ProductId, int Quantity, decimal UnitPrice);

    /// <summary>
    /// Bulk insert order items
    /// </summary>
    public async Task AddOrderItems(List<AddOrderItemsArgs> args)
    {
        var orderItems = args.Select(a => new OrderItem
        {
            OrderItemId = Guid.NewGuid(),
            OrderId = a.OrderId,
            ProductId = a.ProductId,
            Quantity = a.Quantity,
            UnitPrice = a.UnitPrice
        }).ToList();

        await _dbContext.OrderItems.AddRangeAsync(orderItems);
        await _dbContext.SaveChangesAsync();
    }
}