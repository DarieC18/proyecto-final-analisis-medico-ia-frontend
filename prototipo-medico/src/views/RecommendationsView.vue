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
        <div v-if="pacienteSeleccionado" class="mb-3 d-flex align-items-center gap-2">
          <span class="badge bg-primary rounded-pill px-3 py-2 fs-6">
            {{ pacienteSeleccionado.fullName }}
            <span class="text-white-50 ms-1">· {{ pacienteSeleccionado.identificationNumber }}</span>
          </span>
          <button @click="limpiarBusqueda" class="btn btn-sm btn-outline-secondary rounded-pill">✕ Cambiar</button>
        </div>

        <div v-else>
          <div class="row g-2 align-items-end">
            <div class="col-md-6">
              <label class="form-label text-muted small fw-bold text-uppercase">Buscar paciente</label>
              <input
                v-model="busqueda"
                type="text"
                class="form-control bg-light border-0"
                placeholder="Nombre o número de identificación"
                @keyup.enter="buscarPacientes"
              >
            </div>
            <div class="col-md-3">
              <button @click="buscarPacientes" class="btn btn-dark px-4 w-100" :disabled="buscando">
                <span v-if="buscando" class="spinner-border spinner-border-sm me-1"></span>
                Buscar
              </button>
            </div>
            <div class="col-md-3">
              <button @click="limpiarBusqueda" class="btn btn-light border px-4 w-100">Limpiar</button>
            </div>
          </div>

          <div v-if="pacientesEncontrados.length > 0" class="mt-2 border rounded-3 overflow-hidden">
            <div
              v-for="p in pacientesEncontrados"
              :key="p.id"
              class="px-3 py-2 d-flex justify-content-between align-items-center cursor-pointer"
              style="cursor: pointer;"
              :class="{ 'border-top': p !== pacientesEncontrados[0] }"
              @click="seleccionarPaciente(p)"
            >
              <span class="fw-medium">{{ p.fullName }}</span>
              <small class="text-muted">{{ p.identificationNumber }}</small>
            </div>
          </div>

          <div v-else-if="buscado && !buscando" class="mt-2 text-muted small ps-1">
            No se encontraron pacientes.
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
      <p v-if="!pacienteSeleccionado">Busque un paciente por nombre o identificación para ver sus recomendaciones.</p>
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
import { patientService } from '@/api/patients'

const loading = ref(false)
const error = ref('')
const recomendaciones = ref([])
const busqueda = ref('')
const buscando = ref(false)
const buscado = ref(false)
const pacientesEncontrados = ref([])
const pacienteSeleccionado = ref(null)

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

const buscarPacientes = async () => {
  if (!busqueda.value.trim()) return
  buscando.value = true
  buscado.value = false
  pacientesEncontrados.value = []
  try {
    const res = await patientService.search(busqueda.value)
    pacientesEncontrados.value = res.data || []
    buscado.value = true
  } catch {
    pacientesEncontrados.value = []
    buscado.value = true
  } finally {
    buscando.value = false
  }
}

const seleccionarPaciente = async (p) => {
  pacienteSeleccionado.value = p
  pacientesEncontrados.value = []
  loading.value = true
  error.value = ''
  try {
    const res = await recommendationService.getByPatient(p.id)
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
  busqueda.value = ''
  buscado.value = false
  pacientesEncontrados.value = []
  pacienteSeleccionado.value = null
  recomendaciones.value = []
}
</script>
