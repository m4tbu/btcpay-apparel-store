# Quick Start Guide

Get your BTCPayServer Apparel Store up and running in minutes!

## Installation

### Option 1: From Source (Development)

```bash
# Clone or download this repository
git clone https://github.com/yourusername/btcpayserver-apparel-store.git
cd btcpayserver-apparel-store

# Build the plugin
dotnet build -c Release

# Copy to BTCPayServer plugins directory
cp bin/Release/net8.0/BTCPayServer.Plugins.ApparelStore.dll /path/to/btcpayserver/Plugins/

# Restart BTCPayServer
systemctl restart btcpayserver
```

### Option 2: Install via BTCPayServer UI (Future)

Once published to the plugin marketplace:
1. Go to Server Settings > Plugins
2. Search for "Apparel Store"
3. Click Install
4. Restart server when prompted

## First-Time Setup

### 1. Create Your Store App

1. Log into BTCPayServer
2. Navigate to your Store > **Apps**
3. Click **"Create a new app"**
4. Select **"Apparel Store"**
5. Give it a name (e.g., "My T-Shirt Shop")
6. Save and note the App ID

### 2. Add Your First Product

1. Go to **Apps > [Your Apparel Store] > Admin**
2. Click **"Add New Product"**
3. Fill in the form:
   - **Name**: "Classic T-Shirt"
   - **Description**: "Comfortable cotton t-shirt"
   - **Base Price**: 20.00
   - **Currency**: USD
4. Click **"Create Product"**

### 3. Add Product Variants

You'll be redirected to the product edit page. Add variants:

1. Scroll to **"Product Variants"**
2. Add these variants:

| Size | Color | Color Hex | Price Adjustment |
|------|-------|-----------|------------------|
| S    | Black | #000000   | 0.00             |
| M    | Black | #000000   | 0.00             |
| L    | Black | #000000   | 0.00             |
| XL   | Black | #000000   | 2.00             |
| S    | White | #FFFFFF   | 0.00             |
| M    | White | #FFFFFF   | 0.00             |
| L    | White | #FFFFFF   | 0.00             |
| XL   | White | #FFFFFF   | 2.00             |

3. Click **"Add Variant"** for each one

### 4. Add Product Images

#### Using BTCPayServer File Storage:

1. Go to **Server Settings > Files**
2. Upload your product images
3. Copy the public URLs

#### Using External URLs (Printful, Imgur, etc.):

Just use the direct image URLs

#### Add Images to Product:

1. Back on the Product Edit page
2. Scroll to **"Product Images"**
3. Add images:
   - Image URL: `https://your-btcpay-server.com/files/tshirt-black-front.jpg`
   - Color Variant: Black
   - Display Order: 1
   - Is Primary: ✓ (check for first image)
4. Repeat for all images (front, back, different colors)

### 5. View Your Store

1. Click **"View Store"** or visit:
   ```
   https://your-btcpay-server.com/apps/{appId}/apparel
   ```

2. You should see your product listed!

## Testing the Complete Flow

1. **Browse**: Visit your store and click on your product
2. **Select**: Choose a color and size
3. **Add to Cart**: Click "Add to Cart"
4. **View Cart**: Click the cart icon (should show "1")
5. **Checkout**: Click "Proceed to Checkout"
6. **Shipping Info**: Fill in the shipping form
7. **Pay**: Click "Place Order" - you'll be redirected to BTCPay invoice
8. **Payment**: Complete payment with Bitcoin/Lightning
9. **Confirmation**: You'll see order confirmation

## Adding Printful Products

### Get Printful Product IDs

1. Log into Printful
2. Go to your Product Templates
3. Find the Product ID in the URL or product settings
4. Copy the Variant IDs for each size/color combination

### Add to Your Store

1. When creating/editing products, fill in:
   - **Printful Product ID**: e.g., `123456789`
2. When adding variants, fill in:
   - **Printful Variant ID**: e.g., `987654321`

### Manual Fulfillment (Until Phase 2)

When an order comes in:

1. Check **Admin > Orders**
2. Find the paid order
3. Log into Printful
4. Create manual order with:
   - Items from your store order
   - Shipping address from order details
5. Mark order as "Processing" in your store
6. When Printful ships, mark as "Shipped"

## Customization

### Change Store Appearance

Edit `/wwwroot/css/store.css`:

```css
/* Change primary color */
.btn-primary {
    background-color: #your-color;
}

/* Change header background */
.store-header {
    background-color: #your-color;
}
```

### Add Store Logo

Edit `/Views/ApparelStore/Index.cshtml`:

```html
<h1>
    <img src="https://your-logo-url.com/logo.png" alt="Store Logo" />
    Your Store Name
</h1>
```

## Common Tasks

### Disable a Product

1. Edit product
2. Uncheck "Is Active"
3. Save

### Change Product Price

1. Edit product
2. Update "Base Price"
3. Save (applies to all variants)

### Add New Color

1. Edit product
2. Add variants for all sizes in the new color
3. Upload images with the new color specified

### View Sales

1. Admin > Orders
2. Filter by date/status
3. Export to CSV (coming in Phase 2)

## Getting Help

- **Issue?** Check the [README.md](README.md) troubleshooting section
- **Feature request?** Open an issue on GitHub
- **Questions?** Join BTCPayServer community chat

## Next Steps

- Add more products
- Customize styling
- Set up Printful integration
- Configure automated emails (Phase 2)
- Add promotional banners
- Create product collections

## Tips for Success

1. **High-quality images**: Use clear, well-lit photos
2. **Detailed descriptions**: Help customers know what they're buying
3. **Accurate sizing**: Include size charts if possible
4. **Fast shipping**: Printful typically ships within 2-7 days
5. **Customer service**: Respond quickly to questions

Happy selling! 🚀
