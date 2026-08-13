<template>
  <div>
    <PageHeader
      title="Mis Documentos"
      subtitle="Documentos médicos compartidos por tu médico"
      :icon="IconDocument"
    />

    <LoadingState v-if="loading" label="Cargando documentos…" />

    <AppAlert v-else-if="error" variant="danger" :message="error" />

    <BaseCard v-else-if="docs.length === 0" flush padding="none">
      <EmptyState
        :icon="IconDocument"
        title="Sin documentos disponibles"
        message="Cuando tu médico comparta un documento contigo, aparecerá aquí."
      />
    </BaseCard>

    <div v-else class="row g-3">
      <div v-for="d in docs" :key="d.id" class="col-md-6 col-lg-4">
        <BaseCard hoverable class="h-100">
          <div class="d-flex align-items-center gap-3 mb-3">
            <IconTile :icon="fileIconFor(d.fileName)" :tone="fileToneFor(d.fileName)" size="md" />
            <div class="u-min-w-0">
              <p class="doc__name">{{ d.fileName }}</p>
              <small class="text-app-muted">{{ d.fileType || 'Documento' }}</small>
            </div>
          </div>
          <div class="d-flex justify-content-between align-items-center">
            <small class="text-app-muted">{{ formatDate(d.uploadedAt) }}</small>
            <small class="text-app-muted">{{ d.uploadedByUserName || 'Médico' }}</small>
          </div>
        </BaseCard>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { portalService } from '@/api/portal'
import { AppAlert, BaseCard, EmptyState, IconTile, LoadingState, PageHeader } from '@/components/ui'
import { IconDocument } from '@/lib/icons'
import { fileIconFor, fileToneFor } from '@/lib/fileIcons'

const loading = ref(true)
const error = ref('')
const docs = ref([])

const formatDate = (dateStr) => {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' })
}

onMounted(async () => {
  try {
    const res = await portalService.getDocuments()
    docs.value = res.data || []
  } catch (err) {
    error.value =
      err.response?.status === 404
        ? 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
        : 'Error al cargar documentos'
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.doc__name {
  margin: 0;
  font-weight: 600;
  font-size: 0.9rem;
  color: var(--app-text-strong);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
