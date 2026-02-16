<template>
  <div class="loading-container">
    <PhSpinner :size="size" class="spinner" />
    <p v-if="message">{{ message }}</p>
  </div>
</template>

<script setup>
import { PhSpinner } from '@phosphor-icons/vue'

defineProps({
  message: {
    type: String,
    default: ''
  },
  size: {
    type: Number,
    default: 40
  }
})
</script>

<style scoped>
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem;
  color: #6b7280;
}

.spinner {
  animation: spin 1s linear infinite;
  color: #10b981;
}

.loading-container p {
  margin: 1rem 0 0;
  font-size: 0.9375rem;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}
</style>
