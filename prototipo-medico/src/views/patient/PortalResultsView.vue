<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Resultados de Análisis</h3>
        <p class="text-muted">Resultados de análisis de inteligencia artificial</p>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <div v-else-if="resultados.length === 0" class="text-center py-5 text-muted">
      <div class="display-4 mb-3">🧠</div>
      <p>No hay resultados de análisis disponibles.</p>
    </div>

    <div v-else>
      <div v-for="r in resultados" :key="r.id" class="card border-0 shadow-sm rounded-4 mb-4">
        <div class="card-body p-5">
          <div class="d-flex justify-content-between align-items-start mb-4">
            <div>
              <h5 class="fw-bold mb-1">{{ r.analysisType || 'Análisis Clínico' }}</h5>
              <small class="text-muted">{{ formatDate(r.createdAt) }} - Modelo: {{ r.modelUsed }}</small>
            </div>
            <span class="badge rounded-pill px-3 py-2" :class="r.isReviewed ? 'bg-success' : 'bg-info'">
              {{ r.isReviewed ? 'Revisado' : 'Nuevo' }}
            </span>
          </div>

          <div class="bg-light p-4 rounded-4 mb-3">
            <h6 class="fw-bold text-muted small text-uppercase mb-2">Resultado</h6>
            <p class="mb-0" style="white-space: pre-wrap;">{{ r.aiResponse }}</p>
          </div>

          <div v-if="r.promptUsed" class="mt-3">
            <h6 class="fw-bold text-muted small text-uppercase mb-2">Prompt utilizado</h6>
            <p class="text-muted small">{{ r.promptUsed }}</p>
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
const resultados = ref([])

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch { return dateStr }
}

onMounted(async () => {
  loading.value = true
  try {
    const res = await portalService.getResults()
    resultados.value = res.data || []
  } catch {
    error.value = 'Error al cargar resultados'
  } finally {
    loading.value = false
  }
})
</script>
