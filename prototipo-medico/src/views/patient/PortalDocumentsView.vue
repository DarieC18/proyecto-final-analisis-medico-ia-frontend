<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Mis Documentos</h3>
        <p class="text-muted">Documentos médicos de tus consultas</p>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <div v-else-if="documentos.length === 0" class="text-center py-5 text-muted">
      <div class="display-4 mb-3"><FileIcon file-name="doc.pdf" style="width: 64px;" /></div>
      <p>No tienes documentos disponibles.</p>
    </div>

    <div v-else class="row g-4">
      <div class="col-md-4 col-sm-6" v-for="d in documentos" :key="d.id">
        <div class="card border-0 shadow-sm rounded-4 h-100">
          <div class="card-body p-4 text-center">
            <div style="width: 48px; margin: 0 auto;"><FileIcon :file-name="d.fileName" :file-path="d.filePath" /></div>
            <h6 class="fw-bold small mb-1">{{ d.fileName }}</h6>
            <p class="text-muted small mb-2">{{ d.fileType }}</p>
            <small class="text-muted d-block mb-3">{{ formatDate(d.uploadedAt) }}</small>
            <button @click="abrirVisor(d)" class="btn btn-sm btn-light border px-3">Ver Documento</button>
          </div>
        </div>
      </div>
    </div>

    <DocumentViewerModal
      :visible="showViewer"
      :document="viewerDoc"
      @close="showViewer = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { portalService } from '@/api/portal'
import DocumentViewerModal from '@/components/DocumentViewerModal.vue'
import FileIcon from '@/components/FileIcon.vue'

const loading = ref(true)
const error = ref('')
const documentos = ref([])
const showViewer = ref(false)
const viewerDoc = ref(null)

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch { return dateStr }
}

const abrirVisor = (doc) => {
  viewerDoc.value = doc
  showViewer.value = true
}

onMounted(async () => {
  loading.value = true
  try {
    const res = await portalService.getDocuments()
    documentos.value = res.data || []
  } catch {
    error.value = 'Error al cargar documentos'
  } finally {
    loading.value = false
  }
})
</script>
