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
            <h5 class="fw-bold mb-3">📊 Reporte de Paciente</h5>
            <div class="row g-2">
              <div class="col-8">
                <select v-model="pacienteId" class="form-select bg-light border-0" @change="cargarReportePaciente">
                  <option value="">Seleccione un paciente...</option>
                  <option v-for="p in pacientes" :key="p.id" :value="p.id">{{ p.fullName || `#${p.id}` }}</option>
                </select>
              </div>
              <div class="col-4">
                <button @click="cargarReportePaciente" class="btn btn-dark w-100" :disabled="!pacienteId">Ver</button>
              </div>
            </div>
          </div>
        </div>
      </div>
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

        <div v-if="reporte.patientDetail" class="mb-4">
          <h6 class="fw-bold text-muted text-uppercase mb-3">Información del Paciente</h6>
          <div class="row g-3">
            <div class="col-md-4"><strong>Nombre:</strong> {{ reporte.patientDetail.fullName || '-' }}</div>
            <div class="col-md-4"><strong>ID:</strong> {{ reporte.patientDetail.identificationNumber || '-' }}</div>
            <div class="col-md-4"><strong>Teléfono:</strong> {{ reporte.patientDetail.phoneNumber }}</div>
          </div>
        </div>

        <div v-if="reporte.appointmentDate" class="mb-4">
          <h6 class="fw-bold text-muted text-uppercase mb-3">Información de la Cita</h6>
          <div class="row g-3">
            <div class="col-md-3"><strong>Fecha:</strong> {{ formatDate(reporte.appointmentDate) }}</div>
            <div class="col-md-3"><strong>Estado:</strong> <StatusBadge :text="reporte.status" :variant="reporte.status?.toLowerCase()" /></div>
            <div class="col-md-3"><strong>Motivo:</strong> {{ reporte.reason }}</div>
            <div class="col-md-3"><strong>Notas:</strong> {{ reporte.notes || '-' }}</div>
          </div>
        </div>

        <div v-if="reporte.patientDetail?.medicalRecords?.length || reporte.medicalRecords?.length" class="mb-4">
          <h6 class="fw-bold text-muted text-uppercase mb-3">Historial Médico</h6>
          <div class="table-responsive">
            <table class="table table-sm table-hover">
              <thead class="bg-light">
                <tr>
                  <th>Fecha</th>
                  <th>Diagnóstico</th>
                  <th>Notas</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="r in (reporte.patientDetail?.medicalRecords || reporte.medicalRecords)" :key="r.id">
                  <td>{{ formatDate(r.appointmentDate || r.createdAt) }}</td>
                  <td>{{ r.diagnosisInitial || r.reason || '-' }}</td>
                  <td>{{ r.notes || '-' }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div v-if="reporte.symptoms?.length" class="mb-4">
          <h6 class="fw-bold text-muted text-uppercase mb-3">Síntomas</h6>
          <div class="table-responsive">
            <table class="table table-sm table-hover">
              <thead class="bg-light">
                <tr><th>Síntoma</th><th>Severidad</th><th>Notas</th></tr>
              </thead>
              <tbody>
                <tr v-for="s in reporte.symptoms" :key="s.id">
                  <td>{{ s.name }}</td>
                  <td>{{ s.severity }}</td>
                  <td>{{ s.notes || '-' }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div v-if="reporte.vitalSigns?.length" class="mb-4">
          <h6 class="fw-bold text-muted text-uppercase mb-3">Signos Vitales</h6>
          <div class="table-responsive">
            <table class="table table-sm table-hover">
              <thead class="bg-light">
                <tr><th>Temperatura</th><th>Frec. Cardíaca</th><th>Presión</th><th>Saturación</th></tr>
              </thead>
              <tbody>
                <tr v-for="v in reporte.vitalSigns" :key="v.id">
                  <td>{{ v.temperature }} °C</td>
                  <td>{{ v.heartRate }} lpm</td>
                  <td>{{ v.systolicPressure }}/{{ v.diastolicPressure }}</td>
                  <td>{{ v.oxygenSaturation }}%</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div v-if="reporte.alerts?.length" class="mb-4">
          <h6 class="fw-bold text-muted text-uppercase mb-3">Alertas</h6>
          <div v-for="a in reporte.alerts" :key="a.id" class="alert border-0 rounded-3 mb-2" :class="a.isResolved ? 'bg-light text-muted' : 'bg-danger bg-opacity-10 text-danger'">
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
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { reportService } from '@/api/reports'
import { patientService } from '@/api/patients'
import StatusBadge from '@/components/StatusBadge.vue'

const loading = ref(false)
const error = ref('')
const reporte = ref(null)
const reporteTitulo = ref('')
const citaId = ref('')
const pacienteId = ref('')
const pacientes = ref([])

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch { return dateStr }
}

onMounted(async () => {
  try {
    const res = await patientService.getAll()
    pacientes.value = (res.data?.value || res.data || []).filter(p => p.fullName)
  } catch {
    pacientes.value = []
  }
})

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
    reporteTitulo.value = `Reporte del Paciente ${pacientes.value.find(p => p.id === pacienteId.value)?.fullName || `#${pacienteId.value}`}`
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
