<template>
  <div class="container mt-4">
    <div class="mb-4">
      <h3 class="fw-bold mb-0">Mis Documentos</h3>
      <p class="text-muted">Documentos médicos compartidos por tu médico</p>
    </div>
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>
    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>
    <div v-else-if="docs.length === 0" class="text-center py-5 text-muted">
      <p>No hay documentos disponibles.</p>
    </div>
    <div v-else class="row g-4">
      <div v-for="d in docs" :key="d.id" class="col-md-6 col-lg-4">
        <div class="card shadow-sm border-0 rounded-4 h-100">
          <div class="card-body p-4">
            <div class="d-flex align-items-center gap-3 mb-3">
              <span class="fs-2">📄</span>
              <div class="text-truncate">
                <h6 class="fw-bold mb-0 text-truncate">{{ d.fileName }}</h6>
                <small class="text-muted">{{ d.fileType || 'Documento' }}</small>
              </div>
            </div>
            <div class="d-flex justify-content-between align-items-center">
              <small class="text-muted">{{ formatDate(d.uploadedAt) }}</small>
              <small class="text-muted">{{ d.uploadedByUserName || 'Médico' }}</small>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { portalService } from '@/api/portal'

const loading = ref(true)
const error = ref('')
const docs = ref([])

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' })
}

onMounted(async () => {
  try {
    const res = await portalService.getDocuments()
    docs.value = res.data || []
  } catch (err) {
    if (err.response?.status === 404) {
      error.value = 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
    } else {
      error.value = 'Error al cargar documentos'
    }
  } finally {
    loading.value = false
  }
})
</script>
