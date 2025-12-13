using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MysqlEFCoreImpl;

public class Queries
{
    private readonly SalesDbContext _dbContext;
    private readonly bool _useTracking;

    public Queries(SalesDbContext dbContext, bool useTracking = false)
    {
        _dbContext = dbContext;
        _useTracking = useTracking;
    }

    public DbContext DbContext => _dbContext;

    // Result type for GetCustomerOrders matching SqlC output
    public record GetCustomerOrdersRow(
        long OrderId,
        DateTime OrderedAt,
        string OrderState,
        decimal TotalAmount,
        long OrderItemId,
        int Quantity,
        decimal UnitPrice,
        int ProductId,
        string ProductName,
        string ProductCategory
    );

    public record GetCustomerOrdersArgs(int CustomerId, int Offset, int Limit);

    /// <summary>
    /// Get customer orders with all order items and product details, ordered by date descending with pagination
    /// </summary>
    public async Task<List<GetCustomerOrdersRow>> GetCustomerOrders(GetCustomerOrdersArgs args)
    {
        var ordersQuery = _dbContext.Orders.AsQueryable();
        var orderItemsQuery = _dbContext.OrderItems.AsQueryable();
        var productsQuery = _dbContext.Products.AsQueryable();

        if (!_useTracking)
        {
            ordersQuery = ordersQuery.AsNoTracking();
            orderItemsQuery = orderItemsQuery.AsNoTracking();
            productsQuery = productsQuery.AsNoTracking();
        }

        var query = (from o in ordersQuery
                     join i in orderItemsQuery on o.OrderId equals i.OrderId
                     join p in productsQuery on i.ProductId equals p.ProductId
                     where o.CustomerId == args.CustomerId
                     orderby o.OrderedAt descending
                     select new GetCustomerOrdersRow(
                         o.OrderId,
                         o.OrderedAt,
                         o.OrderState,
                         o.TotalAmount,
                         i.OrderItemId,
                         i.Quantity,
                         i.UnitPrice,
                         p.ProductId,
                         p.Name,
                         p.Category
                     ))
                     .Skip(args.Offset)
                     .Take(args.Limit);

        var results = await query.ToListAsync();

        return results;
    }

    public record AddProductsArgs(string Name, string Category, decimal UnitPrice, int StockQuantity, string? Description);

    /// <summary>
    /// Bulk insert products
    /// </summary>
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
        _dbContext.ChangeTracker.Clear();
    }

    public record AddOrdersArgs(int CustomerId, string OrderState, decimal TotalAmount);

    /// <summary>
    /// Bulk insert orders
    /// </summary>
    public async Task AddOrders(List<AddOrdersArgs> args)
    {
        var orders = args.Select(a => new Order
        {
            CustomerId = a.CustomerId,
            OrderState = a.OrderState,
            TotalAmount = a.TotalAmount,
            OrderedAt = DateTime.UtcNow
        }).ToList();

        await _dbContext.Orders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();
    }

    public record AddOrderItemsArgs(long OrderId, int ProductId, int Quantity, decimal UnitPrice);

    /// <summary>
    /// Bulk insert order items
    /// </summary>
    public async Task AddOrderItems(List<AddOrderItemsArgs> args)
    {
        var orderItems = args.Select(a => new OrderItem
        {
            OrderId = a.OrderId,
            ProductId = a.ProductId,
            Quantity = a.Quantity,
            UnitPrice = a.UnitPrice
        }).ToList();

        await _dbContext.OrderItems.AddRangeAsync(orderItems);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();
    }
}