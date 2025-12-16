using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostgresEFCoreImpl;

[Table("customers", Schema = "sales")]
public class Customer
{
    [Key]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("phone")]
    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [Column("address")]
    [MaxLength(255)]
    public string? Address { get; set; }

    [Required]
    [Column("registered_at")]
    public DateTime RegisteredAt { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

[Table("products", Schema = "sales")]
public class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("category")]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Column("unit_price", TypeName = "decimal(10,2)")]
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

[Table("orders", Schema = "sales")]
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
    [MaxLength(10)]
    public string OrderState { get; set; } = string.Empty;

    [Required]
    [Column("total_amount", TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

[Table("order_items", Schema = "sales")]
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
    [Column("unit_price", TypeName = "decimal(10,2)")]
    public decimal UnitPrice { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;
}