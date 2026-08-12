<template>
  <div class="filter-bar" :class="{ 'filter-bar--sticky': sticky }">
    <div class="filter-bar__fields">
      <slot />
    </div>
    <div v-if="$slots.actions" class="filter-bar__actions">
      <slot name="actions" />
    </div>
  </div>
</template>

<script setup>
/** Barra de búsqueda y filtros. Sustituye al `card shadow-sm mb-4 border-0 >
    card-body p-3` que se repetía en unas 8 vistas. */
defineProps({ sticky: Boolean })
</script>

<style scoped>
.filter-bar {
  display: flex;
  flex-wrap: wrap;
  align-items: flex-end;
  gap: 0.75rem;
  padding: 0.875rem 1rem;
  margin-bottom: 1.25rem;
  background-color: var(--app-surface);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius-lg);
  box-shadow: var(--app-shadow-sm);
}

.filter-bar--sticky {
  position: sticky;
  top: calc(var(--app-topbar-h) + 0.5rem);
  z-index: 10;
}

.filter-bar__fields {
  display: flex;
  flex-wrap: wrap;
  align-items: flex-end;
  gap: 0.75rem;
  flex: 1;
  min-width: 0;
}

.filter-bar__actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
</style>
