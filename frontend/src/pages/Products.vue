<template>
  <div class="products-container">
    <div class="header">
      <h2>Gestión de Productos</h2>
      <BaseButton variant="primary" :icon="PhPlus" @click="openModal">
        Nuevo Producto
      </BaseButton>
    </div>

    <LoadingSpinner v-if="productsStore.loading" message="Cargando productos..." />
    
    <EmptyState 
      v-else-if="productsStore.products.length === 0" 
      :icon="PhPackage" 
      title="No hay productos disponibles"
    >
      <template #action>
        <BaseButton variant="primary" :icon="PhPlus" size="sm" @click="openModal">
          Agregar primer producto
        </BaseButton>
      </template>
    </EmptyState>

    <template v-else>
      <div class="table-wrapper">
        <table class="products-table">
          <thead>
            <tr>
              <th class="th-image">Imagen</th>
              <th class="th-name">Producto</th>
              <th class="th-price">Precio</th>
              <th class="th-stock">Stock</th>
              <th class="th-actions">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="product in productsStore.products" :key="product.id">
              <td class="td-image">
                <div class="image-container">
                  <img v-if="product.imageUrl" :src="product.imageUrl" :alt="product.name" />
                  <div v-else class="image-placeholder">
                    <PhPackage :size="24" />
                  </div>
                </div>
              </td>
              <td class="td-name">
                <span class="product-name">{{ product.name }}</span>
              </td>
              <td class="td-price">
                <span class="price-badge">${{ formatNumber(product.price, 2) }}</span>
              </td>
              <td class="td-stock">
                <span 
                  class="stock-badge" 
                  :class="{ 'low-stock': product.stock <= 5, 'out-of-stock': product.stock === 0 }"
                >
                  <PhCube :size="16" />
                  {{ formatNumber(product.stock, 0) }}
                </span>
              </td>
              <td class="td-actions">
                <BaseButton variant="edit" size="sm" :icon="PhPencil" @click="editProduct(product)" />
                <BaseButton variant="delete" size="sm" :icon="PhTrash" @click="confirmDelete(product.id)" />
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </template>

    <BaseModal 
      :show="showModal" 
      :title="editingProduct ? 'Editar Producto' : 'Nuevo Producto'" 
      :icon="editingProduct ? PhPencil : PhPlus"
      @close="closeModal"
    >
      <form @submit.prevent="saveProduct">
        <div class="form-group">
          <label>
            <PhTag :size="18" />
            Nombre del producto
          </label>
          <div class="input-wrapper">
            <input 
              v-model="form.name" 
              type="text" 
              placeholder="Ingresa el nombre del producto"
              required 
            />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>
              <PhCurrencyDollar :size="18" />
              Precio
            </label>
            <div class="input-wrapper">
              <input 
                v-model.number="form.price" 
                type="number" 
                step="0.01" 
                min="0" 
                placeholder="0.00"
                required 
              />
            </div>
          </div>

          <div class="form-group">
            <label>
              <PhCube :size="18" />
              Stock
            </label>
            <div class="input-wrapper">
              <input 
                v-model.number="form.stock" 
                type="number" 
                min="0" 
                placeholder="0"
                required 
              />
            </div>
          </div>
        </div>

        <div class="form-group">
          <label>
            <PhImage :size="18" />
            URL de Imagen
          </label>
          <div class="input-wrapper">
            <input 
              v-model="form.imageUrl" 
              type="text" 
              placeholder="https://ejemplo.com/imagen.jpg"
            />
          </div>
        </div>
      </form>

      <template #footer>
        <BaseButton variant="secondary" :icon="PhX" @click="closeModal">
          Cancelar
        </BaseButton>
        <BaseButton variant="primary" :icon="PhCheck" :loading="productsStore.loading" @click="saveProduct">
          {{ productsStore.loading ? 'Guardando...' : 'Guardar' }}
        </BaseButton>
      </template>
    </BaseModal>

    <ConfirmDialog
      :show="showDeleteConfirm"
      title="Eliminar Producto"
      message="¿Está seguro de eliminar este producto? Esta acción no se puede deshacer."
      :loading="productsStore.loading"
      @confirm="deleteProduct"
      @cancel="showDeleteConfirm = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useProductsStore } from '../stores/products'
import { toast } from 'vue3-toastify'
import {
  PhPlus,
  PhPencil,
  PhTrash,
  PhX,
  PhCheck,
  PhPackage,
  PhCube,
  PhCurrencyDollar,
  PhTag,
  PhImage
} from '@phosphor-icons/vue'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseModal from '../components/ui/BaseModal.vue'
import LoadingSpinner from '../components/ui/LoadingSpinner.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ConfirmDialog from '../components/ui/ConfirmDialog.vue'

const productsStore = useProductsStore()

const showModal = ref(false)
const showDeleteConfirm = ref(false)
const deleteTargetId = ref(null)
const editingProduct = ref(null)
const form = ref({ name: '', price: 0, stock: 0, imageUrl: '' })

const formatNumber = (num, decimals = 2) => {
  const fixed = num.toFixed(decimals)
  const parts = fixed.split('.')
  parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, '.')
  return parts.join(',')
}

const editProduct = (product) => {
  editingProduct.value = product
  form.value = { ...product }
  showModal.value = true
}

const saveProduct = async () => {
  try {
    if (editingProduct.value) {
      await productsStore.updateProduct(editingProduct.value.id, form.value)
    } else {
      await productsStore.createProduct(form.value)
    }
    closeModal()
  } catch (error) {
    toast.error('Error al guardar el producto')
  }
}

const confirmDelete = (id) => {
  deleteTargetId.value = id
  showDeleteConfirm.value = true
}

const deleteProduct = async () => {
  if (deleteTargetId.value) {
    await productsStore.deleteProduct(deleteTargetId.value)
  }
  showDeleteConfirm.value = false
  deleteTargetId.value = null
}

const closeModal = () => {
  showModal.value = false
  editingProduct.value = null
  form.value = { name: '', price: 0, stock: 0, imageUrl: '' }
}

const openModal = () => {
  showModal.value = true
}

onMounted(() => {
  productsStore.fetchProducts()
})
</script>

<style scoped>
.products-container {
  padding: 2rem;
  max-width: 1200px;
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

.table-wrapper {
  background: white;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.products-table {
  width: 100%;
  border-collapse: collapse;
}

.products-table thead {
  background: #f9fafb;
}

.products-table th {
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  color: #374151;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.products-table td {
  padding: 1rem;
  border-bottom: 1px solid #f3f4f6;
}

.products-table tbody tr {
  transition: background 0.15s ease;
}

.products-table tbody tr:hover {
  background: #f9fafb;
}

.products-table tbody tr:nth-child(even) {
  background: #fafafa;
}

.products-table tbody tr:nth-child(even):hover {
  background: #f5f5f5;
}

.th-image { width: 80px; }
.th-name { min-width: 200px; }
.th-price { width: 120px; }
.th-stock { width: 140px; }
.th-actions { width: 100px; text-align: center; }

.image-container {
  width: 48px;
  height: 48px;
  border-radius: 8px;
  overflow: hidden;
  background: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
}

.image-container img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.2s ease;
}

.image-container:hover img {
  transform: scale(1.1);
}

.image-placeholder {
  color: #9ca3af;
}

.product-name {
  font-weight: 500;
  color: #1f2937;
}

.price-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  background: #ecfdf5;
  color: #059669;
  border-radius: 6px;
  font-weight: 600;
  font-size: 0.9rem;
}

.stock-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 500;
}

.stock-badge:not(.low-stock):not(.out-of-stock) {
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

.td-actions {
  text-align: center;
  display: flex;
  justify-content: center;
  gap: 0.25rem;
}

.form-group {
  margin-bottom: 1.25rem;
}

.form-group label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.input-wrapper input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 0.95rem;
  transition: all 0.2s ease;
  background: white;
  font-family: inherit;
}

.input-wrapper input:focus {
  outline: none;
  border-color: #10b981;
  box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.1);
}

.input-wrapper input::placeholder {
  color: #9ca3af;
}
</style>
