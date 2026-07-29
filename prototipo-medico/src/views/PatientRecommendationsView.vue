<template>
  <div class="container mt-4">
    <div class="mb-4">
      <h3 class="fw-bold mb-0">Recomendaciones</h3>
      <p class="text-muted">Recomendaciones generadas por análisis de IA</p>
    </div>
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>
    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>
    <div v-else-if="items.length === 0" class="text-center py-5 text-muted">
      <p>No hay recomendaciones disponibles.</p>
    </div>
    <div v-else class="row g-4">
      <div v-for="r in items" :key="r.id" class="col-md-6">
        <div class="card shadow-sm border-0 rounded-4 h-100">
          <div class="card-body p-4">
            <div class="d-flex justify-content-between align-items-start mb-2">
              <h6 class="fw-bold mb-0">{{ r.title }}</h6>
              <span class="badge" :class="riskClass(r.riskLevel)">{{ r.riskLevel || 'N/A' }}</span>
            </div>
            <p class="text-muted small mb-0">{{ r.description }}</p>
            <small class="text-muted mt-2 d-block">{{ formatDate(r.createdAt) }}</small>
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
const items = ref([])

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' })
}

const riskClass = (level) => {
  const map = { Alto: 'bg-danger', Medio: 'bg-warning text-dark', Bajo: 'bg-success' }
  return map[level] || 'bg-secondary'
}

onMounted(async () => {
  try {
    const res = await portalService.getRecommendations()
    items.value = res.data || []
  } catch (err) {
    if (err.response?.status === 404) {
      error.value = 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
    } else {
      error.value = 'Error al cargar recomendaciones'
    }
  } finally {
    loading.value = false
  }
})
</script>
