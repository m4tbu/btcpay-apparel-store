# BTCPayServer Apparel Store Plugin

A full-featured e-commerce plugin for BTCPayServer that enables selling dropshipped apparel with product variants (sizes/colors), image galleries, shopping cart, and Bitcoin payments.

## Features

- **Product Management**
  - Create products with descriptions and pricing
  - Multiple product variants (sizes and colors)
  - Image gallery with multiple images per product
  - Color-specific product images
  - Stock management per variant

- **Storefront**
  - Modern, responsive product listing page
  - Detailed product pages with image galleries
  - Interactive variant selection (size/color)
  - Dynamic price updates based on selected variant
  - Shopping cart with localStorage persistence
  - Checkout with shipping information collection

- **Payment Integration**
  - Automatic BTCPay Server invoice creation
  - Bitcoin payment processing
  - Order tracking tied to invoices

- **Admin Panel**
  - Product CRUD operations
  - Variant management
  - Image management
  - Order management and fulfillment tracking

- **Printful Integration Ready**
  - Product and variant fields for Printful IDs
  - Order data structure ready for fulfillment automation (Phase 2)

## Installation

### Prerequisites

- BTCPayServer instance (v1.13 or later)
- .NET 8.0 SDK
- SQL database (PostgreSQL/SQLite/MySQL)

### Building the Plugin

1. Clone or download this repository to your local machine

2. Build the plugin:
```bash
cd btcpay-apparel-store
dotnet build -c Release
```

3. Package the plugin:
```bash
dotnet pack -c Release
```

### Installing on BTCPayServer

#### Method 1: Manual Installation

1. Copy the compiled DLL to your BTCPayServer plugins directory:
```bash
cp bin/Release/net8.0/BTCPayServer.Plugins.ApparelStore.dll /path/to/btcpayserver/Plugins/
```

2. Restart BTCPayServer:
```bash
systemctl restart btcpayserver
```

#### Method 2: BTCPayServer Plugin Builder (Recommended)

1. Fork this repository on GitHub

2. Add your repository to the BTCPayServer plugin registry

3. Install through BTCPayServer admin interface:
   - Go to Server Settings > Plugins
   - Click "Add Plugin"
   - Enter your plugin repository URL
   - Click Install

### Database Migrations

The plugin automatically runs database migrations on startup. The following tables will be created:
- `Products`
- `ProductVariants`
- `ProductImages`
- `Orders`
- `OrderItems`

## Usage

### Creating a Store App

1. Log in to your BTCPayServer admin panel
2. Go to your store > Apps
3. Click "Create a new app"
4. Select "Apparel Store"
5. Configure your app name and settings

### Managing Products

#### Adding a Product

1. Navigate to Apps > [Your Apparel Store] > Admin
2. Click "Add New Product"
3. Fill in:
   - Product name
   - Description
   - Base price
   - Currency (USD, BTC, etc.)
   - Optional: Printful Product ID
4. Click "Create Product"

#### Adding Variants

1. Go to Edit Product page
2. Scroll to "Product Variants" section
3. Click "Add Variant"
4. Enter:
   - Size (e.g., S, M, L, XL, 2XL)
   - Color (e.g., Black, White, Navy)
   - Color hex code (for color swatches, e.g., #000000)
   - Price adjustment (if this variant costs more/less)
   - Stock quantity
   - Optional: Printful Variant ID
5. Click "Add Variant"

#### Adding Images

1. Upload images to BTCPayServer file storage or use external URLs (Printful, CDN, etc.)
2. Go to Edit Product page
3. Scroll to "Product Images" section
4. Click "Add Image"
5. Enter:
   - Image URL
   - Optional: Color variant (to show specific images for specific colors)
   - Display order (lower numbers appear first)
   - Mark as primary image (checkbox)
6. Click "Add Image"

#### Using BTCPayServer File Storage

To host images on your BTCPayServer:
1. Go to Server Settings > Files
2. Upload your product images
3. Copy the public URL
4. Use this URL when adding product images

### Customer Shopping Experience

1. Customer visits the storefront at `/apps/{appId}/apparel`
2. Browses products and clicks on one for details
3. Selects color and size on product page
4. Adds to cart
5. Views cart and proceeds to checkout
6. Enters shipping information
7. Creates order and is redirected to BTCPay invoice for payment
8. Completes payment with Bitcoin/Lightning

### Order Management

#### Viewing Orders

1. Navigate to Apps > [Your Apparel Store] > Admin
2. Click "View Orders"
3. See all orders with status, customer info, and items

#### Order Statuses

- **Pending**: Order created, awaiting payment
- **PaymentReceived**: Payment confirmed
- **Processing**: Order being prepared for fulfillment
- **Shipped**: Order shipped to customer
- **Completed**: Order delivered
- **Cancelled**: Order cancelled
- **Refunded**: Payment refunded

### Printful Integration (Phase 2 - Manual for Now)

The plugin stores Printful product/variant IDs for future automation. For now:

1. When an order is marked as "PaymentReceived", manually create the order in Printful
2. Use the stored Printful IDs to match products
3. Enter shipping information from the order
4. Mark order as "Processing" in the plugin
5. When Printful ships, update order status to "Shipped"

**Future Enhancement**: Automatic webhook integration to create Printful orders when payment is received.

## Configuration

### File Structure

```
BTCPayServer.Plugins.ApparelStore/
├── Controllers/
│   ├── ApparelStoreController.cs       # Public storefront
│   └── ApparelStoreAdminController.cs  # Admin panel
├── Models/
│   ├── Product.cs                       # Data models
│   └── Order.cs
├── Views/
│   ├── ApparelStore/
│   │   ├── Index.cshtml                 # Product listing
│   │   ├── Product.cshtml               # Product detail
│   │   ├── Cart.cshtml                  # Shopping cart
│   │   └── Checkout.cshtml              # Checkout page
│   └── ApparelStoreAdmin/
│       ├── Index.cshtml                 # Admin dashboard
│       ├── EditProduct.cshtml           # Product editor
│       └── Orders.cshtml                # Order list
├── wwwroot/
│   ├── css/
│   │   └── store.css                    # Storefront styles
│   └── js/
│       ├── cart.js                      # Cart management
│       └── product.js                   # Product page logic
├── Data/
│   └── ApparelStoreDbContext.cs         # EF Core context
├── ApparelStorePlugin.cs                # Plugin entry point
├── ApparelStoreHostedService.cs         # Background service
└── BTCPayServer.Plugins.ApparelStore.csproj
```

### Customization

#### Styling

Edit `/wwwroot/css/store.css` to customize the appearance:
- Colors and branding
- Typography
- Layout and spacing
- Responsive breakpoints

#### Templates

Modify Razor views in `/Views/` to change:
- Page layouts
- Product card design
- Checkout form fields
- Admin interface

## API Endpoints

### Public Endpoints (Customer-facing)

- `GET /apps/{appId}/apparel` - Product listing page
- `GET /apps/{appId}/apparel/product/{productId}` - Product detail page
- `GET /apps/{appId}/apparel/cart` - Shopping cart page
- `GET /apps/{appId}/apparel/checkout` - Checkout page
- `POST /apps/{appId}/apparel/api/cart/items` - Fetch cart item details
- `POST /apps/{appId}/apparel/api/order/create` - Create order and invoice
- `GET /apps/{appId}/apparel/order/{orderId}` - Order confirmation page

### Admin Endpoints (Store owner)

- `GET /apps/{appId}/apparel/admin` - Admin dashboard
- `GET /apps/{appId}/apparel/admin/product/create` - Create product form
- `POST /apps/{appId}/apparel/admin/product/create` - Create product action
- `GET /apps/{appId}/apparel/admin/product/{productId}` - Edit product form
- `POST /apps/{appId}/apparel/admin/product/{productId}` - Update product action
- `POST /apps/{appId}/apparel/admin/product/{productId}/delete` - Delete product
- `POST /apps/{appId}/apparel/admin/product/{productId}/variant` - Add variant
- `POST /apps/{appId}/apparel/admin/variant/{variantId}/delete` - Delete variant
- `POST /apps/{appId}/apparel/admin/product/{productId}/image` - Add image
- `POST /apps/{appId}/apparel/admin/image/{imageId}/delete` - Delete image
- `GET /apps/{appId}/apparel/admin/orders` - Order list
- `GET /apps/{appId}/apparel/admin/order/{orderId}` - Order detail

## Development

### Running Locally

1. Clone BTCPayServer repository
2. Add this plugin as a project reference
3. Run BTCPayServer in development mode:

```bash
cd /path/to/BTCPayServer
dotnet run --project BTCPayServer
```

### Testing

Create test products and variants:
1. Add a t-shirt product with base price $20
2. Add variants: S/M/L/XL in Black/White/Navy
3. Upload multiple images per color
4. Test the complete checkout flow

### Database Migrations

To create a new migration:

```bash
dotnet ef migrations add MigrationName --context ApparelStoreDbContext
```

## Troubleshooting

### Plugin Not Loading

- Check BTCPayServer logs: `docker logs btcpayserver`
- Ensure .NET 8.0 compatibility
- Verify plugin DLL is in correct directory

### Database Errors

- Check connection string in BTCPayServer config
- Ensure migrations ran successfully
- Verify database permissions

### Images Not Displaying

- Check image URLs are publicly accessible
- Verify CORS settings if using external CDN
- Use BTCPayServer file storage for reliability

### Cart Not Persisting

- Ensure JavaScript is enabled in browser
- Check browser localStorage is not blocked
- Test in different browser/incognito mode

## Roadmap

### Phase 2: Automation
- [ ] Automatic Printful order creation via webhook
- [ ] Shipping tracking integration
- [ ] Automated status updates
- [ ] Email notifications for order updates

### Phase 3: Enhanced Features
- [ ] Product categories and collections
- [ ] Search and filtering
- [ ] Customer reviews and ratings
- [ ] Discount codes and promotions
- [ ] Bulk product import/export
- [ ] Multi-currency pricing

### Phase 4: Analytics
- [ ] Sales reports and analytics
- [ ] Inventory tracking
- [ ] Customer insights
- [ ] Conversion funnel analysis

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

[Specify your license here]

## Support

For issues and questions:
- Open an issue on GitHub
- Contact BTCPayServer community
- Check BTCPayServer documentation

## Credits

Built for BTCPayServer by [Your Name]
Integrates with Printful for dropshipping fulfillment
