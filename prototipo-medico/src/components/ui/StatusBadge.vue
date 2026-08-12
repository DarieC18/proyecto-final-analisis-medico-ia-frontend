<template>
  <span class="status-badge" :class="[`status-badge--${tone}`, `status-badge--${size}`]">
    <span v-if="dot" class="status-badge__dot" />
    {{ text }}
  </span>
</template>

<script setup>
import { computed } from 'vue'

/**
 * Distintivo de estado.
 *
 * Se conserva la API pública anterior (`text`, `variant`) para poder migrar las
 * 10 vistas que ya lo usan sin romperlas, pero cambia la implementación: antes
 * era `bg-X bg-opacity-10 text-X`, que en claro dejaba combinaciones de ~2:1
 * (`text-warning` sobre `bg-warning` al 10 %) y en oscuro no reaccionaba al
 * tema. Ahora usa los tríos subtle/emphasis de Bootstrap 5.3, que sí son
 * conscientes del tema y sí contrastan.
 */
const props = defineProps({
  text: { type: String, default: '' },
  variant: { type: String, default: 'secondary' },
  size: { type: String, default: 'md' },
  dot: Boolean
})

const TONES = {
  pending: 'warning',
  inprogress: 'brand',
  completed: 'success',
  cancelled: 'danger',
  active: 'success',
  inactive: 'neutral',
  info: 'info',
  low: 'success',
  moderate: 'warning',
  severe: 'danger',
  critical: 'danger',
  // nombres de color directos, por compatibilidad con las llamadas actuales
  primary: 'brand',
  secondary: 'neutral',
  success: 'success',
  warning: 'warning',
  danger: 'danger'
}

const tone = computed(() => TONES[props.variant] ?? 'neutral')
</script>

<style scoped>
.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  border-radius: var(--app-radius-pill);
  border: 1px solid;
  font-weight: 600;
  line-height: 1.2;
  white-space: nowrap;
}

.status-badge--sm {
  padding: 0.15rem 0.5rem;
  font-size: 0.7rem;
}
.status-badge--md {
  padding: 0.25rem 0.65rem;
  font-size: 0.75rem;
}
.status-badge--lg {
  padding: 0.35rem 0.85rem;
  font-size: 0.825rem;
}

.status-badge__dot {
  width: 0.4rem;
  height: 0.4rem;
  border-radius: 50%;
  background-color: currentColor;
  flex: none;
}

.status-badge--brand {
  background-color: var(--bs-primary-bg-subtle);
  border-color: var(--bs-primary-border-subtle);
  color: var(--bs-primary-text-emphasis);
}
.status-badge--success {
  background-color: var(--bs-success-bg-subtle);
  border-color: var(--bs-success-border-subtle);
  color: var(--bs-success-text-emphasis);
}
.status-badge--warning {
  background-color: var(--bs-warning-bg-subtle);
  border-color: var(--bs-warning-border-subtle);
  color: var(--bs-warning-text-emphasis);
}
.status-badge--danger {
  background-color: var(--bs-danger-bg-subtle);
  border-color: var(--bs-danger-border-subtle);
  color: var(--bs-danger-text-emphasis);
}
.status-badge--info {
  background-color: var(--bs-info-bg-subtle);
  border-color: var(--bs-info-border-subtle);
  color: var(--bs-info-text-emphasis);
}
.status-badge--neutral {
  background-color: var(--bs-tertiary-bg);
  border-color: var(--app-border);
  color: var(--app-text-muted);
}
</style>
