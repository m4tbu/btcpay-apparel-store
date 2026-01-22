# Development Guide

Guide for developers who want to extend or modify the BTCPayServer Apparel Store plugin.

## Development Environment Setup

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 / VS Code / Rider
- BTCPayServer source code
- PostgreSQL / SQLite (for database)
- Git

### Clone BTCPayServer

```bash
git clone https://github.com/btcpayserver/btcpayserver.git
cd btcpayserver
```

### Add Plugin as Project Reference

1. Clone this plugin into the BTCPayServer solution:

```bash
cd btcpayserver
git clone https://github.com/yourusername/btcpayserver-apparel-store.git BTCPayServer.Plugins.ApparelStore
```

2. Add project reference to BTCPayServer.csproj:

```xml
<ItemGroup>
  <ProjectReference Include="../BTCPayServer.Plugins.ApparelStore/BTCPayServer.Plugins.ApparelStore.csproj" />
</ItemGroup>
```

### Run in Development Mode

```bash
cd BTCPayServer
dotnet run
```

BTCPayServer will start on `https://localhost:14142`

## Project Structure

```
BTCPayServer.Plugins.ApparelStore/
├── Controllers/              # MVC Controllers
│   ├── ApparelStoreController.cs          # Public storefront
│   └── ApparelStoreAdminController.cs     # Admin panel
├── Models/                   # Data models
│   ├── Product.cs
│   └── Order.cs
├── Views/                    # Razor views
│   ├── ApparelStore/
│   │   ├── Index.cshtml      # Product listing
│   │   ├── Product.cshtml    # Product detail
│   │   ├── Cart.cshtml       # Shopping cart
│   │   └── Checkout.cshtml   # Checkout
│   └── ApparelStoreAdmin/
│       ├── Index.cshtml      # Admin dashboard
│       └── EditProduct.cshtml # Product editor
├── Services/                 # Business logic (future)
├── Data/                     # EF Core
│   └── ApparelStoreDbContext.cs
├── wwwroot/                  # Static files
│   ├── css/store.css
│   └── js/
│       ├── cart.js
│       └── product.js
├── ApparelStorePlugin.cs     # Plugin entry point
└── ApparelStoreHostedService.cs # Background service
```

## Adding New Features

### Adding a New Model

1. Create model in `Models/`
2. Add DbSet to `ApparelStoreDbContext.cs`
3. Create migration:

```bash
dotnet ef migrations add AddNewModel --context ApparelStoreDbContext
```

4. Migration will auto-apply on next startup

### Adding a New Controller Action

```csharp
[HttpGet("custom-endpoint")]
[AllowAnonymous]
public async Task<IActionResult> CustomAction(string appId)
{
    // Your logic here
    ViewBag.AppId = appId;
    return View();
}
```

### Adding a New View

Create in `Views/ApparelStore/` or `Views/ApparelStoreAdmin/`:

```cshtml
@model YourModel
@{
    ViewData["Title"] = "Page Title";
    var appId = ViewBag.AppId;
}

<h1>Your Content</h1>
```

### Adding JavaScript Functionality

Add to `/wwwroot/js/`:

```javascript
// your-feature.js
function yourFunction() {
    // Implementation
}
```

Reference in view:

```html
<script src="~/apparel-store/your-feature.js"></script>
```

## Database Migrations

### Create New Migration

```bash
dotnet ef migrations add MigrationName --context ApparelStoreDbContext --project BTCPayServer.Plugins.ApparelStore
```

### Update Database

Migrations apply automatically on startup via `ApparelStoreHostedService`.

Manual update:

```bash
dotnet ef database update --context ApparelStoreDbContext
```

### Rollback Migration

```bash
dotnet ef database update PreviousMigrationName --context ApparelStoreDbContext
```

## Testing

### Manual Testing

1. Start BTCPayServer in development mode
2. Create a test store
3. Create an Apparel Store app
4. Add test products with variants and images
5. Test complete checkout flow

### Unit Testing (Future Enhancement)

Create test project:

```bash
dotnet new xunit -n BTCPayServer.Plugins.ApparelStore.Tests
```

Add tests for:
- Product variant selection logic
- Price calculations
- Order creation
- Invoice generation

## BTCPayServer Integration Points

### Invoice Creation

```csharp
var invoice = await _invoiceRepository.CreateInvoiceAsync(
    storeId,
    invoiceRequest,
    HttpContext.Request.GetAbsoluteRoot()
);
```

### Store Data Access

```csharp
var store = await _storeRepository.FindStore(appId);
```

### Payment Status Webhooks

Listen for invoice status changes:

```csharp
// TODO: Implement IHostedService to monitor invoice status
// Update order status when payment is confirmed
```

## Printful Integration (Phase 2)

### Architecture

```
Order Created → Payment Received → Webhook Triggered →
Printful API Called → Order Submitted → Status Updated
```

### Implementation Plan

1. Create `Services/PrintfulService.cs`
2. Add Printful API client
3. Implement webhook handler
4. Auto-create Printful orders on payment

### Example Service

```csharp
public class PrintfulService
{
    private readonly HttpClient _httpClient;

    public async Task<string> CreateOrderAsync(Order order)
    {
        // Map order to Printful format
        var printfulOrder = new
        {
            recipient = new
            {
                name = order.ShippingName,
                address1 = order.ShippingAddress,
                city = order.ShippingCity,
                state_code = order.ShippingState,
                country_code = order.ShippingCountry,
                zip = order.ShippingZipCode
            },
            items = order.Items.Select(i => new
            {
                sync_variant_id = i.ProductVariant.PrintfulVariantId,
                quantity = i.Quantity
            })
        };

        var response = await _httpClient.PostAsJsonAsync(
            "https://api.printful.com/orders",
            printfulOrder
        );

        return await response.Content.ReadAsStringAsync();
    }
}
```

## Styling Customization

### CSS Variables

Add to `wwwroot/css/store.css`:

```css
:root {
    --primary-color: #007bff;
    --secondary-color: #6c757d;
    --success-color: #28a745;
    --danger-color: #dc3545;
}
```

### Theme System

Future enhancement: Allow store owners to customize colors from admin panel.

## Performance Optimization

### Caching

Add output caching to product listing:

```csharp
[ResponseCache(Duration = 300)] // 5 minutes
public async Task<IActionResult> Index(string appId)
{
    // ...
}
```

### Eager Loading

Already implemented via `.Include()`:

```csharp
var products = await _dbContext.Products
    .Include(p => p.Images)
    .Include(p => p.Variants)
    .ToListAsync();
```

### Image Optimization

Recommend:
- Use WebP format
- Lazy loading for images
- CDN for image delivery
- Thumbnail generation

## Security Considerations

### Authorization

- Public endpoints: `[AllowAnonymous]`
- Admin endpoints: `[Authorize(Policy = Policies.CanModifyStoreSettings)]`

### Input Validation

Always validate user input:

```csharp
if (!ModelState.IsValid)
{
    return BadRequest(ModelState);
}
```

### SQL Injection Prevention

Using EF Core parameterized queries automatically.

### XSS Prevention

Razor automatically HTML-encodes output.

## Debugging

### Enable Detailed Errors

In BTCPayServer `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "BTCPayServer.Plugins.ApparelStore": "Debug"
    }
  }
}
```

### Add Logging

```csharp
private readonly ILogger<ApparelStoreController> _logger;

public ApparelStoreController(ILogger<ApparelStoreController> logger, ...)
{
    _logger = logger;
}

// Usage
_logger.LogInformation("Order created: {OrderId}", order.Id);
_logger.LogError(ex, "Failed to create invoice");
```

### Debugging Tips

- Use breakpoints in VS Code/Visual Studio
- Check BTCPayServer logs: `docker logs btcpayserver`
- Use browser dev tools for frontend issues
- Test with ngrok for webhook testing

## Contributing

### Pull Request Process

1. Fork the repository
2. Create feature branch: `git checkout -b feature/my-feature`
3. Make changes and test thoroughly
4. Commit with clear messages
5. Push and create PR
6. Wait for review

### Code Style

- Follow C# coding conventions
- Use meaningful variable names
- Add XML comments for public APIs
- Keep methods focused and small

### Testing Checklist

- [ ] Product CRUD operations
- [ ] Variant management
- [ ] Image gallery functionality
- [ ] Shopping cart persistence
- [ ] Checkout flow
- [ ] Payment integration
- [ ] Order creation
- [ ] Admin panel access control
- [ ] Mobile responsiveness

## Deployment

### Build Release

```bash
dotnet build -c Release
```

### Package Plugin

```bash
dotnet pack -c Release -o ./artifacts
```

### Deploy to BTCPayServer

```bash
cp bin/Release/net8.0/BTCPayServer.Plugins.ApparelStore.dll \
   /var/lib/btcpayserver/Plugins/

systemctl restart btcpayserver
```

### Docker Deployment

Add to BTCPayServer Dockerfile:

```dockerfile
COPY BTCPayServer.Plugins.ApparelStore/bin/Release/net8.0/*.dll \
     /app/Plugins/
```

## Resources

- [BTCPayServer Docs](https://docs.btcpayserver.org/)
- [BTCPayServer GitHub](https://github.com/btcpayserver/btcpayserver)
- [Printful API Docs](https://developers.printful.com/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)

## Support

- GitHub Issues: Report bugs and feature requests
- BTCPayServer Mattermost: Community chat
- Stack Overflow: Technical questions

## License

[Your License Here]
