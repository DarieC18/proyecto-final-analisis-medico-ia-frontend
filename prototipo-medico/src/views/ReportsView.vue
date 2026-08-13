<template>
  <div>
    <PageHeader
      title="Reportes clínicos"
      subtitle="Reportes detallados de pacientes y citas"
      :icon="IconReport"
    />

    <div class="row g-3 mb-3">
      <div class="col-lg-6">
        <BaseCard title="Reporte de paciente" :icon="IconUser" padding="sm">
          <div class="d-flex gap-2">
            <select
              v-model="pacienteId"
              class="form-select"
              aria-label="Seleccionar paciente"
              @change="cargarReportePaciente"
            >
              <option value="">Seleccione un paciente…</option>
              <option v-for="p in pacientes" :key="p.id" :value="p.id">
                {{ p.fullName || `#${p.id}` }}
              </option>
            </select>
            <AppButton variant="primary" :disabled="!pacienteId" @click="cargarReportePaciente">
              Ver
            </AppButton>
          </div>
        </BaseCard>
      </div>

      <div class="col-lg-6">
        <BaseCard title="Reporte de cita" :icon="IconAppointment" icon-tone="info" padding="sm">
          <div class="d-flex gap-2 flex-wrap flex-sm-nowrap">
            <input
              v-model="citaFecha"
              type="date"
              class="form-control"
              aria-label="Fecha de la cita"
              @change="filtrarCitasPorFecha"
            >
            <select
              v-model="citaSeleccionada"
              class="form-select"
              aria-label="Seleccionar cita"
              :disabled="!citasDelDia.length"
            >
              <option value="">{{ placeholderCita }}</option>
              <option v-for="c in citasDelDia" :key="c.id" :value="c.id">
                {{ c.patientName || `Paciente #${c.patientId}` }} — {{ formatHora(c.appointmentDate) }}
              </option>
            </select>
            <AppButton variant="primary" :disabled="!citaSeleccionada" @click="cargarReporteCita">
              Ver
            </AppButton>
          </div>
        </BaseCard>
      </div>
    </div>

    <LoadingState v-if="loading" label="Generando reporte…" />

    <AppAlert v-else-if="error" variant="danger" :message="error" />

    <BaseCard v-else-if="reporte" :title="reporteTitulo" :icon="IconReport" class="u-fade-in">
      <template #header-actions>
        <AppButton variant="ghost" size="sm" :icon="IconClose" aria-label="Cerrar reporte" @click="reporte = null" />
      </template>

      <div class="d-grid gap-4">
        <section v-if="reporte.patientDetail">
          <h3 class="report__heading">Información del paciente</h3>
          <div class="row g-3">
            <div class="col-md-4"><DataField label="Nombre" :value="reporte.patientDetail.fullName" /></div>
            <div class="col-md-4"><DataField label="Identificación" :value="reporte.patientDetail.identificationNumber" /></div>
            <div class="col-md-4"><DataField label="Teléfono" :value="reporte.patientDetail.phoneNumber" /></div>
          </div>
        </section>

        <section v-if="reporte.appointmentDate">
          <h3 class="report__heading">Información de la cita</h3>
          <div class="row g-3">
            <div class="col-md-3"><DataField label="Fecha" :value="formatDate(reporte.appointmentDate)" /></div>
            <div class="col-md-3">
              <DataField label="Estado">
                <StatusBadge :text="translateStatus(reporte.status)" :variant="statusVariant(reporte.status)" />
              </DataField>
            </div>
            <div class="col-md-3"><DataField label="Motivo" :value="reporte.reason" /></div>
            <div class="col-md-3"><DataField label="Notas" :value="reporte.notes" /></div>
          </div>
        </section>

        <section v-if="historial.length">
          <h3 class="report__heading">Historial médico</h3>
          <div class="table-responsive">
            <table class="table table-sm table-hover align-middle mb-0">
              <thead class="table-head">
                <tr><th scope="col">Fecha</th><th scope="col">Diagnóstico</th><th scope="col">Notas</th></tr>
              </thead>
              <tbody>
                <tr v-for="r in historial" :key="r.id">
                  <td>{{ formatDate(r.appointmentDate || r.createdAt) }}</td>
                  <td>{{ r.diagnosisInitial || r.reason || '—' }}</td>
                  <td>{{ r.notes || '—' }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>

        <section v-if="reporte.symptoms?.length">
          <h3 class="report__heading">Síntomas</h3>
          <div class="table-responsive">
            <table class="table table-sm table-hover align-middle mb-0">
              <thead class="table-head">
                <tr><th scope="col">Síntoma</th><th scope="col">Severidad</th><th scope="col">Notas</th></tr>
              </thead>
              <tbody>
                <tr v-for="s in reporte.symptoms" :key="s.id">
                  <td>{{ s.name }}</td>
                  <td><StatusBadge :text="s.severity" :variant="s.severity?.toLowerCase()" size="sm" /></td>
                  <td>{{ s.notes || '—' }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>

        <section v-if="reporte.vitalSigns?.length">
          <h3 class="report__heading">Signos vitales</h3>
          <div class="table-responsive">
            <table class="table table-sm table-hover align-middle mb-0">
              <thead class="table-head">
                <tr>
                  <th scope="col">Temperatura</th>
                  <th scope="col">Frec. cardíaca</th>
                  <th scope="col">Presión</th>
                  <th scope="col">Saturación</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="v in reporte.vitalSigns" :key="v.id">
                  <td>{{ v.temperature }} °C</td>
                  <td>{{ v.heartRate }} lpm</td>
                  <td>{{ v.systolicPressure }}/{{ v.diastolicPressure }}</td>
                  <td>{{ v.oxygenSaturation }} %</td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>

        <section v-if="reporte.alerts?.length">
          <h3 class="report__heading">Alertas</h3>
          <div class="d-grid gap-2">
            <AppAlert
              v-for="a in reporte.alerts"
              :key="a.id"
              :variant="a.isResolved ? 'success' : 'danger'"
              :title="a.alertType"
              :message="a.description"
            />
          </div>
        </section>

        <section v-if="reporte.aiAnalyses?.length">
          <h3 class="report__heading">Análisis de IA</h3>
          <div class="d-grid gap-2">
            <BaseCard v-for="ia in reporte.aiAnalyses" :key="ia.id" tone="muted" padding="sm">
              <DataField label="Resultado">
                <span class="report__ai">{{ ia.aiResponse }}</span>
              </DataField>
            </BaseCard>
          </div>
        </section>
      </div>
    </BaseCard>

    <BaseCard v-else flush padding="none">
      <EmptyState
        :icon="IconReport"
        title="Ningún reporte generado"
        message="Elige un paciente o una cita en los selectores de arriba para generar su reporte."
      />
    </BaseCard>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import {
  AppAlert,
  AppButton,
  BaseCard,
  DataField,
  EmptyState,
  LoadingState,
  PageHeader,
  StatusBadge
} from '@/components/ui'
import { IconAppointment, IconClose, IconReport, IconUser } from '@/lib/icons'
import { reportService } from '@/api/reports'
import { patientService } from '@/api/patients'
import { appointmentService } from '@/api/appointments'
import { statusVariant, translateStatus } from '@/utils/appointmentStatus'

const loading = ref(false)
const error = ref('')
const reporte = ref(null)
const reporteTitulo = ref('')
const pacienteId = ref('')
const pacientes = ref([])
const citas = ref([])
const citaFecha = ref('')
const citaSeleccionada = ref('')

const formatDate = (dateStr) => {
  if (!dateStr) return '—'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch {
    return dateStr
  }
}

const formatHora = (dateStr) => {
  if (!dateStr) return '—'
  try {
    return new Date(dateStr).toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })
  } catch {
    return dateStr
  }
}

// Comparación en hora local: convertir a ISO usaría UTC y desplazaría las citas
// de la franja nocturna al día anterior o siguiente.
const toLocalDateKey = (value) => {
  const d = new Date(value)
  const mm = String(d.getMonth() + 1).padStart(2, '0')
  const dd = String(d.getDate()).padStart(2, '0')
  return `${d.getFullYear()}-${mm}-${dd}`
}

const citasDelDia = computed(() => {
  if (!citaFecha.value) return []
  return citas.value.filter(
    (c) => c.appointmentDate && toLocalDateKey(c.appointmentDate) === citaFecha.value
  )
})

const placeholderCita = computed(() => {
  if (!citaFecha.value) return 'Primero selecciona una fecha'
  return citasDelDia.value.length ? 'Seleccione cita…' : 'Sin citas ese día'
})

const historial = computed(
  () => reporte.value?.patientDetail?.medicalRecords || reporte.value?.medicalRecords || []
)

const filtrarCitasPorFecha = () => {
  citaSeleccionada.value = ''
  reporte.value = null
}

onMounted(async () => {
  try {
    const [resP, resC] = await Promise.all([patientService.getAll(), appointmentService.getAll()])
    pacientes.value = (resP.data?.value || resP.data || []).filter((p) => p.fullName)
    citas.value = resC.data || []
  } catch {
    pacientes.value = []
    citas.value = []
  }
})

const cargarReporteCita = async () => {
  if (!citaSeleccionada.value) return
  loading.value = true
  error.value = ''
  reporte.value = null
  try {
    const res = await reportService.getAppointmentReport(citaSeleccionada.value)
    reporte.value = res.data
    const cita = citasDelDia.value.find((c) => c.id === citaSeleccionada.value)
    reporteTitulo.value = `Cita de ${cita?.patientName || 'paciente'} — ${formatDate(cita?.appointmentDate)}`
  } catch (err) {
    error.value = err.response?.status === 404 ? 'Cita no encontrada.' : 'Error al cargar el reporte de la cita'
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
    const nombre = pacientes.value.find((p) => p.id === pacienteId.value)?.fullName
    reporteTitulo.value = `Reporte de ${nombre || `paciente #${pacienteId.value}`}`
  } catch (err) {
    error.value =
      err.response?.status === 404 ? 'Paciente no encontrado.' : 'Error al cargar el reporte del paciente'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.report__heading {
  font-size: 0.72rem;
  font-weight: 600;
  letter-spacing: 0.06em;
  text-transform: uppercase;
  color: var(--app-text-muted);
  margin-bottom: 0.75rem;
  padding-bottom: 0.4rem;
  border-bottom: 1px solid var(--app-border);
}

.report__ai {
  white-space: pre-wrap;
  font-weight: 400;
  color: var(--app-text);
}
</style>
