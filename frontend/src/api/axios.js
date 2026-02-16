import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL || (import.meta.env.PROD ? '/api' : 'http://localhost:5000')

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export const authApi = {
  login: (credentials) => api.post('/auth/login', credentials)
}

export const productsApi = {
  getAll: (page = 1, pageSize = 10) => api.get(`/products?page=${page}&pageSize=${pageSize}`),
  getById: (id) => api.get(`/products/${id}`),
  create: (product) => api.post('/products', product),
  update: (id, product) => api.put(`/products/${id}`, product),
  delete: (id) => api.delete(`/products/${id}`)
}

export const salesApi = {
  create: (sale) => api.post('/sales', sale),
  getReport: (from, to, page = 1, pageSize = 10) => 
    api.get(`/sales/report?from=${from}&to=${to}&page=${page}&pageSize=${pageSize}`),
  getById: (id) => api.get(`/sales/${id}`)
}

export default api
