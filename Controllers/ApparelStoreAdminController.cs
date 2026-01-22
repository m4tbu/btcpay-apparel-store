using BTCPayServer.Abstractions.Constants;
using BTCPayServer.Client;
using BTCPayServer.Plugins.ApparelStore.Data;
using BTCPayServer.Plugins.ApparelStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTCPayServer.Plugins.ApparelStore.Controllers;

[Route("apps/{appId}/apparel/admin")]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.Cookie, Policy = Policies.CanModifyStoreSettings)]
public class ApparelStoreAdminController : Controller
{
    private readonly ApparelStoreDbContext _dbContext;

    public ApparelStoreAdminController(ApparelStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Admin dashboard
    [HttpGet("")]
    public async Task<IActionResult> Index(string appId)
    {
        var products = await _dbContext.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Where(p => p.StoreId == appId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        ViewBag.AppId = appId;
        return View(products);
    }

    // Create product form
    [HttpGet("product/create")]
    public IActionResult CreateProduct(string appId)
    {
        ViewBag.AppId = appId;
        return View();
    }

    // Create product POST
    [HttpPost("product/create")]
    public async Task<IActionResult> CreateProduct(string appId, [FromForm] ProductFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AppId = appId;
            return View(model);
        }

        var product = new Product
        {
            StoreId = appId,
            Name = model.Name,
            Description = model.Description,
            BasePrice = model.BasePrice,
            Currency = model.Currency,
            PrintfulProductId = model.PrintfulProductId,
            IsActive = model.IsActive
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Product '{product.Name}' created successfully";
        return RedirectToAction("EditProduct", new { appId, productId = product.Id });
    }

    // Edit product form
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> EditProduct(string appId, string productId)
    {
        var product = await _dbContext.Products
            .Include(p => p.Variants)
            .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
            .FirstOrDefaultAsync(p => p.Id == productId && p.StoreId == appId);

        if (product == null)
        {
            return NotFound();
        }

        ViewBag.AppId = appId;
        return View(product);
    }

    // Update product
    [HttpPost("product/{productId}")]
    public async Task<IActionResult> UpdateProduct(string appId, string productId, [FromForm] ProductFormModel model)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.StoreId == appId);

        if (product == null)
        {
            return NotFound();
        }

        product.Name = model.Name;
        product.Description = model.Description;
        product.BasePrice = model.BasePrice;
        product.Currency = model.Currency;
        product.PrintfulProductId = model.PrintfulProductId;
        product.IsActive = model.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Product updated successfully";
        return RedirectToAction("EditProduct", new { appId, productId });
    }

    // Delete product
    [HttpPost("product/{productId}/delete")]
    public async Task<IActionResult> DeleteProduct(string appId, string productId)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.StoreId == appId);

        if (product == null)
        {
            return NotFound();
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Product deleted successfully";
        return RedirectToAction("Index", new { appId });
    }

    // Add variant
    [HttpPost("product/{productId}/variant")]
    public async Task<IActionResult> AddVariant(string appId, string productId, [FromForm] ProductVariantFormModel model)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.StoreId == appId);

        if (product == null)
        {
            return NotFound();
        }

        var variant = new ProductVariant
        {
            ProductId = productId,
            Size = model.Size,
            Color = model.Color,
            ColorHex = model.ColorHex,
            PriceAdjustment = model.PriceAdjustment,
            StockQuantity = model.StockQuantity,
            PrintfulVariantId = model.PrintfulVariantId,
            IsAvailable = model.IsAvailable
        };

        _dbContext.ProductVariants.Add(variant);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Variant added successfully";
        return RedirectToAction("EditProduct", new { appId, productId });
    }

    // Delete variant
    [HttpPost("variant/{variantId}/delete")]
    public async Task<IActionResult> DeleteVariant(string appId, string variantId)
    {
        var variant = await _dbContext.ProductVariants
            .Include(v => v.Product)
            .FirstOrDefaultAsync(v => v.Id == variantId && v.Product!.StoreId == appId);

        if (variant == null)
        {
            return NotFound();
        }

        var productId = variant.ProductId;
        _dbContext.ProductVariants.Remove(variant);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Variant deleted successfully";
        return RedirectToAction("EditProduct", new { appId, productId });
    }

    // Add image
    [HttpPost("product/{productId}/image")]
    public async Task<IActionResult> AddImage(string appId, string productId, [FromForm] ProductImageFormModel model)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.StoreId == appId);

        if (product == null)
        {
            return NotFound();
        }

        var image = new ProductImage
        {
            ProductId = productId,
            ImageUrl = model.ImageUrl,
            ColorVariant = model.ColorVariant,
            DisplayOrder = model.DisplayOrder,
            IsPrimary = model.IsPrimary
        };

        _dbContext.ProductImages.Add(image);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Image added successfully";
        return RedirectToAction("EditProduct", new { appId, productId });
    }

    // Delete image
    [HttpPost("image/{imageId}/delete")]
    public async Task<IActionResult> DeleteImage(string appId, string imageId)
    {
        var image = await _dbContext.ProductImages
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == imageId && i.Product!.StoreId == appId);

        if (image == null)
        {
            return NotFound();
        }

        var productId = image.ProductId;
        _dbContext.ProductImages.Remove(image);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Image deleted successfully";
        return RedirectToAction("EditProduct", new { appId, productId });
    }

    // Orders list
    [HttpGet("orders")]
    public async Task<IActionResult> Orders(string appId)
    {
        var orders = await _dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.StoreId == appId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        ViewBag.AppId = appId;
        return View(orders);
    }

    // Order detail
    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> OrderDetail(string appId, string orderId)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.StoreId == appId);

        if (order == null)
        {
            return NotFound();
        }

        ViewBag.AppId = appId;
        return View(order);
    }
}

public class ProductFormModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = "USD";
    public string? PrintfulProductId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ProductVariantFormModel
{
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? ColorHex { get; set; }
    public decimal PriceAdjustment { get; set; } = 0;
    public int StockQuantity { get; set; } = 999;
    public string? PrintfulVariantId { get; set; }
    public bool IsAvailable { get; set; } = true;
}

public class ProductImageFormModel
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? ColorVariant { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimary { get; set; } = false;
}
