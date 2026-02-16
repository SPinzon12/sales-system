<template>
  <div class="empty-container">
    <component :is="icon" :size="iconSize" />
    <p>{{ title }}</p>
    <slot name="action"></slot>
  </div>
</template>

<script setup>
import { PhPackage, PhShoppingCart, PhChartLine } from '@phosphor-icons/vue'

defineProps({
  icon: {
    type: Object,
    default: () => PhPackage
  },
  iconSize: {
    type: Number,
    default: 64
  },
  title: {
    type: String,
    default: 'No hay datos'
  }
})
</script>

<style scoped>
.empty-container {
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

.empty-container p {
  margin: 1rem 0;
  font-size: 1.1rem;
  color: #6b7280;
}

.empty-container :deep(svg) {
  color: #9ca3af;
}
</style>
