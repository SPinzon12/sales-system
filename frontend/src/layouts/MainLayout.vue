<template>
  <div class="layout">
    <aside class="sidebar">
      <div class="logo">
        <PhStorefront :size="28" />
        <span>Ventas</span>
      </div>
      
      <nav>
        <router-link to="/" class="nav-link" active-class="" :class="{ 'active': route.path === '/' }">
          <PhPackage :size="20" />
          <span>Productos</span>
        </router-link>
        <router-link to="/sales" class="nav-link" active-class="" :class="{ 'active': route.path === '/sales' }">
          <PhShoppingCart :size="20" />
          <span>Ventas</span>
        </router-link>
        <router-link to="/reports" class="nav-link" active-class="" :class="{ 'active': route.path === '/reports' }">
          <PhChartLine :size="20" />
          <span>Reportes</span>
        </router-link>
      </nav>
      
      <div class="user-section">
        <div class="user-info">
          <PhUser :size="18" />
          <span>{{ authStore.user?.username || 'Usuario' }}</span>
        </div>
        <button @click="handleLogout" class="logout-btn" title="Cerrar Sesión">
          <PhSignOut :size="20" />
        </button>
      </div>
    </aside>
    
    <main class="main-content">
      <router-view></router-view>
    </main>
  </div>
</template>

<script setup>
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import {
  PhStorefront,
  PhPackage,
  PhShoppingCart,
  PhChartLine,
  PhUser,
  PhSignOut
} from '@phosphor-icons/vue'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.layout {
  display: flex;
  min-height: 100vh;
}

.sidebar {
  width: 240px;
  background: #111;
  color: #fff;
  padding: 0;
  display: flex;
  flex-direction: column;
  position: fixed;
  height: 100vh;
}

.logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1.5rem 1.25rem;
  border-bottom: 1px solid #222;
}

.logo svg {
  color: #10b981;
}

.logo span {
  font-size: 1.125rem;
  font-weight: 600;
  letter-spacing: -0.02em;
}

.sidebar nav {
  display: flex;
  flex-direction: column;
  padding: 1rem 0.75rem;
  gap: 0.25rem;
  flex: 1;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  color: #888;
  text-decoration: none;
  font-size: 0.9375rem;
  font-weight: 500;
  border-radius: 8px;
  transition: all 0.15s ease;
  position: relative;
}

.nav-link:hover {
  color: #fff;
  background: #1a1a1a;
}

.nav-link.active {
  color: #fff;
  background: #1a1a1a;
}

.nav-link.active::before {
  content: '';
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 3px;
  height: 24px;
  background: #10b981;
  border-radius: 0 2px 2px 0;
}

.nav-link svg {
  flex-shrink: 0;
}

.user-section {
  padding: 1rem;
  border-top: 1px solid #222;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #666;
  font-size: 0.8125rem;
}

.logout-btn {
  background: transparent;
  border: none;
  color: #666;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.15s ease;
}

.logout-btn:hover {
  background: #1a1a1a;
  color: #fff;
}

.main-content {
  flex: 1;
  margin-left: 240px;
  background: #fafafa;
  min-height: 100vh;
}
</style>
