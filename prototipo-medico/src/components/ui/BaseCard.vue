<template>
  <div class="card base-card" :class="[`base-card--${tone}`, { 'base-card--hoverable': hoverable }]">
    <div v-if="$slots.header || title" class="base-card__header">
      <slot name="header">
        <div class="base-card__identity">
          <IconTile v-if="icon" :icon="icon" :tone="iconTone" size="sm" />
          <div>
            <h2 class="base-card__title">{{ title }}</h2>
            <p v-if="subtitle" class="base-card__subtitle">{{ subtitle }}</p>
          </div>
        </div>
      </slot>
      <div v-if="$slots['header-actions']" class="base-card__header-actions">
        <slot name="header-actions" />
      </div>
    </div>

    <div :class="flush ? '' : `card-body p-${paddingScale}`">
      <slot />
    </div>

    <div v-if="$slots.footer" class="base-card__footer">
      <slot name="footer" />
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import IconTile from './IconTile.vue'

/**
 * Tarjeta base. Sustituye a las combinaciones que se retecleaban en cada vista
 * (`card shadow-sm border-0 rounded-4`, `card bg-light border-0 shadow-sm p-3
 * text-center rounded-4`, …) y hace innecesario el
 * `.card { border-radius: 1rem !important; border: none !important }` global
 * que peleaba con el CSS propio de LandingView.
 *
 * Ojo con `border-0`: en tema oscuro el borde ES necesario para separar la
 * superficie de la tarjeta del canvas, ya que la sombra deja de verse.
 */
const props = defineProps({
  title: { type: String, default: '' },
  subtitle: { type: String, default: '' },
  icon: { type: [Object, Function], default: null },
  iconTone: { type: String, default: 'brand' },
  tone: { type: String, default: 'default' },
  padding: { type: String, default: 'md' },
  flush: Boolean,
  hoverable: Boolean
})

const PADDING = { none: '0', sm: '3', md: '4', lg: '5' }
const paddingScale = computed(() => PADDING[props.padding] ?? '4')
</script>

<style scoped>
.base-card {
  border: 1px solid var(--app-border);
  box-shadow: var(--app-shadow-sm);
  transition: box-shadow var(--app-transition), transform var(--app-transition),
    border-color var(--app-transition);
}

.base-card--muted {
  background-color: var(--app-surface-sunken);
}
.base-card--brand {
  background-color: var(--bs-primary-bg-subtle);
  border-color: var(--bs-primary-border-subtle);
}
.base-card--danger {
  background-color: var(--bs-danger-bg-subtle);
  border-color: var(--bs-danger-border-subtle);
}
.base-card--warning {
  background-color: var(--bs-warning-bg-subtle);
  border-color: var(--bs-warning-border-subtle);
}

.base-card--hoverable:hover {
  box-shadow: var(--app-shadow);
  transform: translateY(-2px);
  border-color: var(--app-border-strong);
}

.base-card__header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.875rem 1.25rem;
  border-bottom: 1px solid var(--app-border);
}

.base-card__identity {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  min-width: 0;
}

.base-card__title {
  margin: 0;
  font-size: 0.9375rem;
  font-weight: 600;
  line-height: 1.3;
}

.base-card__subtitle {
  margin: 0;
  font-size: 0.8rem;
  color: var(--app-text-muted);
}

.base-card__header-actions {
  margin-inline-start: auto;
  display: flex;
  align-items: center;
  gap: 0.375rem;
}

.base-card__footer {
  padding: 0.875rem 1.25rem;
  border-top: 1px solid var(--app-border);
  background-color: var(--app-surface-sunken);
}
</style>
