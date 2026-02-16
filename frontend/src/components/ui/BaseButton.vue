<template>
  <button
    :class="['base-btn', `base-btn--${variant}`, `base-btn--${size}`]"
    :disabled="disabled || loading"
    @click="$emit('click', $event)"
  >
    <PhSpinner v-if="loading" :size="iconSize" class="spinner" />
    <component v-else-if="icon" :is="icon" :size="iconSize" />
    <span v-if="$slots.default || label"><slot>{{ label }}</slot></span>
  </button>
</template>

<script setup>
import { PhSpinner } from '@phosphor-icons/vue'

defineProps({
  variant: {
    type: String,
    default: 'primary',
    validator: (v) => ['primary', 'secondary', 'danger', 'ghost', 'edit', 'delete'].includes(v)
  },
  size: {
    type: String,
    default: 'md',
    validator: (v) => ['sm', 'md', 'lg'].includes(v)
  },
  disabled: {
    type: Boolean,
    default: false
  },
  loading: {
    type: Boolean,
    default: false
  },
  icon: {
    type: Object,
    default: null
  },
  iconSize: {
    type: Number,
    default: 18
  },
  label: {
    type: String,
    default: ''
  }
})

defineEmits(['click'])
</script>

<style scoped>
.base-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  border: none;
  border-radius: 8px;
  font-family: inherit;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  line-height: 1;
}

.base-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Sizes */
.base-btn--sm {
  padding: 0.375rem 0.75rem;
  font-size: 0.8125rem;
}

.base-btn--md {
  padding: 0.625rem 1.25rem;
  font-size: 0.875rem;
}

.base-btn--lg {
  padding: 0.75rem 1.5rem;
  font-size: 0.95rem;
}

/* Variants */
.base-btn--primary {
  background: #10b981;
  color: white;
}

.base-btn--primary:hover:not(:disabled) {
  background: #059669;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
}

.base-btn--secondary {
  background: #f3f4f6;
  color: #374151;
}

.base-btn--secondary:hover:not(:disabled) {
  background: #e5e7eb;
}

.base-btn--danger {
  background: #fee2e2;
  color: #dc2626;
}

.base-btn--danger:hover:not(:disabled) {
  background: #dc2626;
  color: white;
}

.base-btn--ghost {
  background: transparent;
  color: #6b7280;
}

.base-btn--ghost:hover:not(:disabled) {
  background: #f3f4f6;
  color: #374151;
}

.base-btn--edit {
  background: #dbeafe;
  color: #2563eb;
}

.base-btn--edit:hover:not(:disabled) {
  background: #2563eb;
  color: white;
}

.base-btn--delete {
  background: #fee2e2;
  color: #dc2626;
}

.base-btn--delete:hover:not(:disabled) {
  background: #dc2626;
  color: white;
}

.spinner {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}
</style>
