<template>
  <div class="empty-state" :class="`empty-state--${size}`">
    <IconTile v-if="icon" :icon="icon" :tone="tone" :size="size === 'sm' ? 'lg' : 'xl'" />
    <p class="empty-state__title">{{ title }}</p>
    <p v-if="message" class="empty-state__message">{{ message }}</p>
    <div v-if="$slots.action || actionLabel" class="empty-state__action">
      <slot name="action">
        <AppButton variant="soft-primary" size="sm" :icon="actionIcon" @click="$emit('action')">
          {{ actionLabel }}
        </AppButton>
      </slot>
    </div>
  </div>
</template>

<script setup>
import AppButton from './AppButton.vue'
import IconTile from './IconTile.vue'

/**
 * Estado vacío. Sustituye al patrón `text-center py-5 text-muted` con un emoji
 * en `display-4` haciendo de gráfico.
 *
 * Deliberadamente NO se pone un icono lucide a 4rem: el arte lineal a ese
 * tamaño se ve hueco. Va dentro de un IconTile, que le da superficie y peso.
 */
defineProps({
  icon: { type: [Object, Function], default: null },
  title: { type: String, required: true },
  message: { type: String, default: '' },
  tone: { type: String, default: 'neutral' },
  size: { type: String, default: 'md' },
  actionLabel: { type: String, default: '' },
  actionIcon: { type: [Object, Function], default: null }
})

defineEmits(['action'])
</script>

<style scoped>
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.375rem;
  padding: 3rem 1.5rem;
  text-align: center;
}

.empty-state--sm {
  padding: 1.75rem 1rem;
}

.empty-state__title {
  margin: 0.625rem 0 0;
  font-weight: 600;
  color: var(--app-text-strong);
}

.empty-state__message {
  margin: 0;
  max-width: 46ch;
  font-size: 0.875rem;
  color: var(--app-text-muted);
}

.empty-state__action {
  margin-top: 0.75rem;
}
</style>
