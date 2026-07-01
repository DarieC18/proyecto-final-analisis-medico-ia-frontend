<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Documentos Médicos</h3>
        <p class="text-muted">Gestión de documentos clínicos por paciente</p>
      </div>
    </div>

    <div class="card shadow-sm mb-4 border-0">
      <div class="card-body p-3">
        <div class="row g-2 align-items-end">
          <div class="col-md-4">
            <label class="form-label text-muted small fw-bold text-uppercase">Paciente ID</label>
            <input v-model="pacienteId" type="number" class="form-control bg-light border-0" placeholder="Ingrese ID del paciente">
          </div>
          <div class="col-md-3">
            <button @click="cargarDocumentos" class="btn btn-dark px-4 w-100">Buscar</button>
          </div>
          <div class="col-md-3">
            <button @click="limpiarBusqueda" class="btn btn-light border px-4 w-100">Limpiar</button>
          </div>
          <div class="col-md-2" v-if="documentos.length">
            <button @click="showUpload = true" class="btn btn-primary px-4 w-100">+ Subir</button>
          </div>
        </div>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <div v-else-if="documentos.length === 0 && buscado" class="text-center py-5 text-muted">
      <div class="display-4 mb-3">📄</div>
      <p>No hay documentos para este paciente.</p>
    </div>

    <div v-else-if="buscado">
      <div class="row g-4">
        <div class="col-md-4 col-sm-6" v-for="d in documentos" :key="d.id">
          <div class="card border-0 shadow-sm rounded-4 h-100">
            <div class="card-body p-4">
              <div class="text-center mb-3">
                <div class="display-5">{{ fileIcon(d.fileName) }}</div>
              </div>
              <h6 class="fw-bold text-center mb-1">{{ d.fileName }}</h6>
              <p class="text-muted small text-center mb-2">{{ formatDate(d.uploadedAt) }}</p>
              <p class="small text-muted text-center mb-3" v-if="d.description">{{ d.description }}</p>
              <div class="d-flex justify-content-center gap-2">
                <a :href="d.fileUrl" target="_blank" class="btn btn-sm btn-light border px-3">Ver</a>
                <button @click="confirmarEliminar(d)" class="btn btn-sm btn-light border text-danger px-3">Eliminar</button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-if="showUpload" class="card shadow-sm border-0 rounded-4 mt-4">
        <div class="card-body p-4">
          <h5 class="fw-bold mb-4">Subir Documento</h5>
          <form @submit.prevent="subirDocumento">
            <div class="row g-3">
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Archivo</label>
                <input ref="fileInput" type="file" class="form-control bg-light border-0" accept=".pdf,.jpg,.jpeg,.png" required>
                <small class="text-muted">PDF, JPG o PNG. Máximo 10MB.</small>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Descripción (opcional)</label>
                <input v-model="uploadDescription" type="text" class="form-control bg-light border-0" placeholder="Descripción del documento">
              </div>
            </div>
            <div class="d-flex justify-content-end gap-3 mt-4">
              <button type="button" @click="showUpload = false" class="btn btn-light border px-4">Cancelar</button>
              <button type="submit" class="btn btn-success px-4 shadow-sm" :disabled="subiendo">
                <span v-if="subiendo" class="spinner-border spinner-border-sm me-2"></span>
                Subir Documento
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-5 text-muted">
      <div class="display-4 mb-3">📄</div>
      <p>Ingrese un ID de paciente para buscar sus documentos.</p>
    </div>

    <ConfirmDialog
      :visible="deleteDialog"
      title="Eliminar Documento"
      :message="`¿Está seguro que desea eliminar el documento ${deleteTarget?.fileName}?`"
      confirmText="Eliminar"
      :danger="true"
      @confirm="eliminarDocumento"
      @cancel="deleteDialog = false"
    />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { medicalDocumentService } from '@/api/medicalDocuments'
import ConfirmDialog from '@/components/ConfirmDialog.vue'

const loading = ref(false)
const error = ref('')
const documentos = ref([])
const pacienteId = ref('')
const buscado = ref(false)
const showUpload = ref(false)
const subiendo = ref(false)
const uploadDescription = ref('')
const fileInput = ref(null)
const deleteDialog = ref(false)
const deleteTarget = ref(null)

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch { return dateStr }
}

const fileIcon = (name) => {
  if (!name) return '📄'
  const ext = name.split('.').pop()?.toLowerCase()
  if (ext === 'pdf') return '📕'
  if (['jpg', 'jpeg', 'png'].includes(ext)) return '🖼️'
  return '📄'
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

const subirDocumento = async () => {
  const file = fileInput.value?.files?.[0]
  if (!file) return
  subiendo.value = true
  try {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('patientId', pacienteId.value)
    if (uploadDescription.value) formData.append('description', uploadDescription.value)
    await medicalDocumentService.upload(formData)
    showUpload.value = false
    uploadDescription.value = ''
    fileInput.value.value = ''
    await cargarDocumentos()
  } catch (err) {
    error.value = 'Error al subir el documento'
  } finally {
    subiendo.value = false
  }
}

const confirmarEliminar = (d) => {
  deleteTarget.value = d
  deleteDialog.value = true
}

const eliminarDocumento = async () => {
  deleteDialog.value = false
  try {
    await medicalDocumentService.remove(deleteTarget.value.id)
    documentos.value = documentos.value.filter(d => d.id !== deleteTarget.value.id)
  } catch (err) {
    error.value = 'Error al eliminar el documento'
  }
}
</script>
