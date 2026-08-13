<template>
  <nav class="tab-nav" role="tablist">
    <button
      v-for="tab in tabs"
      :key="tab.id"
      type="button"
      role="tab"
      class="tab-nav__tab"
      :class="{ 'tab-nav__tab--active': modelValue === tab.id }"
      :aria-selected="modelValue === tab.id"
      @click="$emit('update:modelValue', tab.id)"
    >
      <Icon v-if="tab.icon" :icon="tab.icon" :size="16" />
      <span>{{ tab.label }}</span>
      <span v-if="tab.count != null" class="tab-nav__count">{{ tab.count }}</span>
    </button>
  </nav>
</template>

<script setup>
import Icon from './Icon.vue'

/**
 * Barra de pestañas.
 *
 * Absorbe el `ul.nav.nav-pills.nav-fill` que AppointmentDetailView y
 * NurseFollowUpView se habían escrito por separado: ambos repetían el mismo
 * `<a href="#">` con un emoji delante y un objeto de clases distinto por
 * pestaña. Dos problemas de fondo que aquí desaparecen:
 *
 *   - `<a href="#">` no es un control: no se anuncia como pestaña y al pulsarlo
 *     salta al principio de la página. Aquí es un <button role="tab">.
 *   - El estado activo iba con `bg-primary` / `bg-dark` en hex de Bootstrap, que
 *     no reacciona a data-bs-theme. Ahora sale de los tokens del tema.
 *
 * `tabs`: [{ id, label, icon?, count? }]. El icono es el COMPONENTE lucide.
 */
defineProps({
  tabs: { type: Array, required: true },
  modelValue: { type: String, required: true }
})

defineEmits(['update:modelValue'])
</script>

<style scoped>
.tab-nav {
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem;
  padding: 0.25rem;
}

.tab-nav__tab {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  flex: 1 1 auto;
  padding: 0.5rem 0.875rem;
  border: 1px solid transparent;
  border-radius: var(--app-radius-pill);
  background: transparent;
  color: var(--app-text-muted);
  font-size: 0.875rem;
  font-weight: 500;
  white-space: nowrap;
  cursor: pointer;
  transition: background-color var(--app-transition), color var(--app-transition),
    border-color var(--app-transition);
}

.tab-nav__tab:hover {
  background-color: var(--app-surface-sunken);
  color: var(--app-text);
}

.tab-nav__tab--active,
.tab-nav__tab--active:hover {
  background-color: var(--bs-primary-bg-subtle);
  border-color: var(--bs-primary-border-subtle);
  color: var(--bs-primary-text-emphasis);
  font-weight: 600;
}

.tab-nav__tab:focus-visible {
  outline: none;
  box-shadow: var(--app-ring);
}

.tab-nav__count {
  padding: 0 0.375rem;
  border-radius: var(--app-radius-pill);
  background-color: var(--bs-tertiary-bg);
  font-size: 0.7rem;
  font-variant-numeric: tabular-nums;
}
</style>
