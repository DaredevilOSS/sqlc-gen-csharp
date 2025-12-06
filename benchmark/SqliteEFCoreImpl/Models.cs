using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqliteEFCoreImpl;

[Table("customers")]
public class Customer
{
    [Key]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("phone")]
    public string Phone { get; set; } = string.Empty;

    [Column("address")]
    public string? Address { get; set; }

    [Required]
    [Column("registered_at")]
    public DateTime RegisteredAt { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

[Table("products")]
public class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("category")]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    [Required]
    [Column("stock_quantity")]
    public int StockQuantity { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("added_at")]
    public DateTime AddedAt { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

[Table("orders")]
public class Order
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Required]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Required]
    [Column("ordered_at")]
    public DateTime OrderedAt { get; set; }

    [Required]
    [Column("order_state")]
    public string OrderState { get; set; } = string.Empty;

    [Required]
    [Column("total_amount")]
    public decimal TotalAmount { get; set; }

    [ForeignKey("CustomerId")]
    public Customer Customer { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

[Table("order_items")]
public class OrderItem
{
    [Key]
    [Column("order_item_id")]
    public int OrderItemId { get; set; }

    [Required]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Required]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Required]
    [Column("quantity")]
    public int Quantity { get; set; }

    [Required]
    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    [ForeignKey("OrderId")]
    public Order Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;
}