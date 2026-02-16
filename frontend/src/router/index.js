import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import Login from '../pages/Login.vue'
import Products from '../pages/Products.vue'
import Sales from '../pages/Sales.vue'
import Reports from '../pages/Reports.vue'
import MainLayout from '../layouts/MainLayout.vue'

const routes = [
  { path: '/login', name: 'Login', component: Login },
  {
    path: '/',
    component: MainLayout,
    meta: { requiresAuth: true },
    children: [
      { path: '', name: 'Products', component: Products },
      { path: 'sales', name: 'Sales', component: Sales },
      { path: 'reports', name: 'Reports', component: Reports }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    next('/')
  } else {
    next()
  }
})

export default router
