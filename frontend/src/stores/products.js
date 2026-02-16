import { defineStore } from 'pinia'
import { ref } from 'vue'
import { productsApi, salesApi } from '../api/axios'
import { toast } from 'vue3-toastify'

export const useProductsStore = defineStore('products', () => {
  const products = ref([])
  const productsMap = ref({})
  const page = ref(1)
  const pageSize = ref(10)
  const total = ref(0)
  const loading = ref(false)
  const report = ref(null)
  const reportTotal = ref(0)
  const reportLoading = ref(false)
  const selectedSale = ref(null)
  const selectedSaleLoading = ref(false)
  const saleDetails = ref({})
  const saleDetailsLoading = ref(false)

  const fetchProducts = async (params = {}) => {
    loading.value = true
    try {
      const response = await productsApi.getAll(
        params.page || page.value,
        params.pageSize || pageSize.value
      )
      let productsList = []
      if (Array.isArray(response.data)) {
        productsList = response.data
        total.value = response.data.length
      } else {
        productsList = response.data.items || []
        total.value = response.data.total || 0
      }
      products.value = productsList
      productsList.forEach(p => {
        productsMap.value[p.id] = p
      })
    } catch (error) {
      console.error('Error loading products:', error)
      toast.error('Error al cargar productos')
    } finally {
      loading.value = false
    }
  }

  const setPage = (newPage) => {
    page.value = newPage
    fetchProducts()
  }

  const createProduct = async (product) => {
    await productsApi.create(product)
    await fetchProducts()
    toast.success('Producto creado exitosamente')
    return true
  }

  const updateProduct = async (id, product) => {
    await productsApi.update(id, product)
    await fetchProducts()
    toast.success('Producto actualizado exitosamente')
    return true
  }

  const deleteProduct = async (id) => {
    await productsApi.delete(id)
    await fetchProducts()
    toast.success('Producto eliminado exitosamente')
    return true
  }

  const createSale = async (saleData) => {
    loading.value = true
    try {
      await salesApi.create(saleData)
      await fetchProducts()
      toast.success('Venta registrada exitosamente')
      return true
    } catch (error) {
      toast.error(error.response?.data?.error || 'Error al registrar venta')
      return false
    } finally {
      loading.value = false
    }
  }

  const fetchReport = async (from, to, pageNum = 1, pageSizeNum = 10) => {
    reportLoading.value = true
    saleDetailsLoading.value = true
    saleDetails.value = {}
    try {
      const response = await salesApi.getReport(from, to, pageNum, pageSizeNum)
      report.value = response.data
      reportTotal.value = response.data.pagination?.totalCount || 0
      
      if (response.data.items?.length > 0) {
        const saleIds = response.data.items.map(s => s.id)
        await fetchSalesDetails(saleIds)
      }
    } catch (error) {
      toast.error('Error al cargar el reporte')
    } finally {
      reportLoading.value = false
      saleDetailsLoading.value = false
    }
  }

  const fetchSalesDetails = async (saleIds) => {
    try {
      const promises = saleIds.map(id => salesApi.getById(id))
      const responses = await Promise.all(promises)
      
      const details = {}
      const productIdsNeeded = new Set()
      
      responses.forEach(response => {
        const sale = response.data
        details[sale.id] = sale
        if (sale.items) {
          sale.items.forEach(item => {
            if (!productsMap.value[item.productId]) {
              productIdsNeeded.add(item.productId)
            }
          })
        }
      })
      saleDetails.value = details
      
      if (productIdsNeeded.size > 0) {
        await fetchProductsByIds(Array.from(productIdsNeeded))
      }
    } catch (error) {
      console.error('Error loading sale details:', error)
    }
  }

  const fetchProductsByIds = async (ids) => {
    try {
      for (const id of ids) {
        const response = await productsApi.getById(id)
        if (response.data) {
          productsMap.value[id] = response.data
        }
      }
    } catch (error) {
      console.error('Error loading products by ids:', error)
    }
  }

  const fetchSaleById = async (id) => {
    selectedSaleLoading.value = true
    selectedSale.value = null
    try {
      const response = await salesApi.getById(id)
      selectedSale.value = response.data
    } catch (error) {
      toast.error('Error al cargar los detalles de la venta')
    } finally {
      selectedSaleLoading.value = false
    }
  }

  const clearSelectedSale = () => {
    selectedSale.value = null
  }

  return {
    products,
    productsMap,
    page,
    pageSize,
    total,
    loading,
    report,
    reportTotal,
    reportLoading,
    selectedSale,
    selectedSaleLoading,
    saleDetails,
    saleDetailsLoading,
    fetchProducts,
    setPage,
    createProduct,
    updateProduct,
    deleteProduct,
    createSale,
    fetchReport,
    fetchSalesDetails,
    fetchSaleById,
    clearSelectedSale
  }
})
