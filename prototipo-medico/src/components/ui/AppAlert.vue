<template>
  <div v-if="visible" class="app-alert" :class="`app-alert--${variant}`" :role="role">
    <Icon :icon="resolvedIcon" :size="18" class="app-alert__icon" />
    <div class="app-alert__body">
      <p v-if="title" class="app-alert__title">{{ title }}</p>
      <div class="app-alert__text"><slot>{{ message }}</slot></div>
    </div>
    <button
      v-if="dismissible"
      type="button"
      class="app-alert__close"
      aria-label="Cerrar aviso"
      @click="dismiss"
    >
      <Icon :icon="IconClose" :size="15" />
    </button>
  </div>
</template>

<script setup>
import { computed, ref, watch } from 'vue'
import Icon from './Icon.vue'
import { IconAttention, IconClose, IconError, IconInfo, IconSuccess } from '@/lib/icons'

/**
 * Aviso. Sustituye a los 36 `alert alert-danger border-0 rounded-3` sueltos.
 * A diferencia del .alert de Bootstrap, trae icono y jerarquía título/cuerpo.
 */
const props = defineProps({
  variant: { type: String, default: 'danger' },
  title: { type: String, default: '' },
  message: { type: String, default: '' },
  icon: { type: [Object, Function], default: null },
  dismissible: Boolean,
  modelValue: { type: Boolean, default: true }
})

const emit = defineEmits(['update:modelValue', 'close'])

const internal = ref(props.modelValue)
watch(() => props.modelValue, (v) => (internal.value = v))

const visible = computed(() => internal.value)

const DEFAULT_ICONS = {
  danger: IconError,
  warning: IconAttention,
  success: IconSuccess,
  info: IconInfo,
  brand: IconInfo
}
const resolvedIcon = computed(() => props.icon ?? DEFAULT_ICONS[props.variant] ?? IconInfo)

// Un error debe interrumpir al lector de pantalla; un aviso informativo no.
const role = computed(() => (props.variant === 'danger' ? 'alert' : 'status'))

function dismiss() {
  internal.value = false
  emit('update:modelValue', false)
  emit('close')
}
</script>

<style scoped>
.app-alert {
  display: flex;
  align-items: flex-start;
  gap: 0.625rem;
  padding: 0.75rem 0.875rem;
  border: 1px solid;
  border-radius: var(--app-radius);
  font-size: 0.875rem;
}

.app-alert__icon {
  margin-top: 0.1rem;
}

.app-alert__body {
  min-width: 0;
  flex: 1;
}

.app-alert__title {
  margin: 0 0 0.125rem;
  font-weight: 600;
}

.app-alert__text {
  margin: 0;
}

.app-alert__text :deep(p:last-child) {
  margin-bottom: 0;
}

.app-alert__close {
  flex: none;
  border: 0;
  background: transparent;
  color: inherit;
  opacity: 0.6;
  padding: 0.125rem;
  border-radius: var(--app-radius-sm);
  cursor: pointer;
}
.app-alert__close:hover {
  opacity: 1;
}

.app-alert--danger {
  background-color: var(--bs-danger-bg-subtle);
  border-color: var(--bs-danger-border-subtle);
  color: var(--bs-danger-text-emphasis);
}
.app-alert--warning {
  background-color: var(--bs-warning-bg-subtle);
  border-color: var(--bs-warning-border-subtle);
  color: var(--bs-warning-text-emphasis);
}
.app-alert--success {
  background-color: var(--bs-success-bg-subtle);
  border-color: var(--bs-success-border-subtle);
  color: var(--bs-success-text-emphasis);
}
.app-alert--info {
  background-color: var(--bs-info-bg-subtle);
  border-color: var(--bs-info-border-subtle);
  color: var(--bs-info-text-emphasis);
}
.app-alert--brand {
  background-color: var(--bs-primary-bg-subtle);
  border-color: var(--bs-primary-border-subtle);
  color: var(--bs-primary-text-emphasis);
}
</style>
