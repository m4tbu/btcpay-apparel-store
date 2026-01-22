// Shopping cart management using localStorage

function getCart() {
    const cartData = localStorage.getItem('apparelCart');
    return cartData ? JSON.parse(cartData) : [];
}

function saveCart(cart) {
    localStorage.setItem('apparelCart', JSON.stringify(cart));
    updateCartCount();
}

function addToCartStorage(variantId, quantity) {
    const cart = getCart();
    const existingItem = cart.find(item => item.variantId === variantId);

    if (existingItem) {
        existingItem.quantity += quantity;
    } else {
        cart.push({ variantId, quantity });
    }

    saveCart(cart);
}

function removeFromCartStorage(variantId) {
    let cart = getCart();
    cart = cart.filter(item => item.variantId !== variantId);
    saveCart(cart);
}

function updateCartItemQuantity(variantId, quantity) {
    const cart = getCart();
    const item = cart.find(item => item.variantId === variantId);

    if (item) {
        item.quantity = quantity;
        saveCart(cart);
    }
}

function clearCart() {
    localStorage.removeItem('apparelCart');
    updateCartCount();
}

function getCartItemCount() {
    const cart = getCart();
    return cart.reduce((total, item) => total + item.quantity, 0);
}

function updateCartCount() {
    const count = getCartItemCount();
    const countElements = document.querySelectorAll('.cart-count');
    countElements.forEach(el => {
        el.textContent = count;
        el.style.display = count > 0 ? 'inline-block' : 'none';
    });
}

// Initialize cart count on page load
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', updateCartCount);
} else {
    updateCartCount();
}
