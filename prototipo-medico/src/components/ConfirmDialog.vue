<template>
  <div v-if="visible" class="d-flex justify-content-center mt-5 animation-fade">
    <div class="card shadow border-0 rounded-4 text-center p-5" style="max-width: 500px;">
      <div class="mb-4">
        <div class="display-1" :class="iconClass">{{ icon }}</div>
      </div>
      <h4 class="fw-bold mb-3">{{ title }}</h4>
      <p class="text-muted mb-4">{{ message }}</p>
      <div class="d-flex justify-content-center gap-3">
        <button @click="$emit('cancel')" class="btn btn-light border px-4 py-2 fw-medium">Cancelar</button>
        <button @click="$emit('confirm')" class="btn px-4 py-2 fw-medium shadow-sm" :class="confirmClass">Sí, {{ confirmText }}</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  visible: Boolean,
  title: { type: String, default: 'Confirmar acción' },
  message: { type: String, default: '¿Está seguro que desea continuar?' },
  confirmText: { type: String, default: 'Aceptar' },
  danger: Boolean,
  icon: { type: String, default: '🗑️' }
})

defineEmits(['confirm', 'cancel'])

const iconClass = computed(() => props.danger ? 'text-danger' : 'text-warning')
const confirmClass = computed(() => props.danger ? 'btn-danger' : 'btn-primary')
</script>

<style scoped>
.animation-fade {
  animation: fadeIn 0.3s ease-in-out;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
