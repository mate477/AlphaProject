const apiBaseUrl = "https://localhost:7071";

// Load Products
async function loadProducts() {
    const response = await fetch(`${apiBaseUrl}/product`);
    const products = await response.json();
    const list = document.getElementById('productList');
    list.innerHTML = '';

    const grouped = products.reduce((acc, product) => {
        if (!acc[product.category]) acc[product.category] = [];
        acc[product.category].push(product);
        return acc;
    }, {});

    for (const category in grouped) {
        const catTitle = document.createElement('div');
        catTitle.className = "category-title";
        catTitle.innerText = category;
        list.appendChild(catTitle);

        grouped[category].forEach(product => {
            const item = document.createElement('div');
            item.className = "product";
            item.innerHTML = `
                <div class="product-info">
                    <img src="${product.imageUrl}" alt="${product.name}">
                    <span>${product.name}</span>
                </div>
                <div>
                    <button class="btn-cart" onclick="addToCart(${product.id})">Add to Cart</button>
                    <span class="price">${product.price.toFixed(2)} $</span>
                </div>
            `;
            list.appendChild(item);
        });
    }
}

// Load Cart
async function loadCart() {
    const response = await fetch(`${apiBaseUrl}/cart`);
    const cartItems = await response.json();
    const list = document.getElementById('cartList');
    list.innerHTML = '';

    let total = 0;

    if (cartItems.length === 0) {
        const emptyMessage = document.createElement('div');
        emptyMessage.innerText = "Cart is currently empty.";
        list.appendChild(emptyMessage);
    } else {
        const rendered = new Set();
        for (const item of cartItems) {
            if (rendered.has(item.productId)) continue;
            rendered.add(item.productId);
            const productResponse = await fetch(`${apiBaseUrl}/product/${item.productId}`);
            const product = await productResponse.json();
            total += product.price * item.quantity;

            const cartItem = document.createElement('div');
            cartItem.className = "cart-item";
            cartItem.innerHTML = `
                <img src="${product.imageUrl}" alt="${product.name}" />
                <div class="cart-details">
                    <strong>${product.name}</strong><br/>
                    Quantity: <span id="cart-qty-${item.productId}">${item.quantity}</span>
                </div>
                <div class="cart-buttons">
                    <button class="btn-increase" onclick="increaseCartItem(${item.productId})">+</button>
                    <button class="btn-decrease" onclick="decreaseCartItem(${item.productId})">-</button>
                    <button class="btn-delete" onclick="removeFromCart(${item.productId})">Delete</button>
                </div>
            `;
            list.appendChild(cartItem);
        }

        const totalPrice = document.createElement('div');
        totalPrice.className = "cart-total";
        totalPrice.innerText = `Total: ${total.toFixed(2)} $`;
        list.appendChild(totalPrice);

        const orderButton = document.createElement('button');
        orderButton.className = "btn-order";
        orderButton.innerText = "Submit Order";
        orderButton.onclick = submitOrder;
        list.appendChild(orderButton);

        const clearButton = document.createElement('button');
        clearButton.className = "btn-clear";
        clearButton.innerText = "Clear Cart";
        clearButton.onclick = clearCart;
        list.appendChild(clearButton);
    }

    document.getElementById('cartCount').innerText = cartItems.reduce((sum, item) => sum + item.quantity, 0);
}

// Cart operations
async function addToCart(productId) {
    await fetch(`${apiBaseUrl}/cart`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ productId, quantity: 1 })
    });
    loadCart();
}

async function increaseCartItem(productId) {
    await fetch(`${apiBaseUrl}/cart`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ productId, quantity: 1 })
    });
    loadCart();
}

async function decreaseCartItem(productId) {
    const qtyElem = document.getElementById(`cart-qty-${productId}`);
    if (parseInt(qtyElem.innerText) > 1) {
        await fetch(`${apiBaseUrl}/cart`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productId, quantity: -1 })
        });
    } else {
        await removeFromCart(productId);
    }
    loadCart();
}

async function removeFromCart(productId) {
    await fetch(`${apiBaseUrl}/cart/${productId}`, { method: 'DELETE' });
    loadCart();
}

async function clearCart() {
    await fetch(`${apiBaseUrl}/cart/clear`, { method: 'DELETE' });
    loadCart();
}

async function submitOrder() {
    await fetch(`${apiBaseUrl}/order`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
    });
    showToast("✅ Order sent successfully!");
    loadCart();
}

// Toast display message
function showToast(message) {
    const toast = document.getElementById('toast');
    toast.innerText = message;
    toast.style.visibility = 'visible';
    toast.style.opacity = 1;

    setTimeout(() => {
        toast.style.opacity = 0;
        toast.style.visibility = 'hidden';
    }, 3000);
}

// Loading Site
window.onload = function () {
    loadProducts();
    loadCart();

    document.getElementById('cartIcon').addEventListener('click', () => {
        document.getElementById('cartSection').style.display = 'block';
        document.getElementById('cartSection').scrollIntoView({ behavior: "smooth" });
    });
};