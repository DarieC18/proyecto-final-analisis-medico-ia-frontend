<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Recomendaciones</h3>
        <p class="text-muted">Indicaciones y recetas de tus médicos</p>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <div v-else-if="recomendaciones.length === 0" class="text-center py-5 text-muted">
      <div class="display-4 mb-3">💡</div>
      <p>No tienes recomendaciones pendientes.</p>
    </div>

    <div v-else class="row g-4">
      <div class="col-md-6" v-for="r in recomendaciones" :key="r.id">
        <div class="card border-0 shadow-sm rounded-4 h-100 border-start border-4" :class="riskBorder(r.riskLevel)">
          <div class="card-body p-4">
            <div class="d-flex justify-content-between align-items-start mb-3">
              <h6 class="fw-bold mb-0">{{ r.title }}</h6>
              <span class="badge rounded-pill px-3" :class="riskBadge(r.riskLevel)">{{ r.riskLevel }}</span>
            </div>
            <p class="text-muted mb-2">{{ r.description }}</p>
            <small class="text-muted">{{ formatDate(r.createdAt) }}</small>
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
const recomendaciones = ref([])

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric'
    })
  } catch { return dateStr }
}

const riskBorder = (level) => {
  const map = { Alto: 'border-danger', Medio: 'border-warning', Bajo: 'border-success' }
  return map[level] || 'border-info'
}

const riskBadge = (level) => {
  const map = { Alto: 'bg-danger', Medio: 'bg-warning text-dark', Bajo: 'bg-success' }
  return map[level] || 'bg-info'
}

onMounted(async () => {
  loading.value = true
  try {
    const res = await portalService.getRecommendations()
    recomendaciones.value = res.data || []
  } catch {
    error.value = 'Error al cargar recomendaciones'
  } finally {
    loading.value = false
  }
})
</script>
