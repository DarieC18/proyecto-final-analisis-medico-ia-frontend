<template>
  <span class="icon-tile" :class="[`icon-tile--${tone}`, `icon-tile--${size}`]">
    <Icon :icon="icon" :size="iconSize" :stroke-width="1.75" />
  </span>
</template>

<script setup>
import { computed } from 'vue'
import Icon from './Icon.vue'

/**
 * Cuadrado redondeado de color con un icono dentro.
 *
 * Es el sustituto de los emojis grandes (`display-1` / `display-4`) que hacían
 * de gráfico en estados vacíos. Un icono lucide suelto a 4rem se ve hueco y
 * frágil; encapsulado en una superficie de color tiene peso visual.
 */
const props = defineProps({
  icon: { type: [Object, Function], required: true },
  tone: { type: String, default: 'brand' },
  size: { type: String, default: 'md' }
})

const ICON_SIZES = { sm: 16, md: 20, lg: 28, xl: 34 }
const iconSize = computed(() => ICON_SIZES[props.size] ?? 20)
</script>

<style scoped>
.icon-tile {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex: none;
  border-radius: var(--app-radius);
  border: 1px solid transparent;
}

.icon-tile--sm {
  width: 28px;
  height: 28px;
  border-radius: var(--app-radius-sm);
}
.icon-tile--md {
  width: 38px;
  height: 38px;
}
.icon-tile--lg {
  width: 56px;
  height: 56px;
  border-radius: var(--app-radius-lg);
}
.icon-tile--xl {
  width: 68px;
  height: 68px;
  border-radius: var(--app-radius-xl);
}

.icon-tile--brand {
  background-color: var(--bs-primary-bg-subtle);
  border-color: var(--bs-primary-border-subtle);
  color: var(--bs-primary-text-emphasis);
}
.icon-tile--success {
  background-color: var(--bs-success-bg-subtle);
  border-color: var(--bs-success-border-subtle);
  color: var(--bs-success-text-emphasis);
}
.icon-tile--warning {
  background-color: var(--bs-warning-bg-subtle);
  border-color: var(--bs-warning-border-subtle);
  color: var(--bs-warning-text-emphasis);
}
.icon-tile--danger {
  background-color: var(--bs-danger-bg-subtle);
  border-color: var(--bs-danger-border-subtle);
  color: var(--bs-danger-text-emphasis);
}
.icon-tile--info {
  background-color: var(--bs-info-bg-subtle);
  border-color: var(--bs-info-border-subtle);
  color: var(--bs-info-text-emphasis);
}
.icon-tile--neutral {
  background-color: var(--bs-tertiary-bg);
  border-color: var(--app-border);
  color: var(--app-text-muted);
}
</style>
