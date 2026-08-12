<template>
  <div>
    <PageHeader
      title="Documentos médicos"
      subtitle="Gestión de documentos clínicos por paciente"
      :icon="IconDocument"
    >
      <template #actions>
        <AppButton
          v-if="buscado && !showUpload"
          variant="primary"
          :icon="IconUpload"
          @click="showUpload = true"
        >
          Subir documento
        </AppButton>
      </template>
    </PageHeader>

    <FilterBar>
      <div class="flex-grow-1" style="min-width: 240px">
        <label for="d-patient" class="form-label">Paciente</label>
        <select id="d-patient" v-model="pacienteId" class="form-select" @change="cargarDocumentos">
          <option value="">Seleccione un paciente…</option>
          <option v-for="p in pacientes" :key="p.id" :value="p.id">
            {{ p.fullName || `#${p.id}` }}
          </option>
        </select>
      </div>
      <template #actions>
        <AppButton variant="primary" :icon="IconSearch" :disabled="!pacienteId" @click="cargarDocumentos">
          Buscar
        </AppButton>
        <AppButton variant="soft" @click="limpiarBusqueda">Limpiar</AppButton>
      </template>
    </FilterBar>

    <AppAlert v-if="error" variant="danger" :message="error" class="mb-3" />

    <!-- ============ Formulario de subida ============ -->
    <BaseCard v-if="showUpload" title="Subir documento" :icon="IconUpload" class="mb-3 u-fade-in">
      <form @submit.prevent="subirDocumento">
        <div class="row g-3">
          <div class="col-md-6">
            <label for="d-file" class="form-label">Archivo</label>
            <input id="d-file" ref="fileInput" type="file" class="form-control" accept=".pdf,.jpg,.jpeg,.png" required>
            <small class="text-app-muted">PDF, JPG o PNG. Máximo 10 MB.</small>
          </div>
          <div class="col-md-3">
            <label for="d-desc" class="form-label">
              Descripción <span class="text-app-subtle fw-normal">(opcional)</span>
            </label>
            <input id="d-desc" v-model="uploadDescription" type="text" class="form-control" placeholder="Descripción">
          </div>
          <div class="col-md-3">
            <label for="d-type" class="form-label">Tipo de documento</label>
            <select id="d-type" v-model="fileType" class="form-select" required>
              <option value="">Seleccionar…</option>
              <option v-for="t in TIPOS_DOCUMENTO" :key="t" :value="t">{{ t }}</option>
            </select>
          </div>
        </div>

        <div class="d-flex justify-content-end gap-2 mt-4">
          <AppButton type="button" variant="soft" @click="cancelarSubida">Cancelar</AppButton>
          <AppButton type="submit" variant="primary" :icon="IconUpload" :loading="subiendo">
            Subir documento
          </AppButton>
        </div>
      </form>
    </BaseCard>

    <!-- ============ Resultados ============ -->
    <LoadingState v-if="loading" label="Cargando documentos…" />

    <BaseCard v-else-if="!buscado" flush padding="none">
      <EmptyState
        :icon="IconDocument"
        title="Selecciona un paciente"
        message="Elige un paciente en el filtro superior para consultar sus documentos clínicos."
      />
    </BaseCard>

    <BaseCard v-else-if="!documentos.length" flush padding="none">
      <EmptyState
        :icon="IconDocument"
        title="Sin documentos"
        message="Este paciente todavía no tiene documentos cargados."
        action-label="Subir el primero"
        :action-icon="IconUpload"
        @action="showUpload = true"
      />
    </BaseCard>

    <div v-else class="doc-grid">
      <BaseCard v-for="d in documentos" :key="d.id" hoverable padding="sm">
        <div class="doc">
          <IconTile :icon="fileIconFor(d.fileName)" :tone="fileToneFor(d.fileName)" size="lg" />
          <div class="doc__body">
            <p class="doc__name" :title="d.fileName">{{ d.fileName }}</p>
            <p class="doc__date">{{ formatDate(d.uploadedAt) }}</p>
            <p v-if="d.description" class="doc__desc">{{ d.description }}</p>
          </div>
          <div class="doc__actions">
            <AppButton variant="soft" size="sm" :icon="IconView" @click="verDocumento(d)">Ver</AppButton>
            <AppButton
              variant="ghost"
              size="sm"
              :icon="IconDelete"
              aria-label="Eliminar documento"
              @click="confirmarEliminar(d)"
            />
          </div>
        </div>
      </BaseCard>
    </div>

    <ConfirmDialog
      :visible="deleteDialog"
      danger
      title="Eliminar documento"
      :message="`¿Seguro que deseas eliminar ${deleteTarget?.fileName ?? 'este documento'}?`"
      confirm-text="Eliminar"
      @confirm="eliminarDocumento"
      @cancel="deleteDialog = false"
    />

    <DocumentPreviewModal v-model="showPreview" :doc="previewDoc" />
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import {
  AppAlert,
  AppButton,
  BaseCard,
  ConfirmDialog,
  DocumentPreviewModal,
  EmptyState,
  FilterBar,
  IconTile,
  LoadingState,
  PageHeader
} from '@/components/ui'
import { IconDelete, IconDocument, IconSearch, IconUpload, IconView } from '@/lib/icons'
import { fileIconFor, fileToneFor, isPdfFile } from '@/lib/fileIcons'
import { medicalDocumentService } from '@/api/medicalDocuments'
import { patientService } from '@/api/patients'

const TIPOS_DOCUMENTO = [
  'Resultados de laboratorio',
  'Indicaciones médicas',
  'Historial externo',
  'Estudios en PDF',
  'Documentos administrativos'
]

const loading = ref(false)
const error = ref('')
const documentos = ref([])
const pacientes = ref([])
const pacienteId = ref('')
const buscado = ref(false)
const showUpload = ref(false)
const subiendo = ref(false)
const uploadDescription = ref('')
const fileType = ref('')
const fileInput = ref(null)
const deleteDialog = ref(false)
const deleteTarget = ref(null)

const showPreview = ref(false)
const previewDoc = ref(null)

onMounted(async () => {
  try {
    const res = await patientService.getAll()
    pacientes.value = (res.data?.value || res.data || []).filter((p) => p.fullName)
  } catch {
    pacientes.value = []
  }
})

const formatDate = (dateStr) => {
  if (!dateStr) return '—'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch {
    return dateStr
  }
}

const cargarDocumentos = async () => {
  if (!pacienteId.value) return
  loading.value = true
  error.value = ''
  buscado.value = true
  try {
    const res = await medicalDocumentService.getByPatient(pacienteId.value)
    documentos.value = res.data || []
  } catch (err) {
    if (err.response?.status === 204 || err.response?.status === 404) {
      documentos.value = []
    } else {
      error.value = 'Error al cargar documentos'
    }
  } finally {
    loading.value = false
  }
}

const limpiarBusqueda = () => {
  pacienteId.value = ''
  documentos.value = []
  buscado.value = false
  showUpload.value = false
}

const cancelarSubida = () => {
  showUpload.value = false
  fileType.value = ''
  uploadDescription.value = ''
}

const subirDocumento = async () => {
  const file = fileInput.value?.files?.[0]
  if (!file) return
  subiendo.value = true
  try {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('fileName', file.name)
    formData.append('fileType', fileType.value)
    formData.append('patientId', pacienteId.value)
    if (uploadDescription.value) formData.append('description', uploadDescription.value)
    await medicalDocumentService.upload(formData)
    cancelarSubida()
    fileInput.value.value = ''
    await cargarDocumentos()
  } catch {
    error.value = 'Error al subir el documento'
  } finally {
    subiendo.value = false
  }
}

const confirmarEliminar = (d) => {
  deleteTarget.value = d
  deleteDialog.value = true
}

// Los PDF no se previsualizan dentro del modal: el visor nativo del navegador
// en una pestaña aparte es mejor que un <embed> encajado.
const verDocumento = (d) => {
  if (isPdfFile(d.fileName)) {
    window.open(`/api/v1/MedicalDocument/${d.id}/file`, '_blank')
    return
  }
  previewDoc.value = d
  showPreview.value = true
}

const eliminarDocumento = async () => {
  deleteDialog.value = false
  try {
    await medicalDocumentService.remove(deleteTarget.value.id)
    documentos.value = documentos.value.filter((d) => d.id !== deleteTarget.value.id)
  } catch {
    error.value = 'Error al eliminar el documento'
  }
}
</script>

<style scoped>
.doc-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 1rem;
}

.doc {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: 0.25rem;
  height: 100%;
}

.doc__body {
  flex: 1;
  min-width: 0;
  width: 100%;
  margin-top: 0.5rem;
}

.doc__name {
  margin: 0;
  font-weight: 600;
  font-size: 0.875rem;
  color: var(--app-text-strong);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.doc__date {
  margin: 0.125rem 0 0;
  font-size: 0.75rem;
  color: var(--app-text-muted);
}

.doc__desc {
  margin: 0.375rem 0 0;
  font-size: 0.8rem;
  color: var(--app-text-muted);
}

.doc__actions {
  display: flex;
  gap: 0.375rem;
  margin-top: 0.875rem;
}
</style>
