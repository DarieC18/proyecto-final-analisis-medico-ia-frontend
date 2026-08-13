<template>
  <BaseModal
    :model-value="modelValue"
    size="xl"
    :title="doc?.fileName"
    @update:model-value="$emit('update:modelValue', $event)"
  >
    <p v-if="doc?.description" class="preview__description">{{ doc.description }}</p>

    <div class="preview__stage">
      <img v-if="isImage" :src="fileUrl" class="preview__image" :alt="doc?.fileName" />
      <EmptyState
        v-else
        size="sm"
        tone="neutral"
        :icon="IconDocument"
        title="Vista previa no disponible"
        message="Este formato no puede mostrarse aquí. Ábrelo en una pestaña nueva para consultarlo."
      />
    </div>

    <template #footer>
      <AppButton variant="soft" :href="fileUrl" target="_blank" :icon="IconExternal">
        Abrir en nueva pestaña
      </AppButton>
      <AppButton variant="primary" @click="$emit('update:modelValue', false)">Cerrar</AppButton>
    </template>
  </BaseModal>
</template>

<script setup>
import { computed } from 'vue'
import AppButton from './AppButton.vue'
import BaseModal from './BaseModal.vue'
import EmptyState from './EmptyState.vue'
import { IconDocument, IconExternal } from '@/lib/icons'
import { isImageFile } from '@/lib/fileIcons'

/**
 * Vista previa de documento. Este markup estaba duplicado tal cual en
 * AppointmentDetailView y MedicalDocumentsView, con backdrop manual y
 * `class="modal d-block"`.
 */
const props = defineProps({
  modelValue: Boolean,
  doc: { type: Object, default: null }
})

defineEmits(['update:modelValue'])

const fileUrl = computed(() =>
  props.doc?.id ? `/api/v1/MedicalDocument/${props.doc.id}/file` : ''
)
const isImage = computed(() => isImageFile(props.doc?.fileName))
</script>

<style scoped>
.preview__description {
  margin: 0 0 0.875rem;
  font-size: 0.85rem;
  color: var(--app-text-muted);
}

.preview__stage {
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: var(--bs-tertiary-bg);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius);
  padding: 0.75rem;
  min-height: 200px;
}

.preview__image {
  max-width: 100%;
  max-height: 68dvh;
  border-radius: var(--app-radius-sm);
}
</style>
