using System.ComponentModel.DataAnnotations;

namespace BTCPayServer.Plugins.ApparelStore.Models;

public class Order
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string StoreId { get; set; } = string.Empty;

    public string? BTCPayInvoiceId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "USD";

    // Shipping information
    [Required]
    [MaxLength(200)]
    public string ShippingName { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string ShippingAddress { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ShippingCity { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ShippingState { get; set; } = string.Empty;

    [MaxLength(20)]
    public string ShippingZipCode { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ShippingCountry { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ShippingEmail { get; set; }

    [MaxLength(50)]
    public string? ShippingPhone { get; set; }

    public string? CustomerNotes { get; set; }

    // Printful integration
    public string? PrintfulOrderId { get; set; }
    public bool FulfilledByPrintful { get; set; } = false;
    public DateTime? FulfilledAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string OrderId { get; set; } = string.Empty;

    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public string ProductVariantId { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Size { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Color { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal UnitPrice { get; set; }

    [Required]
    public decimal TotalPrice { get; set; }

    public string? ProductImageUrl { get; set; }

    // Navigation properties
    public virtual Order? Order { get; set; }
    public virtual Product? Product { get; set; }
    public virtual ProductVariant? ProductVariant { get; set; }
}

public enum OrderStatus
{
    Pending,
    PaymentReceived,
    Processing,
    Shipped,
    Completed,
    Cancelled,
    Refunded
}
