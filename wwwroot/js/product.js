// Product page functionality

let selectedVariant = null;

// Change main image when thumbnail is clicked
function changeMainImage(thumbnail) {
    const mainImage = document.getElementById('mainImage');
    mainImage.src = thumbnail.src;

    // Update active thumbnail
    document.querySelectorAll('.thumbnail').forEach(t => t.classList.remove('active'));
    thumbnail.classList.add('active');
}

// Filter images by selected color
function filterImagesByColor(color) {
    const thumbnails = document.querySelectorAll('.thumbnail');
    const colorImages = Array.from(thumbnails).filter(t =>
        t.dataset.color === color || t.dataset.color === '' || !t.dataset.color
    );

    // Show first matching image
    if (colorImages.length > 0) {
        changeMainImage(colorImages[0]);
    }
}

// Update price based on selected variant
function updatePrice() {
    if (!productData) return;

    const colorInput = document.querySelector('input[name="color"]:checked');
    const sizeSelect = document.getElementById('sizeSelect');

    if (!colorInput || !sizeSelect || !sizeSelect.value) {
        return;
    }

    const color = colorInput.value;
    const size = sizeSelect.value;

    // Find matching variant
    const variant = productData.variants.find(v =>
        v.color === color && v.size === size
    );

    if (variant) {
        selectedVariant = variant;
        const finalPrice = productData.basePrice + variant.priceAdjustment;
        const priceElement = document.getElementById('displayPrice');
        priceElement.textContent = `$${finalPrice.toFixed(2)} ${productData.currency}`;

        // Enable/disable add to cart button
        const addButton = document.getElementById('addToCartBtn');
        addButton.disabled = !variant.isAvailable;
        addButton.textContent = variant.isAvailable ? 'Add to Cart' : 'Out of Stock';
    }
}

// Add to cart
function addToCart() {
    const colorInput = document.querySelector('input[name="color"]:checked');
    const sizeSelect = document.getElementById('sizeSelect');
    const quantityInput = document.getElementById('quantity');

    // Validation
    if (!colorInput) {
        showMessage('Please select a color', 'error');
        return;
    }

    if (!sizeSelect || !sizeSelect.value) {
        showMessage('Please select a size', 'error');
        return;
    }

    if (!selectedVariant) {
        showMessage('Please select a valid variant', 'error');
        return;
    }

    if (!selectedVariant.isAvailable) {
        showMessage('This variant is out of stock', 'error');
        return;
    }

    const quantity = parseInt(quantityInput.value);
    if (quantity < 1 || quantity > 10) {
        showMessage('Quantity must be between 1 and 10', 'error');
        return;
    }

    // Add to cart
    addToCartStorage(selectedVariant.id, quantity);
    showMessage(`Added ${quantity} item(s) to cart`, 'success');

    // Reset form
    setTimeout(() => {
        document.getElementById('message').style.display = 'none';
    }, 3000);
}

// Show message
function showMessage(text, type) {
    const messageEl = document.getElementById('message');
    messageEl.textContent = text;
    messageEl.className = 'message ' + type;
    messageEl.style.display = 'block';
}

// Event listeners
document.addEventListener('DOMContentLoaded', function() {
    // Color selection change
    const colorInputs = document.querySelectorAll('input[name="color"]');
    colorInputs.forEach(input => {
        input.addEventListener('change', function() {
            filterImagesByColor(this.value);
            updatePrice();
        });
    });

    // Size selection change
    const sizeSelect = document.getElementById('sizeSelect');
    if (sizeSelect) {
        sizeSelect.addEventListener('change', updatePrice);
    }

    // Pre-select first color if only one exists
    if (colorInputs.length === 1) {
        colorInputs[0].checked = true;
        colorInputs[0].dispatchEvent(new Event('change'));
    }
});
