<template>
  <Teleport to="body">
    <div v-if="visible" class="delete-modal-overlay" @click.self="$emit('cancel')">
      <div class="delete-modal-container">
        <div class="delete-modal-header">
          <h6 class="fw-bold mb-0 text-white">Eliminar Documento</h6>
          <button @click="$emit('cancel')" class="btn btn-sm text-white border-0 px-2 fs-4 close-btn">&times;</button>
        </div>
        <div class="delete-modal-body text-center">
          <div class="display-1 text-danger mb-3">🗑️</div>
          <h5 class="fw-bold mb-2">{{ title }}</h5>
          <p class="text-muted mb-0">{{ message }}</p>
        </div>
        <div class="delete-modal-footer">
          <button @click="$emit('cancel')" class="btn btn-light border px-4 py-2 fw-medium">Cancelar</button>
          <button @click="$emit('confirm')" class="btn btn-danger px-4 py-2 fw-medium shadow-sm">Sí, eliminar</button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
defineProps({
  visible: Boolean,
  title: { type: String, default: 'Confirmar eliminación' },
  message: { type: String, default: '¿Está seguro que desea eliminar este documento?' }
})

defineEmits(['confirm', 'cancel'])
</script>

<style scoped>
.delete-modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.6);
  z-index: 2000;
  display: flex;
  align-items: center;
  justify-content: center;
  animation: fadeIn 0.2s ease;
}

.delete-modal-container {
  width: 90vw;
  max-width: 420px;
  background: white;
  border-radius: 0.75rem;
  overflow: hidden;
  box-shadow: 0 25px 60px rgba(0, 0, 0, 0.4);
  animation: slideUp 0.25s ease;
}

.delete-modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.875rem 1.25rem;
  background: #dc3545;
}

.delete-modal-body {
  padding: 2rem 1.5rem;
}

.delete-modal-footer {
  display: flex;
  justify-content: center;
  gap: 0.75rem;
  padding: 1rem 1.5rem 1.5rem;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.2);
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
