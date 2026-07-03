<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Reportes Clínicos</h3>
        <p class="text-muted">Reportes detallados de pacientes y citas</p>
      </div>
    </div>

    <div class="row g-4 mb-4">
      <div class="col-md-6">
        <div class="card border-0 shadow-sm rounded-4">
          <div class="card-body p-4">
            <h5 class="fw-bold mb-3">📋 Reporte de Cita</h5>
            <div class="row g-2">
              <div class="col-8">
                <input v-model="citaId" type="number" class="form-control bg-light border-0" placeholder="ID de la cita">
              </div>
              <div class="col-4">
                <button @click="cargarReporteCita" class="btn btn-dark w-100" :disabled="!citaId">Ver</button>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="card border-0 shadow-sm rounded-4">
          <div class="card-body p-4">
            <h5 class="fw-bold mb-3">📊 Reporte de Paciente</h5>
            <div class="row g-2">
              <div class="col-8">
                <input v-model="pacienteId" type="number" class="form-control bg-light border-0" placeholder="ID del paciente">
              </div>
              <div class="col-4">
                <button @click="cargarReportePaciente" class="btn btn-dark w-100" :disabled="!pacienteId">Ver</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <div v-else-if="reporte" class="card shadow-sm border-0 rounded-4">
      <div class="card-body p-5">
        <div class="d-flex justify-content-between align-items-center mb-4">
          <h4 class="fw-bold mb-0">{{ reporteTitulo }}</h4>
          <button @click="reporte = null" class="btn btn-light border px-3">Cerrar</button>
        </div>
        <hr>

        <div v-if="reporte.patientInfo" class="mb-4">
          <h6 class="fw-bold text-muted text-uppercase mb-3">Información del Paciente</h6>
          <div class="row g-3">
            <div class="col-md-4"><strong>Nombre:</strong> {{ reporte.patientInfo.fullName }}</div>
            <div class="col-md-4"><strong>ID:</strong> {{ reporte.patientInfo.identificationNumber }}</div>
            <div class="col-md-4"><strong>Teléfono:</strong> {{ reporte.patientInfo.phoneNumber }}</div>
          </div>
        </div>

        <div v-if="reporte.citas?.length" class="mb-4">
          <h6 class="fw-bold text-muted text-uppercase mb-3">Historial de Citas</h6>
          <div class="table-responsive">
            <table class="table table-sm table-hover">
              <thead class="bg-light">
                <tr>
                  <th>Fecha</th>
                  <th>Motivo</th>
                  <th>Estado</th>
                  <th>Médico</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="c in reporte.citas" :key="c.id">
                  <td>{{ formatDate(c.appointmentDate) }}</td>
                  <td>{{ c.reason }}</td>
                  <td><StatusBadge :text="c.status" :variant="c.status?.toLowerCase()" /></td>
                  <td>{{ c.doctorName }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div v-if="reporte.alertas?.length" class="mb-4">
          <h6 class="fw-bold text-muted text-uppercase mb-3">Alertas</h6>
          <div v-for="a in reporte.alertas" :key="a.id" class="alert border-0 rounded-3 mb-2" :class="a.isResolved ? 'bg-light text-muted' : 'bg-danger bg-opacity-10 text-danger'">
            <strong>{{ a.alertType }}</strong> - {{ a.description }}
            <span v-if="a.isResolved" class="badge bg-success ms-2">Resuelta</span>
          </div>
        </div>

        <div v-if="reporte.aiAnalyses?.length" class="mb-4">
          <h6 class="fw-bold text-muted text-uppercase mb-3">Análisis IA</h6>
          <div v-for="ia in reporte.aiAnalyses" :key="ia.id" class="card bg-light border-0 p-3 rounded-4 mb-2">
            <p class="mb-1"><strong>Modelo:</strong> {{ ia.modelUsed }}</p>
            <p class="mb-0"><strong>Resultado:</strong> {{ ia.aiResponse }}</p>
          </div>
        </div>

        <div v-if="reporte.recommendations?.length" class="mb-4">
          <h6 class="fw-bold text-muted text-uppercase mb-3">Recomendaciones</h6>
          <div v-for="r in reporte.recommendations" :key="r.id" class="card bg-light border-0 p-3 rounded-4 mb-2">
            <p class="mb-0"><strong>{{ r.title }}</strong> - {{ r.description }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { reportService } from '@/api/reports'
import StatusBadge from '@/components/StatusBadge.vue'

const loading = ref(false)
const error = ref('')
const reporte = ref(null)
const reporteTitulo = ref('')
const citaId = ref('')
const pacienteId = ref('')

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch { return dateStr }
}

const cargarReporteCita = async () => {
  if (!citaId.value) return
  loading.value = true
  error.value = ''
  reporte.value = null
  try {
    const res = await reportService.getAppointmentReport(citaId.value)
    reporte.value = res.data
    reporteTitulo.value = `Reporte de Cita #${citaId.value}`
  } catch (err) {
    if (err.response?.status === 404) {
      error.value = 'Cita no encontrada.'
    } else {
      error.value = 'Error al cargar reporte de cita'
    }
  } finally {
    loading.value = false
  }
}

const cargarReportePaciente = async () => {
  if (!pacienteId.value) return
  loading.value = true
  error.value = ''
  reporte.value = null
  try {
    const res = await reportService.getPatientReport(pacienteId.value)
    reporte.value = res.data
    reporteTitulo.value = `Reporte del Paciente #${pacienteId.value}`
  } catch (err) {
    if (err.response?.status === 404) {
      error.value = 'Paciente no encontrado.'
    } else {
      error.value = 'Error al cargar reporte de paciente'
    }
  } finally {
    loading.value = false
  }
}
</script>
