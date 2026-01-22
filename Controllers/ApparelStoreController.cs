using BTCPayServer.Abstractions.Constants;
using BTCPayServer.Client;
using BTCPayServer.Client.Models;
using BTCPayServer.Data;
using BTCPayServer.Payments;
using BTCPayServer.Plugins.ApparelStore.Data;
using BTCPayServer.Plugins.ApparelStore.Models;
using BTCPayServer.Services.Invoices;
using BTCPayServer.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTCPayServer.Plugins.ApparelStore.Controllers;

[Route("apps/{appId}/apparel")]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.Cookie, Policy = Policies.CanViewStoreSettings)]
public class ApparelStoreController : Controller
{
    private readonly ApparelStoreDbContext _dbContext;
    private readonly InvoiceRepository _invoiceRepository;
    private readonly StoreRepository _storeRepository;

    public ApparelStoreController(
        ApparelStoreDbContext dbContext,
        InvoiceRepository invoiceRepository,
        StoreRepository storeRepository)
    {
        _dbContext = dbContext;
        _invoiceRepository = invoiceRepository;
        _storeRepository = storeRepository;
    }

    // Public store front - product listing
    [HttpGet("")]
    [AllowAnonymous]
    public async Task<IActionResult> Index(string appId)
    {
        var products = await _dbContext.Products
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Where(p => p.StoreId == appId && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        ViewBag.AppId = appId;
        return View(products);
    }

    // Product detail page
    [HttpGet("product/{productId}")]
    [AllowAnonymous]
    public async Task<IActionResult> Product(string appId, string productId)
    {
        var product = await _dbContext.Products
            .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == productId && p.StoreId == appId && p.IsActive);

        if (product == null)
        {
            return NotFound();
        }

        ViewBag.AppId = appId;
        return View(product);
    }

    // Shopping cart page
    [HttpGet("cart")]
    [AllowAnonymous]
    public IActionResult Cart(string appId)
    {
        ViewBag.AppId = appId;
        return View();
    }

    // Checkout page
    [HttpGet("checkout")]
    [AllowAnonymous]
    public IActionResult Checkout(string appId)
    {
        ViewBag.AppId = appId;
        return View();
    }

    // Order confirmation page
    [HttpGet("order/{orderId}")]
    [AllowAnonymous]
    public async Task<IActionResult> OrderConfirmation(string appId, string orderId)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.StoreId == appId);

        if (order == null)
        {
            return NotFound();
        }

        ViewBag.AppId = appId;
        return View(order);
    }

    // API endpoint to get cart data
    [HttpPost("api/cart/items")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCartItems(string appId, [FromBody] List<CartItemRequest> items)
    {
        var variantIds = items.Select(i => i.VariantId).ToList();
        var variants = await _dbContext.ProductVariants
            .Include(v => v.Product)
            .ThenInclude(p => p!.Images)
            .Where(v => variantIds.Contains(v.Id))
            .ToListAsync();

        var cartItems = items.Select(item =>
        {
            var variant = variants.FirstOrDefault(v => v.Id == item.VariantId);
            if (variant?.Product == null) return null;

            var primaryImage = variant.Product.Images
                .FirstOrDefault(i => i.IsPrimary || i.ColorVariant == variant.Color)
                ?? variant.Product.Images.FirstOrDefault();

            return new
            {
                variantId = variant.Id,
                productId = variant.ProductId,
                productName = variant.Product.Name,
                size = variant.Size,
                color = variant.Color,
                quantity = item.Quantity,
                price = variant.Product.BasePrice + variant.PriceAdjustment,
                imageUrl = primaryImage?.ImageUrl,
                isAvailable = variant.IsAvailable
            };
        }).Where(i => i != null).ToList();

        return Json(cartItems);
    }

    // API endpoint to create order and BTCPay invoice
    [HttpPost("api/order/create")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateOrder(string appId, [FromBody] CreateOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate cart items
        var variantIds = request.Items.Select(i => i.VariantId).ToList();
        var variants = await _dbContext.ProductVariants
            .Include(v => v.Product)
            .Where(v => variantIds.Contains(v.Id) && v.IsAvailable)
            .ToListAsync();

        if (variants.Count != request.Items.Count)
        {
            return BadRequest("Some items are no longer available");
        }

        // Calculate total
        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            var variant = variants.FirstOrDefault(v => v.Id == item.VariantId);
            if (variant?.Product == null) continue;

            var unitPrice = variant.Product.BasePrice + variant.PriceAdjustment;
            var totalPrice = unitPrice * item.Quantity;
            totalAmount += totalPrice;

            var primaryImage = variant.Product.Images.FirstOrDefault(i => i.IsPrimary);

            orderItems.Add(new OrderItem
            {
                ProductId = variant.ProductId,
                ProductVariantId = variant.Id,
                ProductName = variant.Product.Name,
                Size = variant.Size,
                Color = variant.Color,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = totalPrice,
                ProductImageUrl = primaryImage?.ImageUrl
            });
        }

        // Create order
        var order = new Order
        {
            StoreId = appId,
            TotalAmount = totalAmount,
            Currency = variants.First().Product?.Currency ?? "USD",
            ShippingName = request.ShippingName,
            ShippingAddress = request.ShippingAddress,
            ShippingCity = request.ShippingCity,
            ShippingState = request.ShippingState,
            ShippingZipCode = request.ShippingZipCode,
            ShippingCountry = request.ShippingCountry,
            ShippingEmail = request.ShippingEmail,
            ShippingPhone = request.ShippingPhone,
            CustomerNotes = request.CustomerNotes,
            Items = orderItems,
            Status = OrderStatus.Pending
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Create BTCPay invoice
        try
        {
            var store = await _storeRepository.FindStore(appId);
            if (store == null)
            {
                return BadRequest("Store not found");
            }

            // Build order description
            var orderDescription = $"Order #{order.Id.Substring(0, 8)} - {string.Join(", ", orderItems.Select(i => $"{i.ProductName} ({i.Color}/{i.Size}) x{i.Quantity}"))}";

            var invoiceRequest = new CreateInvoiceRequest
            {
                Amount = totalAmount,
                Currency = order.Currency,
                Checkout = new InvoiceDataBase.CheckoutOptions
                {
                    RedirectURL = Url.Action("OrderConfirmation", "ApparelStore", new { appId, orderId = order.Id }, Request.Scheme),
                    RedirectAutomatically = false
                }
            };

            // Add metadata
            invoiceRequest.Metadata = new JObject
            {
                ["orderId"] = order.Id,
                ["orderType"] = "apparel",
                ["itemDesc"] = orderDescription,
                ["physical"] = true,
                ["buyerName"] = order.ShippingName,
                ["buyerEmail"] = order.ShippingEmail ?? "",
                ["buyerAddress1"] = order.ShippingAddress,
                ["buyerCity"] = order.ShippingCity,
                ["buyerState"] = order.ShippingState,
                ["buyerZip"] = order.ShippingZipCode,
                ["buyerCountry"] = order.ShippingCountry,
                ["buyerPhone"] = order.ShippingPhone ?? ""
            };

            var invoice = await _invoiceRepository.CreateInvoiceAsync(store.Id, invoiceRequest, HttpContext.Request.GetAbsoluteRoot());

            // Update order with invoice ID
            order.BTCPayInvoiceId = invoice.Id;
            await _dbContext.SaveChangesAsync();

            return Json(new
            {
                orderId = order.Id,
                totalAmount = order.TotalAmount,
                invoiceId = invoice.Id,
                checkoutLink = Url.Action("Checkout", "UIInvoice", new { invoiceId = invoice.Id }, Request.Scheme)
            });
        }
        catch (Exception ex)
        {
            // Log error but still return order info
            return Json(new
            {
                orderId = order.Id,
                totalAmount = order.TotalAmount,
                error = "Failed to create payment invoice. Please contact support."
            });
        }
    }
}

public class CartItemRequest
{
    public string VariantId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class CreateOrderRequest
{
    public List<CartItemRequest> Items { get; set; } = new();
    public string ShippingName { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingState { get; set; } = string.Empty;
    public string ShippingZipCode { get; set; } = string.Empty;
    public string ShippingCountry { get; set; } = string.Empty;
    public string? ShippingEmail { get; set; }
    public string? ShippingPhone { get; set; }
    public string? CustomerNotes { get; set; }
}
