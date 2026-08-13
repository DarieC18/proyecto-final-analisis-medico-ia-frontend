<template>
  <div v-if="variant === 'skeleton'" class="skeleton-stack" role="status" :aria-label="label">
    <span v-for="n in rows" :key="n" class="skeleton" :style="{ width: widthFor(n) }" />
    <span class="visually-hidden">{{ label }}</span>
  </div>

  <div v-else-if="inline" class="d-inline-flex align-items-center gap-2 text-app-muted" role="status">
    <span class="spinner-border spinner-border-sm" aria-hidden="true" />
    <span class="small">{{ label }}</span>
  </div>

  <div v-else class="loading-block" role="status">
    <span class="spinner-border text-primary" aria-hidden="true" />
    <p class="loading-block__label">{{ label }}</p>
  </div>
</template>

<script setup>
/**
 * Estado de carga. Sustituye a los ~40 puntos donde se repetía
 * `<div class="text-center py-5"><div class="spinner-border text-primary"></div></div>`.
 */
const props = defineProps({
  label: { type: String, default: 'Cargando…' },
  variant: { type: String, default: 'spinner' },
  rows: { type: Number, default: 3 },
  inline: Boolean
})

// Anchos desiguales: un esqueleto con todas las filas iguales se lee como una
// tabla vacía en vez de como contenido cargando.
const WIDTHS = ['100%', '92%', '78%', '96%', '84%']
const widthFor = (n) => WIDTHS[(n - 1) % WIDTHS.length]
void props
</script>

<style scoped>
.loading-block {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  padding: 3rem 1rem;
}

.loading-block__label {
  margin: 0;
  font-size: 0.85rem;
  color: var(--app-text-muted);
}

.skeleton-stack {
  display: flex;
  flex-direction: column;
  gap: 0.625rem;
  padding: 0.5rem 0;
}

.skeleton {
  display: block;
  height: 1rem;
  border-radius: var(--app-radius-sm);
  background: linear-gradient(
    90deg,
    var(--bs-tertiary-bg) 25%,
    var(--bs-secondary-bg) 37%,
    var(--bs-tertiary-bg) 63%
  );
  background-size: 400% 100%;
  animation: skeleton-shimmer 1.4s ease infinite;
}

@keyframes skeleton-shimmer {
  0% {
    background-position: 100% 50%;
  }
  100% {
    background-position: 0 50%;
  }
}
</style>
