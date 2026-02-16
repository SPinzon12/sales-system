<template>
  <div class="reports-container">
    <div class="header">
      <h2>Reportes de Ventas</h2>
    </div>

    <div class="filters">
      <div class="filter-group">
        <label>Desde</label>
        <input v-model="from" type="date" class="filter-input" />
      </div>
      <div class="filter-group">
        <label>Hasta</label>
        <input v-model="to" type="date" class="filter-input" />
      </div>
      <BaseButton variant="primary" :icon="PhMagnifyingGlass" @click="loadReport">
        Buscar
      </BaseButton>
    </div>

    <LoadingSpinner v-if="productsStore.reportLoading" message="Cargando reporte..." />

    <template v-else-if="productsStore.report">
      <div class="summary">
        <div class="summary-card">
          <PhCurrencyDollar :size="24" class="summary-icon" />
          <h4>Total Ventas</h4>
          <p class="value">${{ formatNumber(productsStore.report.summary?.totalAmount || 0, 2) }}</p>
        </div>
        <div class="summary-card">
          <PhReceipt :size="24" class="summary-icon" />
          <h4>Cantidad Ventas</h4>
          <p class="value">{{ productsStore.report.pagination?.totalCount || 0 }}</p>
        </div>
        <div class="summary-card">
          <PhShoppingBag :size="24" class="summary-icon" />
          <h4>Total Ítems</h4>
          <p class="value">
            <span v-if="productsStore.saleDetailsLoading" class="loading-small">...</span>
            <span v-else>{{ totalItems }}</span>
          </p>
        </div>
      </div>

      <div class="table-wrapper" v-if="productsStore.report.items?.length > 0">
        <table class="report-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Fecha</th>
              <th>Total</th>
              <th>Estado</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="sale in productsStore.report.items" :key="sale.id" @click="viewSaleDetail(sale.id)" class="clickable-row">
              <td class="id-cell">{{ sale.id.substring(0, 8) }}...</td>
              <td>{{ formatDate(sale.saleDate) }}</td>
              <td>
                <span class="price-badge">${{ formatNumber(sale.total || 0, 2) }}</span>
              </td>
              <td>
                <span class="status-badge" :class="sale.status?.toLowerCase()">
                  {{ sale.status }}
                </span>
              </td>
              <td class="action-cell">
                <PhEye :size="18" />
              </td>
            </tr>
          </tbody>
        </table>

        <div class="pagination" v-if="productsStore.report.pagination?.totalPages > 1">
          <button @click="changePage(-1)" :disabled="page <= 1">
            <PhCaretLeft :size="16" />
          </button>
          <span>Página {{ page }} de {{ productsStore.report.pagination?.totalPages || 1 }}</span>
          <button @click="changePage(1)" :disabled="page >= (productsStore.report.pagination?.totalPages || 1)">
            <PhCaretRight :size="16" />
          </button>
        </div>
      </div>

      <div v-else class="empty">
        <PhChartLine :size="48" />
        <p>No hay ventas en el período seleccionado</p>
      </div>
    </template>

    <div v-else class="no-data">
      <PhChartLine :size="64" />
      <p>Selecciona un rango de fechas para ver el reporte</p>
    </div>

    <!-- Sale Detail Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="productsStore.selectedSale" class="modal-overlay" @click.self="closeModal">
          <div class="modal-container">
            <div class="modal-header">
              <div class="modal-title">
                <PhReceipt :size="24" />
                <h3>Detalle de Venta</h3>
              </div>
              <button class="modal-close" @click="closeModal">
                <PhX :size="24" />
              </button>
            </div>

            <div v-if="productsStore.selectedSaleLoading" class="modal-loading">
              <PhSpinner :size="32" class="spinner" />
            </div>

            <div v-else class="modal-body">
              <div class="sale-info">
                <div class="info-row">
                  <span class="info-label">ID</span>
                  <span class="info-value">{{ productsStore.selectedSale.id }}</span>
                </div>
                <div class="info-row">
                  <span class="info-label">Fecha</span>
                  <span class="info-value">{{ formatDate(productsStore.selectedSale.saleDate) }}</span>
                </div>
                <div class="info-row">
                  <span class="info-label">Estado</span>
                  <span class="status-badge" :class="productsStore.selectedSale.status?.toLowerCase()">
                    {{ productsStore.selectedSale.status }}
                  </span>
                </div>
                <div class="info-row">
                  <span class="info-label">Total</span>
                  <span class="info-value total">${{ formatNumber(productsStore.selectedSale.total || 0, 2) }}</span>
                </div>
              </div>

              <h4 class="items-title">Items de la Venta</h4>
              
              <div class="items-list" v-if="productsStore.selectedSale.items?.length > 0">
                <div v-for="item in productsStore.selectedSale.items" :key="item.productId" class="item-row">
                  <div class="item-info">
                    <span class="item-product-name">{{ getProductName(item.productId) }}</span>
                    <span class="item-qty">x{{ item.quantity }}</span>
                  </div>
                  <div class="item-prices">
                    <span class="item-unit-price">${{ formatNumber(item.unitPrice || 0, 2) }} c/u</span>
                    <span class="item-subtotal">${{ formatNumber(item.subTotal || 0, 2) }}</span>
                  </div>
                </div>
              </div>

              <div v-else class="no-items">
                <p>No hay items en esta venta</p>
              </div>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useProductsStore } from '../stores/products'
import {
  PhMagnifyingGlass,
  PhCurrencyDollar,
  PhReceipt,
  PhChartLine,
  PhCaretLeft,
  PhCaretRight,
  PhCalendarBlank,
  PhEye,
  PhX,
  PhShoppingBag,
  PhSpinner
} from '@phosphor-icons/vue'
import LoadingSpinner from '../components/ui/LoadingSpinner.vue'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseModal from '../components/ui/BaseModal.vue'

const productsStore = useProductsStore()

const from = ref('')
const to = ref('')
const page = ref(1)
const pageSize = ref(10)

const dateRange = computed(() => {
  if (!from.value || !to.value) return '-'
  return `${from.value} - ${to.value}`
})

const totalItems = computed(() => {
  if (!productsStore.saleDetails || Object.keys(productsStore.saleDetails).length === 0) return 0
  return Object.values(productsStore.saleDetails).reduce((sum, sale) => {
    const itemQty = sale.items?.reduce((itemSum, item) => itemSum + (item.quantity || 0), 0) || 0
    return sum + itemQty
  }, 0)
})

const formatNumber = (num, decimals = 2) => {
  const fixed = num.toFixed(decimals)
  const parts = fixed.split('.')
  parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, '.')
  return parts.join(',')
}

const getProductName = (productId) => {
  const product = productsStore.productsMap[productId]
  return product?.name || 'Producto desconocido'
}

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('es-ES', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const loadReport = async () => {
  if (!from.value || !to.value) {
    return
  }
  page.value = 1
  await productsStore.fetchReport(from.value, to.value, page.value, pageSize.value)
}

const changePage = async (delta) => {
  page.value += delta
  await productsStore.fetchReport(from.value, to.value, page.value, pageSize.value)
}

const viewSaleDetail = (id) => {
  if (productsStore.saleDetails[id]) {
    productsStore.selectedSale = productsStore.saleDetails[id]
  }
}

const closeModal = () => {
  productsStore.clearSelectedSale()
}

onMounted(() => {
  const today = new Date()
  const lastMonth = new Date()
  lastMonth.setMonth(lastMonth.getMonth() - 1)
  
  to.value = today.toISOString().split('T')[0]
  from.value = lastMonth.toISOString().split('T')[0]
  loadReport()
})
</script>

<style scoped>
.reports-container {
  padding: 2rem;
  max-width: 1000px;
  margin: 0 auto;
}

.header {
  margin-bottom: 1.5rem;
}

.header h2 {
  font-size: 1.75rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
}

.filters {
  display: flex;
  gap: 1rem;
  align-items: flex-end;
  background: white;
  padding: 1.25rem;
  border-radius: 12px;
  margin-bottom: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.filter-group label {
  font-size: 0.8125rem;
  font-weight: 500;
  color: #6b7280;
}

.filter-input {
  padding: 0.625rem 0.875rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.875rem;
  transition: border-color 0.15s;
}

.filter-input:focus {
  outline: none;
  border-color: #10b981;
}

.btn-search {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  background: #10b981;
  color: white;
  padding: 0.625rem 1.25rem;
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 500;
  font-family: inherit;
  cursor: pointer;
  transition: all 0.2s;
  line-height: 1;
}

.btn-search:hover {
  background: #059669;
}

.loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem;
  color: #6b7280;
}

.loading .spinner {
  animation: spin 1s linear infinite;
  color: #10b981;
}

.loading-small {
  font-size: 0.875rem;
  color: #10b981;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.summary {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.summary-card {
  background: white;
  padding: 1.25rem;
  border-radius: 12px;
  text-align: center;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
}

.summary-icon {
  color: #10b981;
  margin-bottom: 0.5rem;
}

.summary-card h4 {
  margin: 0 0 0.5rem;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #6b7280;
}

.summary-card .value {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: #059669;
}

.table-wrapper {
  background: white;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  overflow: hidden;
}

.report-table {
  width: 100%;
  border-collapse: collapse;
}

.report-table th {
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  color: #374151;
  font-size: 0.8125rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  background: #f9fafb;
  border-bottom: 1px solid #e5e7eb;
}

.report-table td {
  padding: 1rem;
  border-bottom: 1px solid #f3f4f6;
  font-size: 0.875rem;
  color: #1f2937;
}

.clickable-row {
  cursor: pointer;
  transition: background 0.15s;
}

.clickable-row:hover {
  background: #f9fafb;
}

.id-cell {
  font-family: monospace;
  color: #6b7280;
}

.action-cell {
  text-align: center;
  color: #9ca3af;
}

.price-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  background: #ecfdf5;
  color: #059669;
  border-radius: 6px;
  font-weight: 600;
  font-size: 0.8125rem;
}

.status-badge {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 500;
}

.status-badge.confirmed {
  background: #d1fae5;
  color: #065f46;
}

.status-badge.pending {
  background: #fef3c7;
  color: #92400e;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  border-top: 1px solid #f3f4f6;
}

.pagination button {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border: 1px solid #e5e7eb;
  background: white;
  border-radius: 6px;
  cursor: pointer;
  color: #374151;
  font-family: inherit;
  transition: all 0.15s;
}

.pagination button:hover:not(:disabled) {
  background: #f3f4f6;
}

.pagination button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination span {
  font-size: 0.875rem;
  color: #6b7280;
}

.empty, .no-data {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem;
  background: white;
  border-radius: 12px;
  color: #9ca3af;
  text-align: center;
}

.empty p, .no-data p {
  margin: 1rem 0 0;
  font-size: 0.9375rem;
  color: #6b7280;
}

/* Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  backdrop-filter: blur(4px);
}

.modal-container {
  background: white;
  border-radius: 16px;
  width: 100%;
  max-width: 480px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.2);
  overflow: hidden;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.modal-title {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  color: #1f2937;
}

.modal-title h3 {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
}

.modal-title svg {
  color: #10b981;
}

.modal-close {
  background: none;
  border: none;
  padding: 0.5rem;
  cursor: pointer;
  color: #6b7280;
  border-radius: 8px;
  transition: all 0.2s;
}

.modal-close:hover {
  background: #f3f4f6;
  color: #1f2937;
}

.modal-loading {
  display: flex;
  justify-content: center;
  padding: 3rem;
}

.modal-body {
  padding: 1.5rem;
}

.sale-info {
  background: #f9fafb;
  border-radius: 10px;
  padding: 1rem;
  margin-bottom: 1.5rem;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
}

.info-row:not(:last-child) {
  border-bottom: 1px solid #e5e7eb;
}

.info-label {
  font-size: 0.8125rem;
  color: #6b7280;
}

.info-value {
  font-size: 0.875rem;
  color: #1f2937;
  font-family: monospace;
}

.info-value.total {
  font-size: 1.125rem;
  font-weight: 700;
  color: #059669;
}

.items-title {
  font-size: 0.9375rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 1rem;
}

.items-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.item-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background: #f9fafb;
  border-radius: 8px;
}

.item-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.item-product-name {
  font-size: 0.875rem;
  font-weight: 500;
  color: #1f2937;
}

.item-qty {
  font-size: 0.875rem;
  font-weight: 600;
  color: #1f2937;
}

.item-prices {
  text-align: right;
}

.item-unit-price {
  display: block;
  font-size: 0.75rem;
  color: #6b7280;
}

.item-subtotal {
  display: block;
  font-size: 0.875rem;
  font-weight: 600;
  color: #059669;
}

.no-items {
  text-align: center;
  padding: 2rem;
  color: #9ca3af;
}

.modal-enter-active,
.modal-leave-active {
  transition: all 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .modal-container,
.modal-leave-to .modal-container {
  transform: scale(0.95) translateY(-20px);
}

.modal-enter-active .modal-container,
.modal-leave-active .modal-container {
  transition: all 0.3s ease;
}

@media (max-width: 768px) {
  .summary {
    grid-template-columns: 1fr;
  }
  
  .filters {
    flex-wrap: wrap;
  }
  
  .report-table th:nth-child(1),
  .report-table td:nth-child(1) {
    display: none;
  }
}
</style>
