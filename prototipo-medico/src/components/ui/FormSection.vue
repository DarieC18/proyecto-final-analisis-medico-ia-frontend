<template>
  <section class="form-section">
    <header v-if="title" class="form-section__header">
      <IconTile v-if="icon" :icon="icon" :tone="tone" size="sm" />
      <div>
        <h3 class="form-section__title">{{ title }}</h3>
        <p v-if="description" class="form-section__description">{{ description }}</p>
      </div>
      <div v-if="$slots.actions" class="form-section__actions">
        <slot name="actions" />
      </div>
    </header>

    <div class="form-section__body">
      <slot />
    </div>
  </section>
</template>

<script setup>
import IconTile from './IconTile.vue'

/**
 * Grupo de campos de un formulario.
 *
 * Este componente es el que borra la mayor deuda de CSS del proyecto: el
 * bloque `.form-section / .section-header / .section-icon / .section-body` más
 * los overrides de `.form-control` estaba duplicado VERBATIM en 10 archivos
 * (PatientListView, UsersView, RegisterView, AppointmentsView,
 * AppointmentDetailView, MedicalDocumentsView, LoginView, ResetPasswordView,
 * ForgotPasswordView y AuditLogView, donde se llamaba `.form-filter`).
 * Son ~500 líneas de CSS que desaparecen.
 */
defineProps({
  title: { type: String, default: '' },
  description: { type: String, default: '' },
  icon: { type: [Object, Function], default: null },
  tone: { type: String, default: 'brand' }
})
</script>

<style scoped>
.form-section {
  background-color: var(--app-surface-sunken);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius-lg);
  overflow: hidden;
}

.form-section__header {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  padding: 0.75rem 1rem;
  background-color: var(--app-surface);
  border-bottom: 1px solid var(--app-border);
}

.form-section__title {
  margin: 0;
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--app-text-strong);
}

.form-section__description {
  margin: 0;
  font-size: 0.775rem;
  color: var(--app-text-muted);
}

.form-section__actions {
  margin-inline-start: auto;
}

.form-section__body {
  padding: 1.125rem 1rem;
}
</style>
