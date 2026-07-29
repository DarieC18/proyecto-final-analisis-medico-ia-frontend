<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Recomendaciones</h3>
        <p class="text-muted">Recomendaciones generadas por el sistema por paciente</p>
      </div>
    </div>

    <div class="card shadow-sm mb-4 border-0">
      <div class="card-body p-3">
        <div class="row g-2 align-items-end">
          <div class="col-md-6">
            <label class="form-label text-muted small fw-bold text-uppercase">Buscar por Paciente ID</label>
            <input v-model="pacienteId" type="number" class="form-control bg-light border-0" placeholder="Ingrese ID del paciente">
          </div>
          <div class="col-md-3">
            <button @click="cargarPorPaciente" class="btn btn-dark px-4 w-100">Buscar</button>
          </div>
          <div class="col-md-3">
            <button @click="limpiarBusqueda" class="btn btn-light border px-4 w-100">Limpiar</button>
          </div>
        </div>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <div v-else-if="recomendaciones.length === 0" class="text-center py-5 text-muted">
      <div class="display-4 mb-3">💡</div>
      <p v-if="!buscado">Ingrese un ID de paciente para ver sus recomendaciones.</p>
      <p v-else>No hay recomendaciones para este paciente.</p>
    </div>

    <div v-else>
      <div class="row g-4">
        <div class="col-md-6" v-for="r in recomendaciones" :key="r.id">
          <div class="card border-0 shadow-sm rounded-4 h-100 border-start border-4 border-info">
            <div class="card-body p-4">
              <div class="d-flex justify-content-between align-items-start mb-3">
                <h6 class="fw-bold mb-0">{{ r.title || 'Recomendación' }}</h6>
                <small class="text-muted">{{ formatDate(r.createdAt) }}</small>
              </div>
              <p class="text-muted mb-2" v-if="r.description">{{ r.description }}</p>
              <p class="mb-1" v-if="r.priority">
                <span class="badge rounded-pill px-3" :class="priorityBadge(r.priority)">{{ r.priority }}</span>
              </p>
              <p class="mb-0 small text-muted" v-if="r.appointmentId">Relacionado a cita #{{ r.appointmentId }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { recommendationService } from '@/api/recommendations'

const loading = ref(false)
const error = ref('')
const recomendaciones = ref([])
const pacienteId = ref('')
const buscado = ref(false)

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch { return dateStr }
}

const priorityBadge = (p) => {
  const map = { Alta: 'bg-danger', Media: 'bg-warning text-dark', Baja: 'bg-success' }
  return map[p] || 'bg-info'
}

const cargarPorPaciente = async () => {
  if (!pacienteId.value) return
  loading.value = true
  error.value = ''
  buscado.value = true
  try {
    const res = await recommendationService.getByPatient(pacienteId.value)
    recomendaciones.value = res.data || []
  } catch (err) {
    if (err.response?.status === 204 || err.response?.status === 404) {
      recomendaciones.value = []
    } else {
      error.value = 'Error al cargar recomendaciones'
    }
  } finally {
    loading.value = false
  }
}

const limpiarBusqueda = () => {
  pacienteId.value = ''
  recomendaciones.value = []
  buscado.value = false
}
</script>
