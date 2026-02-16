<template>
  <BaseModal :show="show" :title="title" :icon="PhWarning" size="sm" @close="cancel">
    <div class="confirm-content">
      <p>{{ message }}</p>
    </div>
    
    <template #footer>
      <BaseButton variant="secondary" @click="cancel">
        Cancelar
      </BaseButton>
      <BaseButton variant="danger" :loading="loading" @click="confirm">
        Confirmar
      </BaseButton>
    </template>
  </BaseModal>
</template>

<script setup>
import { PhWarning } from '@phosphor-icons/vue'
import BaseModal from './BaseModal.vue'
import BaseButton from './BaseButton.vue'

defineProps({
  show: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: 'Confirmar'
  },
  message: {
    type: String,
    default: '¿Está seguro de realizar esta acción?'
  },
  loading: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['confirm', 'cancel'])

const confirm = () => {
  emit('confirm')
}

const cancel = () => {
  emit('cancel')
}
</script>

<style scoped>
.confirm-content {
  text-align: center;
  padding: 1rem 0;
}

.confirm-content p {
  margin: 0;
  color: #374151;
  font-size: 0.9375rem;
}
</style>
