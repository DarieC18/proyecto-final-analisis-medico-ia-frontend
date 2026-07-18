<template>
  <Teleport to="body">
    <Transition name="toast">
      <div v-if="visible" class="toast-notification" :class="typeClass">
        <div class="toast-icon">{{ icon }}</div>
        <div class="toast-message">{{ message }}</div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { ref, watch, computed } from 'vue'

const props = defineProps({
  message: { type: String, default: '' },
  type: { type: String, default: 'success' },
  duration: { type: Number, default: 3000 }
})

const visible = ref(false)
let timer = null

const typeClass = computed(() => `toast-${props.type}`)
const icon = computed(() => {
  if (props.type === 'success') return '✓'
  if (props.type === 'error') return '✕'
  if (props.type === 'warning') return '⚠'
  return 'ℹ'
})

watch(() => props.message, (val) => {
  if (val) {
    clearTimeout(timer)
    visible.value = true
    timer = setTimeout(() => { visible.value = false }, props.duration)
  }
})
</script>

<style scoped>
.toast-notification {
  position: fixed;
  bottom: 30px;
  right: 30px;
  z-index: 3000;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.875rem 1.5rem;
  border-radius: 0.5rem;
  color: white;
  font-weight: 500;
  font-size: 0.95rem;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.25);
  min-width: 280px;
}

.toast-success { background: #198754; }
.toast-error { background: #dc3545; }
.toast-warning { background: #ffc107; color: #333; }
.toast-info { background: #0d6efd; }

.toast-icon {
  font-size: 1.2rem;
  font-weight: 700;
  flex-shrink: 0;
}

.toast-enter-active {
  animation: slideIn 0.3s ease;
}
.toast-leave-active {
  animation: slideOut 0.3s ease;
}

@keyframes slideIn {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}
@keyframes slideOut {
  from { opacity: 1; transform: translateY(0); }
  to { opacity: 0; transform: translateY(20px); }
}
</style>
