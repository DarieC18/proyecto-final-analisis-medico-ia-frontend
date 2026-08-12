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
            <label class="form-label text-muted small fw-bold text-uppercase">Paciente</label>
            <select v-model="pacienteId" class="form-select bg-light border-0" @change="cargarDocumentos">
              <option value="">Seleccione un paciente...</option>
              <option v-for="p in pacientes" :key="p.id" :value="p.id">{{ p.fullName || `#${p.id}` }}</option>
            </select>
          </div>
          <div class="col-md-3">
            <button @click="cargarDocumentos" class="btn btn-dark px-4 w-100" :disabled="!pacienteId">Buscar</button>
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
                <button @click="verDocumento(d)" class="btn btn-sm btn-light border px-3">Ver</button>
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
                <label class="form-label fw-medium">📄 Archivo</label>
                <input ref="fileInput" type="file" class="form-control" accept=".pdf,.jpg,.jpeg,.png" required>
                <small class="text-muted">PDF, JPG o PNG. Máximo 10MB.</small>
              </div>
              <div class="col-md-3">
                <label class="form-label fw-medium">📝 Descripción <span class="text-muted fw-normal">(opcional)</span></label>
                <input v-model="uploadDescription" type="text" class="form-control" placeholder="Descripción">
              </div>
              <div class="col-md-3">
                <label class="form-label fw-medium">Tipo de documento</label>
                <select v-model="fileType" class="form-select" required>
                  <option value="">Seleccionar...</option>
                  <option value="Resultados de laboratorio">Resultados de laboratorio</option>
                  <option value="Indicaciones médicas">Indicaciones médicas</option>
                  <option value="Historial externo">Historial externo</option>
                  <option value="Estudios en PDF">Estudios en PDF</option>
                  <option value="Documentos administrativos">Documentos administrativos</option>
                </select>
              </div>
            </div>
            <div v-if="uploadError" class="alert alert-danger border-0 rounded-3 py-2 small mt-3 mb-0">{{ uploadError }}</div>
            <div v-if="uploadSuccess" class="alert alert-success border-0 rounded-3 py-2 small mt-3 mb-0">{{ uploadSuccess }}</div>
            <div class="d-flex justify-content-end gap-3 mt-4">
              <button type="button" @click="showUpload = false; fileType = ''; uploadError = ''; uploadSuccess = ''" class="btn btn-outline-secondary rounded-pill px-4">Cancelar</button>
              <button type="submit" class="btn btn-success rounded-pill px-4 shadow-sm" :disabled="subiendo">
                <span v-if="subiendo" class="spinner-border spinner-border-sm me-2"></span>
                📤 Subir Documento
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

    <div v-if="showPreview && previewDoc" class="modal-backdrop fade show"></div>
    <div v-if="showPreview && previewDoc" class="modal d-block" tabindex="-1" @click.self="showPreview = false">
      <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content border-0 rounded-4 shadow">
          <div class="modal-header border-0 pb-0">
            <h5 class="fw-bold">{{ previewDoc.fileName }}</h5>
            <button type="button" class="btn-close" @click="showPreview = false"></button>
          </div>
          <div class="modal-body p-3 text-center">
            <template v-if="previewDoc.fileName?.toLowerCase().endsWith('.pdf')">
              <iframe
                :src="`/api/v1/MedicalDocument/${previewDoc.id}/file`"
                class="w-100 border-0 rounded-3"
                style="height: 70vh;"
              />
            </template>
            <template v-else>
              <img
                :src="`/api/v1/MedicalDocument/${previewDoc.id}/file`"
                class="img-fluid rounded-3"
                style="max-height: 70vh;"
                alt="Documento"
              />
            </template>
          </div>
          <div class="modal-footer border-0 pt-0">
            <a :href="`/api/v1/MedicalDocument/${previewDoc.id}/file`" :download="previewDoc.fileName" class="btn btn-outline-primary rounded-pill px-4">Descargar</a>
            <a :href="`/api/v1/MedicalDocument/${previewDoc.id}/file`" target="_blank" class="btn btn-outline-dark rounded-pill px-4">Abrir en nueva pestaña</a>
            <button @click="showPreview = false" class="btn btn-dark rounded-pill px-4">Cerrar</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { medicalDocumentService } from '@/api/medicalDocuments'
import { patientService } from '@/api/patients'
import ConfirmDialog from '@/components/ConfirmDialog.vue'

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
const uploadError = ref('')
const uploadSuccess = ref('')
const deleteDialog = ref(false)
const deleteTarget = ref(null)

const showPreview = ref(false)
const previewDoc = ref(null)

onMounted(async () => {
  try {
    const res = await patientService.getAll()
    pacientes.value = (res.data?.value || res.data || []).filter(p => p.fullName)
  } catch {
    pacientes.value = []
  }
})

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
  uploadError.value = ''
  uploadSuccess.value = ''
  if (!file) return
  if (!fileType.value) {
    uploadError.value = 'Selecciona el tipo de documento.'
    return
  }
  if (file.size > 10 * 1024 * 1024) {
    uploadError.value = 'El archivo supera el límite de 10 MB.'
    return
  }
  subiendo.value = true
  try {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('fileName', file.name)
    formData.append('fileType', fileType.value)
    formData.append('patientId', pacienteId.value)
    if (uploadDescription.value) formData.append('description', uploadDescription.value)
    await medicalDocumentService.upload(formData)
    uploadSuccess.value = 'Documento subido exitosamente.'
    uploadDescription.value = ''
    fileType.value = ''
    fileInput.value.value = ''
    await cargarDocumentos()
    setTimeout(() => {
      uploadSuccess.value = ''
      showUpload.value = false
    }, 1500)
  } catch (err) {
    const msg = err.response?.data?.message
    uploadError.value = msg || 'Error al subir el documento. Verifica el archivo e inténtalo de nuevo.'
  } finally {
    subiendo.value = false
  }
}

const confirmarEliminar = (d) => {
  deleteTarget.value = d
  deleteDialog.value = true
}

const verDocumento = (d) => {
  previewDoc.value = d
  showPreview.value = true
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

<style scoped>
.form-control {
  border: 1px solid #d1d5db;
  background: #fff;
  padding: 10px 12px;
  border-radius: 8px;
  transition: all 0.2s ease;
}
.form-control:focus {
  border-color: #0d6efd;
  box-shadow: 0 0 0 3px rgba(13, 110, 253, 0.15);
  background: #fff;
}
.form-control::placeholder {
  color: #94a3b8;
  font-size: 0.9rem;
}
.form-label {
  font-size: 0.85rem;
  color: #334155;
  margin-bottom: 4px;
}
</style>
