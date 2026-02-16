<template>
  <div class="sales-container">
    <div class="header">
      <h2>Registrar Venta</h2>
    </div>

    <div class="sales-layout">
      <div class="products-section">
        <div class="section-header">
          <h3>Productos Disponibles</h3>
          <span class="product-count">{{ productsStore.products.length }} productos</span>
        </div>

        <LoadingSpinner v-if="productsStore.loading" message="Cargando productos..." />

        <div v-else class="products-grid">
          <div v-for="product in productsStore.products" :key="product.id" class="product-card">
            <div class="product-image">
              <img v-if="product.imageUrl" :src="product.imageUrl" :alt="product.name" />
              <div v-else class="image-placeholder">
                <PhPackage :size="28" />
              </div>
            </div>
            <div class="product-info">
              <h4 class="product-name">{{ product.name }}</h4>
              <div class="product-meta">
                <span class="price-badge">${{ formatNumber(product.price, 2) }}</span>
                <span 
                  class="stock-badge" 
                  :class="{ 'low-stock': product.stock <= 5, 'out-of-stock': product.stock === 0 }"
                >
                  <PhCube :size="14" />
                  {{ formatNumber(product.stock, 0) }}
                </span>
              </div>
              <div class="add-to-cart">
                <div class="qty-control">
                  <button @click="decrementQty(product.id)" class="qty-btn" :disabled="!quantities[product.id]">-</button>
                  <input 
                    v-model.number="quantities[product.id]" 
                    type="number" 
                    min="0" 
                    :max="product.stock" 
                    class="qty-input"
                  />
                  <button @click="incrementQty(product.id, product.stock)" class="qty-btn">+</button>
                </div>
                <button 
                  @click="addToCart(product)" 
                  :disabled="product.stock === 0 || !quantities[product.id]"
                  class="btn-add"
                >
                  <PhPlus :size="16" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="cart-section">
        <div class="cart-header">
          <h3>Carrito de Compras</h3>
        </div>

        <EmptyState v-if="cart.length === 0" :icon="PhShoppingCart" title="El carrito está vacío">
          <template #action>
            <span class="empty-hint">Selecciona productos para agregar</span>
          </template>
        </EmptyState>

        <template v-else>
          <div class="cart-items">
            <div v-for="item in cart" :key="item.productId" class="cart-item">
              <div class="item-image">
                <img v-if="item.imageUrl" :src="item.imageUrl" :alt="item.productName" />
                <div v-else class="item-placeholder">
                  <PhPackage :size="16" />
                </div>
              </div>
              <div class="item-details">
                <span class="item-name">{{ item.productName }}</span>
                <span class="item-price">${{ formatNumber(item.unitPrice, 2) }} c/u</span>
              </div>
              <div class="item-quantity">
                <button @click="updateQty(item, -1)" class="qty-btn-small">-</button>
                <span>{{ item.quantity }}</span>
                <button @click="updateQty(item, 1)" class="qty-btn-small">+</button>
              </div>
              <div class="item-subtotal">
                ${{ formatNumber(item.subTotal, 2) }}
              </div>
              <button @click="removeFromCart(item.productId)" class="btn-remove">
                <PhTrash :size="18" />
              </button>
            </div>
          </div>

          <div class="cart-footer">
            <div class="cart-total-section">
              <span class="total-label">Total</span>
              <span class="total-amount">${{ formatNumber(total, 2) }}</span>
            </div>
            <button @click="registerSale" class="btn-register" :disabled="productsStore.loading">
              <PhCheck :size="20" />
              {{ productsStore.loading ? 'Registrando...' : 'Registrar Venta' }}
            </button>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useProductsStore } from '../stores/products'
import { toast } from 'vue3-toastify'
import {
  PhPackage,
  PhPlus,
  PhTrash,
  PhShoppingCart,
  PhCube,
  PhCheck
} from '@phosphor-icons/vue'
import LoadingSpinner from '../components/ui/LoadingSpinner.vue'
import EmptyState from '../components/ui/EmptyState.vue'

const productsStore = useProductsStore()

const cart = ref([])
const quantities = reactive({})

const total = computed(() => cart.value.reduce((sum, item) => sum + item.subTotal, 0))

const formatNumber = (num, decimals = 2) => {
  const fixed = num.toFixed(decimals)
  const parts = fixed.split('.')
  parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, '.')
  return parts.join(',')
}

watch(() => productsStore.products, (newProducts) => {
  newProducts.forEach(p => {
    if (quantities[p.id] === undefined) {
      quantities[p.id] = 0
    }
  })
}, { immediate: true })

const incrementQty = (productId, maxStock) => {
  if ((quantities[productId] || 0) < maxStock) {
    quantities[productId] = ((quantities[productId] || 0) + 1)
  }
}

const decrementQty = (productId) => {
  if ((quantities[productId] || 0) > 0) {
    quantities[productId] = (quantities[productId] || 0) - 1
  }
}

const addToCart = (product) => {
  const qty = quantities[product.id] || 0
  if (qty <= 0 || qty > product.stock) return

  const existing = cart.value.find(item => item.productId === product.id)
  if (existing) {
    const newQty = existing.quantity + qty
    if (newQty > product.stock) {
      toast.warning('Stock insuficiente')
      return
    }
    existing.quantity = newQty
    existing.subTotal = existing.quantity * existing.unitPrice
  } else {
    cart.value.push({
      productId: product.id,
      productName: product.name,
      imageUrl: product.imageUrl,
      quantity: qty,
      unitPrice: product.price,
      subTotal: qty * product.price
    })
  }
  quantities[product.id] = 0
}

const updateQty = (item, delta) => {
  const product = productsStore.products.find(p => p.id === item.productId)
  if (!product) return
  
  const newQty = item.quantity + delta
  if (newQty < 1) return
  if (newQty > product.stock) {
    toast.warning('Stock insuficiente')
    return
  }
  
  item.quantity = newQty
  item.subTotal = item.quantity * item.unitPrice
}

const removeFromCart = (productId) => {
  cart.value = cart.value.filter(item => item.productId !== productId)
}

const registerSale = async () => {
  const saleData = {
    items: cart.value.map(item => ({
      productId: item.productId,
      quantity: item.quantity,
      unitPrice: item.unitPrice
    }))
  }
  const success = await productsStore.createSale(saleData)
  if (success) {
    cart.value = []
  }
}

onMounted(() => {
  productsStore.fetchProducts({ pageSize: 100 })
})
</script>

<style scoped>
.sales-container {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.header h2 {
  font-size: 1.75rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
}

.sales-layout {
  display: grid;
  grid-template-columns: 1fr 400px;
  gap: 1.5rem;
  align-items: start;
}

/* Products Section */
.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.section-header h3 {
  font-size: 1.1rem;
  font-weight: 600;
  color: #374151;
  margin: 0;
}

.product-count {
  color: #6b7280;
  font-size: 0.875rem;
}

.empty-hint {
  font-size: 0.875rem;
  color: #9ca3af;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
  gap: 0.75rem;
}

.product-card {
  background: white;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  transition: all 0.2s ease;
}

.product-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.12);
}

.product-image {
  width: 100%;
  height: 90px;
  overflow: hidden;
  background: #f9fafb;
  display: flex;
  align-items: center;
  justify-content: center;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.image-placeholder {
  color: #9ca3af;
}

.product-info {
  padding: 0.75rem;
}

.product-name {
  font-size: 0.85rem;
  font-weight: 500;
  color: #1f2937;
  margin: 0 0 0.5rem 0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.product-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.price-badge {
  display: inline-block;
  padding: 0.2rem 0.5rem;
  background: #ecfdf5;
  color: #059669;
  border-radius: 4px;
  font-weight: 600;
  font-size: 0.8rem;
}

.stock-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 500;
  background: #d1fae5;
  color: #065f46;
}

.stock-badge.low-stock {
  background: #fef3c7;
  color: #92400e;
}

.stock-badge.out-of-stock {
  background: #fee2e2;
  color: #991b1b;
}

.add-to-cart {
  display: flex;
}

.qty-control {
  display: flex;
  align-items: center;
  border-radius: 6px 0 0 6px;
  overflow: hidden;
  flex: 1;
  background: #f3f4f6;
}

.qty-btn {
  width: 24px;
  height: 28px;
  border: none;
  background: transparent;
  color: #374151;
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.15s;
  flex-shrink: 0;
}

.qty-btn:hover:not(:disabled) {
  background: #e5e7eb;
}

.qty-btn:disabled {
  opacity: 0.4;
  cursor: default;
}

.qty-input {
  flex: 1;
  min-width: 0;
  height: 28px;
  border: none;
  background: transparent;
  text-align: center;
  font-size: 0.8125rem;
  font-weight: 500;
}

.qty-input::-webkit-inner-spin-button,
.qty-input::-webkit-outer-spin-button {
  -webkit-appearance: none;
}

.btn-add {
  width: 28px;
  height: 28px;
  border: none;
  border-radius: 0 6px 6px 0;
  background: #10b981;
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.15s;
  padding: 0;
  flex-shrink: 0;
}

.btn-add:hover:not(:disabled) {
  background: #059669;
}

.btn-add:disabled {
  background: #d1fae5;
  color: #6ee7b7;
  cursor: not-allowed;
}

/* Cart Section */
.cart-section {
  background: white;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  position: sticky;
  top: 1rem;
}

.cart-header {
  padding: 1.25rem;
  border-bottom: 1px solid #e5e7eb;
}

.cart-header h3 {
  font-size: 1.1rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
}

.empty-cart {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1.5rem;
  color: #9ca3af;
  text-align: center;
}

.empty-cart p {
  margin: 1rem 0 0.25rem;
  font-size: 1rem;
  color: #6b7280;
}

.empty-cart span {
  font-size: 0.875rem;
}

.cart-items {
  max-height: 400px;
  overflow-y: auto;
}

.cart-item {
  display: grid;
  grid-template-columns: 44px 1fr 70px 70px 32px;
  gap: 0.5rem;
  align-items: center;
  padding: 0.625rem 1rem;
  border-bottom: 1px solid #f3f4f6;
}

.cart-item:hover {
  background: #f9fafb;
}

.item-image {
  width: 44px;
  height: 44px;
  border-radius: 6px;
  overflow: hidden;
  background: #f3f4f6;
  flex-shrink: 0;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.item-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #9ca3af;
}

.item-details {
  min-width: 0;
}

.item-name {
  display: block;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #1f2937;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.item-price {
  font-size: 0.6875rem;
  color: #6b7280;
}

.item-quantity {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.25rem;
}

.item-quantity span {
  min-width: 20px;
  text-align: center;
  font-size: 0.8125rem;
  font-weight: 500;
}

.qty-btn-small {
  width: 20px;
  height: 20px;
  border: none;
  border-radius: 6px;
  background: #f3f4f6;
  color: #374151;
  cursor: pointer;
  font-size: 0.875rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.qty-btn-small:hover {
  background: #e5e7eb;
}

.item-subtotal {
  font-weight: 600;
  color: #1f2937;
  min-width: 70px;
  text-align: right;
}

.btn-remove {
  background: none;
  border: none;
  padding: 0.5rem;
  cursor: pointer;
  color: #9ca3af;
  border-radius: 6px;
  transition: all 0.15s;
}

.btn-remove:hover {
  background: #fee2e2;
  color: #dc2626;
}

.cart-footer {
  padding: 1.25rem;
  border-top: 1px solid #e5e7eb;
  background: #f9fafb;
}

.cart-total-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.total-label {
  font-size: 0.95rem;
  color: #6b7280;
}

.total-amount {
  font-size: 1.5rem;
  font-weight: 700;
  color: #059669;
}

.btn-register {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  background: #10b981;
  color: white;
  padding: 1rem;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-register:hover:not(:disabled) {
  background: #059669;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
}

.btn-register:disabled {
  background: #a7f3d0;
  color: #6ee7b7;
  cursor: not-allowed;
}

@media (max-width: 1024px) {
  .sales-layout {
    grid-template-columns: 1fr;
  }
  
  .cart-section {
    position: static;
  }
}
</style>
