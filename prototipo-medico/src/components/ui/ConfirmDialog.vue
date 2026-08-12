<template>
  <BaseModal
    :model-value="visible"
    size="sm"
    :scrollable="false"
    :close-on-backdrop="!loading"
    @update:model-value="$emit('cancel')"
  >
    <div class="confirm">
      <IconTile :icon="resolvedIcon" :tone="tone" size="lg" />
      <h2 class="confirm__title">{{ title }}</h2>
      <p class="confirm__message">{{ message }}</p>
    </div>

    <template #footer>
      <AppButton variant="soft" :disabled="loading" @click="$emit('cancel')">
        {{ cancelText }}
      </AppButton>
      <AppButton
        :variant="tone === 'danger' ? 'danger' : 'primary'"
        :loading="loading"
        @click="$emit('confirm')"
      >
        {{ confirmText }}
      </AppButton>
    </template>
  </BaseModal>
</template>

<script setup>
import { computed } from 'vue'
import AppButton from './AppButton.vue'
import BaseModal from './BaseModal.vue'
import IconTile from './IconTile.vue'
import { IconAttention, IconDelete } from '@/lib/icons'

/**
 * Diálogo de confirmación.
 *
 * CAMBIO DE CONTRATO respecto al anterior: `icon` pasa de String (el emoji
 * '🗑️') a componente de lucide. Además el anterior no era un modal — era un
 * div suelto en el flujo de la página, sin backdrop, sin Teleport y sin Escape.
 * Ahora se apoya en BaseModal.
 */
const props = defineProps({
  visible: Boolean,
  title: { type: String, default: 'Confirmar acción' },
  message: { type: String, default: '¿Está seguro que desea continuar?' },
  confirmText: { type: String, default: 'Confirmar' },
  cancelText: { type: String, default: 'Cancelar' },
  danger: Boolean,
  tone: { type: String, default: null },
  loading: Boolean,
  icon: { type: [Object, Function], default: null }
})

defineEmits(['confirm', 'cancel'])

const tone = computed(() => props.tone ?? (props.danger ? 'danger' : 'warning'))
const resolvedIcon = computed(
  () => props.icon ?? (tone.value === 'danger' ? IconDelete : IconAttention)
)
</script>

<style scoped>
.confirm {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  text-align: center;
  padding-block: 0.5rem;
}

.confirm__title {
  margin: 0.5rem 0 0;
  font-size: 1.0625rem;
  font-weight: 600;
}

.confirm__message {
  margin: 0;
  font-size: 0.875rem;
  color: var(--app-text-muted);
  max-width: 40ch;
}
</style>
