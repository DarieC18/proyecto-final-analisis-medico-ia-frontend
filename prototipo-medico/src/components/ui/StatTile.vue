<template>
  <component
    :is="to ? RouterLink : 'div'"
    :to="to"
    class="stat-tile"
    :class="[`stat-tile--${variant}`, `stat-tile--${tone}`, { 'stat-tile--link': to }]"
  >
    <div class="stat-tile__top">
      <p class="stat-tile__label">{{ label }}</p>
      <IconTile v-if="icon && variant !== 'solid'" :icon="icon" :tone="tone" size="sm" />
      <Icon v-else-if="icon" :icon="icon" :size="18" class="stat-tile__solid-icon" />
    </div>

    <p v-if="loading" class="stat-tile__value stat-tile__value--loading">—</p>
    <p v-else class="stat-tile__value">{{ value ?? '—' }}</p>

    <p v-if="hint" class="stat-tile__hint">{{ hint }}</p>
  </component>
</template>

<script setup>
import { RouterLink } from 'vue-router'
import Icon from './Icon.vue'
import IconTile from './IconTile.vue'

/**
 * Tarjeta de métrica.
 *
 * Unifica los dos lenguajes que convivían: el StatCard con
 * `border-start border-4` que sólo usaba DoctorDashboardView, y las cinco
 * tarjetas de color sólido (`card bg-info text-white p-3`) que
 * AdminDashboardView se había hecho a mano. Un componente, dos variantes.
 */
defineProps({
  label: { type: String, required: true },
  value: { type: [String, Number], default: null },
  hint: { type: String, default: '' },
  icon: { type: [Object, Function], default: null },
  tone: { type: String, default: 'brand' },
  variant: { type: String, default: 'soft' },
  to: { type: [String, Object], default: null },
  loading: Boolean
})
</script>

<style scoped>
.stat-tile {
  display: block;
  padding: 1rem 1.125rem;
  border-radius: var(--app-radius-lg);
  border: 1px solid var(--app-border);
  background-color: var(--app-surface);
  box-shadow: var(--app-shadow-sm);
  color: inherit;
  transition: box-shadow var(--app-transition), transform var(--app-transition),
    border-color var(--app-transition);
  height: 100%;
}

.stat-tile--link:hover {
  box-shadow: var(--app-shadow);
  transform: translateY(-2px);
  border-color: var(--app-border-strong);
}

.stat-tile__top {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 0.75rem;
  margin-bottom: 0.5rem;
}

.stat-tile__label {
  margin: 0;
  font-size: 0.72rem;
  font-weight: 600;
  letter-spacing: 0.05em;
  text-transform: uppercase;
  color: var(--app-text-muted);
}

.stat-tile__value {
  margin: 0;
  font-family: var(--app-font-display);
  font-size: 1.75rem;
  font-weight: 700;
  line-height: 1.1;
  color: var(--app-text-strong);
  font-variant-numeric: tabular-nums;
}

.stat-tile__value--loading {
  color: var(--app-text-subtle);
}

.stat-tile__hint {
  margin: 0.25rem 0 0;
  font-size: 0.775rem;
  color: var(--app-text-muted);
}

/* --- Variante `outline`: acento de color sólo en el borde izquierdo -------- */
.stat-tile--outline {
  border-inline-start-width: 3px;
}
.stat-tile--outline.stat-tile--brand {
  border-inline-start-color: var(--c-brand-500);
}
.stat-tile--outline.stat-tile--success {
  border-inline-start-color: var(--c-success-500);
}
.stat-tile--outline.stat-tile--warning {
  border-inline-start-color: var(--c-warning-500);
}
.stat-tile--outline.stat-tile--danger {
  border-inline-start-color: var(--c-danger-500);
}
.stat-tile--outline.stat-tile--info {
  border-inline-start-color: var(--c-info-500);
}

/* --- Variante `solid`: fondo de color, texto invertido -------------------- */
.stat-tile--solid {
  border-color: transparent;
  color: #fff;
}
.stat-tile--solid .stat-tile__label,
.stat-tile--solid .stat-tile__hint,
.stat-tile--solid .stat-tile__value,
.stat-tile--solid .stat-tile__solid-icon {
  color: #fff;
}
.stat-tile--solid .stat-tile__label,
.stat-tile--solid .stat-tile__hint {
  opacity: 0.82;
}

.stat-tile--solid.stat-tile--brand {
  background: linear-gradient(135deg, var(--c-brand-600), var(--c-brand-800));
}
.stat-tile--solid.stat-tile--success {
  background: linear-gradient(135deg, var(--c-success-700), var(--c-success-emphasis));
}
.stat-tile--solid.stat-tile--danger {
  background: linear-gradient(135deg, var(--c-danger-600), var(--c-danger-emphasis));
}
.stat-tile--solid.stat-tile--info {
  background: linear-gradient(135deg, var(--c-info-600), var(--c-info-emphasis));
}
/* warning en sólido lleva texto oscuro: el amarillo con blanco no contrasta. */
.stat-tile--solid.stat-tile--warning {
  background: linear-gradient(135deg, var(--c-warning-500), var(--c-warning-600));
}
.stat-tile--solid.stat-tile--warning .stat-tile__label,
.stat-tile--solid.stat-tile--warning .stat-tile__hint,
.stat-tile--solid.stat-tile--warning .stat-tile__value,
.stat-tile--solid.stat-tile--warning .stat-tile__solid-icon {
  color: var(--c-slate-900);
}
</style>
