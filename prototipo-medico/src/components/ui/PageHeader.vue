<template>
  <header class="page-header">
    <AppButton
      v-if="backTo"
      :to="backTo"
      variant="ghost"
      size="sm"
      :icon="IconBack"
      class="page-header__back"
    >
      Volver
    </AppButton>

    <div class="page-header__row">
      <div class="page-header__identity">
        <IconTile v-if="icon" :icon="icon" :tone="tone" size="lg" />
        <div class="page-header__text">
          <h1 class="page-header__title">{{ title }}</h1>
          <p v-if="subtitle" class="page-header__subtitle">{{ subtitle }}</p>
          <slot name="meta" />
        </div>
      </div>

      <div v-if="$slots.actions" class="page-header__actions">
        <slot name="actions" />
      </div>
    </div>
  </header>
</template>

<script setup>
import AppButton from './AppButton.vue'
import IconTile from './IconTile.vue'
import { IconBack } from '@/lib/icons'

/**
 * Cabecera de página. Sustituye al bloque
 * `d-flex justify-content-between align-items-center mb-4 > div > h3.fw-bold + p.text-muted`
 * que estaba repetido casi literalmente en unas 15 vistas.
 */
defineProps({
  title: { type: String, required: true },
  subtitle: { type: String, default: '' },
  icon: { type: [Object, Function], default: null },
  tone: { type: String, default: 'brand' },
  backTo: { type: [String, Object], default: null }
})
</script>

<style scoped>
.page-header {
  margin-bottom: 1.5rem;
}

.page-header__back {
  margin-bottom: 0.5rem;
  margin-inline-start: -0.5rem;
}

.page-header__row {
  display: flex;
  flex-wrap: wrap;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1rem;
}

.page-header__identity {
  display: flex;
  align-items: center;
  gap: 0.875rem;
  min-width: 0;
}

.page-header__text {
  min-width: 0;
}

.page-header__title {
  margin: 0;
  font-size: 1.375rem;
  font-weight: 600;
  line-height: 1.25;
}

.page-header__subtitle {
  margin: 0.125rem 0 0;
  font-size: 0.875rem;
  color: var(--app-text-muted);
}

.page-header__actions {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 0.5rem;
  margin-inline-start: auto;
}
</style>
