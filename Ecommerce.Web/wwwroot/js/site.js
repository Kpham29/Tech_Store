// ===== Cookie Utility =====
function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}

function addToCart(productId, quantity = 1) {
    let token = getCookie('jwtToken'); 
    if (!token) {
        window.location.href = '/Auth/Login?message=' + encodeURIComponent('You have not logged in yet');
        return;
    }
    const data = {
        productId: productId,
        quantity: parseInt(quantity)
    };
    
    // Thêm nút loading state nếu có thể, hoặc đơn giản disable nút
    fetch('http://localhost:5678/api/carts/add-to-cart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        },
        body: JSON.stringify(data)
    })
        .then(res => {
            if(res.ok) {
                showToast('Add product to cart successful!', 'success');
                updateCartCount(); 
            } else {
                showToast('Error adding product to cart!', 'error');
            }
        })
        .catch(err => {
            console.error(err);
            showToast('Error adding product to cart!', 'error');
        });
}
function updateCartCount() {
    let badge = document.getElementById('cartCountBadge');
    if(!badge) return;
    
    let token = getCookie('jwtToken'); 
    if (!token) return;

    fetch('http://localhost:5678/api/carts/count', {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + token
        }
    })
        .then(res => {
            if (res.ok) return res.json();
            throw new Error('Unauthorized');
        })
        .then(data => {
            if (data.count > 0) {
                badge.textContent = data.count;
                badge.classList.remove('hidden');
            } else {
                badge.classList.add('hidden');
            }
        })
        .catch(err => console.error(err));
}

function showToast(message, type = 'success') {
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'fixed bottom-4 right-4 z-50 flex flex-col gap-2';
        document.body.appendChild(toastContainer);
    }

    const toast = document.createElement('div');
    const bgColor = type === 'success' ? 'bg-green-500' : 'bg-red-500';
    toast.className = `${bgColor} text-white px-4 py-3 rounded shadow-lg transition-all duration-300 opacity-0 transform translate-y-4`;
    
    // Add icon based on type
    const iconClass = type === 'success' ? 'bi-check-circle' : 'bi-exclamation-circle';
    toast.innerHTML = `<div class="flex items-center gap-2"><i class="bi ${iconClass} text-lg"></i><span>${message}</span></div>`;

    toastContainer.appendChild(toast);

    // Fade in
    requestAnimationFrame(() => {
        toast.classList.remove('opacity-0', 'translate-y-4');
        toast.classList.add('opacity-100', 'translate-y-0');
    });

    // Fade out and remove after 3 seconds
    setTimeout(() => {
        toast.classList.remove('opacity-100', 'translate-y-0');
        toast.classList.add('opacity-0', 'translate-y-4');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}
document.addEventListener('DOMContentLoaded', updateCartCount);
