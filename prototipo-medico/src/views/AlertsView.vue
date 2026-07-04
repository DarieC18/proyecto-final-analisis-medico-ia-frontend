<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Alertas Clínicas</h3>
        <p class="text-muted">Monitor de alertas generadas por signos vitales fuera de rango</p>
      </div>
      <div class="d-flex gap-2">
        <button @click="filtro = 'activas'" class="btn btn-sm" :class="filtro === 'activas' ? 'btn-danger' : 'btn-light border'">Activas</button>
        <button @click="filtro = 'todas'" class="btn btn-sm" :class="filtro === 'todas' ? 'btn-dark' : 'btn-light border'">Todas</button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <div v-else-if="alertas.length === 0" class="text-center py-5 text-muted">
      <div class="display-4 mb-3">🔔</div>
      <p>No hay alertas registradas.</p>
    </div>

    <div v-else>
      <div class="row g-4">
        <div class="col-md-6" v-for="a in alertas" :key="a.id">
          <div class="card border-0 shadow-sm rounded-4 h-100" :class="a.isResolved ? 'bg-light' : 'border-start border-4 border-danger'">
            <div class="card-body p-4">
              <div class="d-flex justify-content-between align-items-start mb-3">
                <div>
                  <h6 class="fw-bold mb-1">{{ a.alertType || 'Alerta Clínica' }}</h6>
                  <small class="text-muted">{{ formatDate(a.createdAt) }}</small>
                </div>
                <span class="badge rounded-pill px-3" :class="a.isResolved ? 'bg-success' : 'bg-danger'">
                  {{ a.isResolved ? 'Resuelta' : 'Activa' }}
                </span>
              </div>
              <p class="mb-2"><strong>Paciente:</strong> {{ a.patientName || `#${a.patientId}` }}</p>
              <p class="mb-2" v-if="a.appointmentId"><strong>Cita:</strong> #{{ a.appointmentId }}</p>
              <p class="mb-3" v-if="a.description">{{ a.description }}</p>
              <div v-if="a.severity" class="mb-3">
                <span class="badge rounded-pill px-3 py-2" :class="severityBadge(a.severity)">
                  {{ a.severity }}
                </span>
              </div>
              <div v-if="!a.isResolved" class="d-flex justify-content-end">
                <button @click="resolverAlerta(a)" class="btn btn-success btn-sm px-4 rounded-pill" :disabled="resolviendo === a.id">
                  <span v-if="resolviendo === a.id" class="spinner-border spinner-border-sm me-1"></span>
                  Marcar Resuelta
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import { alertService } from '@/api/alerts'

const loading = ref(true)
const error = ref('')
const alertas = ref([])
const filtro = ref('activas')
const resolviendo = ref(null)

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch { return dateStr }
}

const severityBadge = (sev) => {
  const map = { Leve: 'bg-success', Moderado: 'bg-warning text-dark', Severo: 'bg-danger', Critical: 'bg-dark' }
  return map[sev] || 'bg-secondary'
}

const cargarAlertas = async () => {
  loading.value = true
  error.value = ''
  try {
    let res
    if (filtro.value === 'activas') {
      res = await alertService.getActive()
    } else if (filtro.value === 'paciente') {
      res = await alertService.getByPatient(filtroPaciente.value)
    } else {
      res = await alertService.getActive()
    }
    alertas.value = res.data || []
  } catch (err) {
    if (err.response?.status === 204) {
      alertas.value = []
    } else {
      error.value = 'Error al cargar alertas'
    }
  } finally {
    loading.value = false
  }
}

const resolverAlerta = async (a) => {
  resolviendo.value = a.id
  try {
    await alertService.resolve(a.id)
    a.isResolved = true
  } catch (err) {
    error.value = 'Error al resolver la alerta'
  } finally {
    resolviendo.value = null
  }
}

watch(filtro, () => { cargarAlertas() })

onMounted(cargarAlertas)
</script>
