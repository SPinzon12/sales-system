<template>
  <div class="login-container">
    <div class="login-card">
      <div class="login-header">
        <PhStorefront :size="40" />
        <h1>Sistema de Ventas</h1>
      </div>
      
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label>
            <PhUser :size="16" />
            Usuario
          </label>
          <div class="input-wrapper">
            <input v-model="username" type="text" placeholder="Ingresa tu usuario" required />
          </div>
        </div>
        
        <div class="form-group">
          <label>
            <PhLock :size="16" />
            Contraseña
          </label>
          <div class="input-wrapper">
            <input v-model="password" type="password" placeholder="Ingresa tu contraseña" required />
          </div>
        </div>
        
        <div class="form-actions">
          <BaseButton type="submit" variant="primary" :icon="PhSignIn" :loading="authStore.loading" :disabled="authStore.loading">
            {{ authStore.loading ? 'Ingresando...' : 'Iniciar Sesión' }}
          </BaseButton>
        </div>
        
        <p v-if="authStore.error" class="error">
          <PhWarning :size="16" />
          {{ authStore.error }}
        </p>
      </form>
      
      <p class="hint">
        <PhInfo :size="14" />
        Credenciales: admin / admin123
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import {
  PhStorefront,
  PhUser,
  PhLock,
  PhSignIn,
  PhWarning,
  PhInfo
} from '@phosphor-icons/vue'
import BaseButton from '../components/ui/BaseButton.vue'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('')
const password = ref('')

const handleLogin = async () => {
  try {
    await authStore.login(username.value, password.value)
    if (authStore.isAuthenticated) {
      router.push('/')
    }
  } catch (err) {
    // Error already handled in store
  }
}
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #111;
}

.login-card {
  background: #1a1a1a;
  padding: 2.5rem;
  border-radius: 16px;
  border: 1px solid #222;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4);
  width: 100%;
  max-width: 380px;
}

.login-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2rem;
}

.login-header svg {
  color: #10b981;
}

.login-header h1 {
  color: #fff;
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0;
  letter-spacing: -0.02em;
}

.form-group {
  margin-bottom: 1.25rem;
}

.form-group label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #888;
  font-size: 0.8125rem;
  font-weight: 500;
  margin-bottom: 0.5rem;
}

.input-wrapper input {
  width: 100%;
  padding: 0.875rem 1rem;
  background: #222;
  border: 1px solid #333;
  border-radius: 8px;
  font-size: 0.9375rem;
  color: #fff;
  transition: all 0.2s ease;
}

.input-wrapper input::placeholder {
  color: #555;
}

.input-wrapper input:focus {
  outline: none;
  border-color: #10b981;
  box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.15);
}

.form-actions {
  margin-top: 0.5rem;
}

.form-actions :deep(.base-btn) {
  width: 100%;
}

.error {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  color: #ef4444;
  font-size: 0.8125rem;
  margin-top: 1rem;
  padding: 0.75rem;
  background: rgba(239, 68, 68, 0.1);
  border-radius: 8px;
}

.hint {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.375rem;
  color: #555;
  font-size: 0.75rem;
  text-align: center;
  margin-top: 1.5rem;
  padding-top: 1.5rem;
  border-top: 1px solid #222;
}
</style>
