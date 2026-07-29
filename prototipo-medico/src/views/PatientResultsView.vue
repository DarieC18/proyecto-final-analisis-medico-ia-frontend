<template>
  <div class="container mt-4">
    <div class="mb-4">
      <h3 class="fw-bold mb-0">Resultados de Análisis</h3>
      <p class="text-muted">Análisis de IA generados para tus consultas</p>
    </div>
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>
    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>
    <div v-else-if="results.length === 0" class="text-center py-5 text-muted">
      <p>No hay resultados disponibles.</p>
    </div>
    <div v-else>
      <div v-for="r in results" :key="r.id" class="card shadow-sm border-0 rounded-4 mb-4">
        <div class="card-body p-4">
          <div class="d-flex justify-content-between align-items-start mb-3">
            <div>
              <h6 class="fw-bold mb-1">{{ r.analysisType || 'Análisis' }}</h6>
              <small class="text-muted">{{ formatDate(r.createdAt) }}</small>
            </div>
            <StatusBadge :text="r.isReviewed ? 'Revisado' : 'Pendiente'" :variant="r.isReviewed ? 'active' : 'inactive'" />
          </div>
          <div class="bg-light rounded-3 p-3 mb-3">
            <small class="text-muted fw-bold d-block mb-1">Resultado:</small>
            <p class="mb-0" style="white-space: pre-wrap;">{{ r.aiResponse || 'Sin respuesta' }}</p>
          </div>
          <div class="d-flex justify-content-between">
            <small class="text-muted">Modelo: {{ r.modelUsed || 'N/A' }}</small>
            <small class="text-muted">Estado: {{ r.status || 'N/A' }}</small>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { portalService } from '@/api/portal'
import StatusBadge from '@/components/StatusBadge.vue'

const loading = ref(true)
const error = ref('')
const results = ref([])

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit' })
}

onMounted(async () => {
  try {
    const res = await portalService.getResults()
    results.value = res.data || []
  } catch (err) {
    if (err.response?.status === 404) {
      error.value = 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
    } else {
      error.value = 'Error al cargar resultados'
    }
  } finally {
    loading.value = false
  }
})
</script>
