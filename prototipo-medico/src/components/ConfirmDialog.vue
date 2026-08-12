<template>
  <Teleport to="body">
    <div v-if="visible" class="confirm-overlay" @click.self="$emit('cancel')">
      <div class="confirm-card animation-fade">
        <div class="mb-4">
          <div class="display-1" :class="iconClass">{{ icon }}</div>
        </div>
        <h4 class="fw-bold mb-3">{{ title }}</h4>
        <p class="text-muted mb-4">{{ message }}</p>
        <div class="d-flex justify-content-center gap-3">
          <button @click="$emit('cancel')" class="btn btn-light border px-4 py-2 fw-medium">Cancelar</button>
          <button @click="$emit('confirm')" class="btn px-4 py-2 fw-medium shadow-sm" :class="confirmClass">
            Sí, {{ confirmText }}
          </button>
        </div>
      </div>
    </div>
  </Teleport>
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
.confirm-overlay {
  position: fixed;
  inset: 0;
  z-index: 9999;
  background: rgba(0, 0, 0, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
}

.confirm-card {
  background: #fff;
  border-radius: 1rem;
  padding: 2.5rem;
  max-width: 480px;
  width: 100%;
  text-align: center;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.2);
}

.animation-fade {
  animation: fadeIn 0.2s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: scale(0.95); }
  to   { opacity: 1; transform: scale(1); }
}
</style>
