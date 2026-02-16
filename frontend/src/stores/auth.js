import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '../api/axios'
import { toast } from 'vue3-toastify'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || '')
  const user = ref(JSON.parse(localStorage.getItem('user') || 'null'))
  const loading = ref(false)
  const error = ref('')

  const isAuthenticated = computed(() => !!token.value)

  const login = async (username, password) => {
    loading.value = true
    error.value = ''
    try {
      const response = await authApi.login({ username, password })
      token.value = response.data.token
      user.value = response.data.user
      localStorage.setItem('token', response.data.token)
      localStorage.setItem('user', JSON.stringify(response.data.user))
      toast.success('Bienvenido!')
    } catch (err) {
      error.value = err.response?.data?.error || 'Error al iniciar sesión'
      toast.error(error.value)
      throw err
    } finally {
      loading.value = false
    }
  }

  const logout = () => {
    token.value = ''
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    toast.info('Sesión cerrada')
  }

  return {
    token,
    user,
    loading,
    error,
    isAuthenticated,
    login,
    logout
  }
})
