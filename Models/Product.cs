using System.ComponentModel.DataAnnotations;

namespace BTCPayServer.Plugins.ApparelStore.Models;

public class Product
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string StoreId { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public decimal BasePrice { get; set; }

    public string Currency { get; set; } = "USD";

    [MaxLength(100)]
    public string? PrintfulProductId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}

public class ProductVariant
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string ProductId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Size { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Color { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ColorHex { get; set; }

    public decimal PriceAdjustment { get; set; } = 0;

    public int StockQuantity { get; set; } = 999;

    [MaxLength(100)]
    public string? PrintfulVariantId { get; set; }

    public bool IsAvailable { get; set; } = true;

    // Navigation property
    public virtual Product? Product { get; set; }
}

public class ProductImage
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? ColorVariant { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsPrimary { get; set; } = false;

    // Navigation property
    public virtual Product? Product { get; set; }
}
