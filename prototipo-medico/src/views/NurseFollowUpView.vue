<template>
  <div>
    <PageHeader
      title="Seguimiento de Pacientes"
      subtitle="Registro de síntomas y signos vitales fuera de cita"
      :icon="IconClinical"
    />

    <!-- ============ Buscador ============ -->
    <BaseCard title="Buscar paciente" :icon="IconSearch" class="mb-3">
      <div class="row g-2 align-items-end">
        <div class="col-md-7">
          <label for="n-busqueda" class="form-label">Nombre, identificación o teléfono</label>
          <input
            id="n-busqueda"
            v-model="busqueda"
            type="text"
            class="form-control"
            placeholder="Ej: María González o 0801199012345"
            @keyup.enter="buscarPaciente"
          />
        </div>
        <div class="col-md-3">
          <AppButton variant="primary" block :icon="IconSearch" :loading="buscando" @click="buscarPaciente">
            Buscar
          </AppButton>
        </div>
        <div class="col-md-2">
          <AppButton v-if="busqueda" variant="soft" block @click="limpiar">Limpiar</AppButton>
        </div>
      </div>

      <ul v-if="resultados.length" class="resultados">
        <li v-for="p in resultados" :key="p.id">
          <button
            type="button"
            class="resultados__item"
            :class="{ 'resultados__item--sel': pacienteSeleccionado?.id === p.id }"
            @click="seleccionarPaciente(p)"
          >
            <span class="fw-medium">{{ p.fullName }}</span>
            <small class="text-app-muted">{{ p.identificationNumber }}</small>
          </button>
        </li>
      </ul>

      <p v-else-if="buscado && !buscando" class="text-app-muted small mb-0 mt-2">
        No se encontraron pacientes.
      </p>
    </BaseCard>

    <!-- ============ Panel del paciente ============ -->
    <BaseCard v-if="!pacienteSeleccionado" flush padding="none">
      <EmptyState
        :icon="IconPatients"
        title="Ningún paciente seleccionado"
        message="Busca un paciente para registrar síntomas o signos vitales de seguimiento."
      />
    </BaseCard>

    <div v-else class="u-fade-in">
      <AppAlert variant="brand" class="mb-3">
        <strong>{{ pacienteSeleccionado.fullName }}</strong>
        <span class="ms-2">{{ pacienteSeleccionado.identificationNumber }}</span>
      </AppAlert>

      <BaseCard flush padding="none" class="mb-3">
        <TabNav v-model="tab" :tabs="TABS" />
      </BaseCard>

      <!-- ---- Síntomas ---- -->
      <template v-if="tab === 'sintomas'">
        <BaseCard title="Registrar síntoma" :icon="IconSymptoms" class="mb-3">
          <AppAlert v-if="sintomaError" variant="danger" :message="sintomaError" class="mb-3" />

          <form @submit.prevent="guardarSintoma">
            <div class="row g-3">
              <div class="col-md-4">
                <label for="n-sintoma" class="form-label">Síntoma</label>
                <input id="n-sintoma" v-model="sintomaForm.name" type="text" class="form-control" placeholder="Ej: Fiebre" required />
              </div>
              <div class="col-md-3">
                <label for="n-severidad" class="form-label">Severidad</label>
                <select id="n-severidad" v-model="sintomaForm.severity" class="form-select" required>
                  <option value="Leve">Leve</option>
                  <option value="Moderado">Moderado</option>
                  <option value="Severo">Severo</option>
                </select>
              </div>
              <div class="col-md-3">
                <label for="n-inicio" class="form-label">Inicio</label>
                <input id="n-inicio" v-model="sintomaForm.startedAt" type="date" class="form-control" required />
              </div>
              <div class="col-md-2 d-flex align-items-end">
                <AppButton type="submit" variant="primary" block :icon="IconAdd" :loading="guardandoSintoma">
                  Agregar
                </AppButton>
              </div>
              <div class="col-12">
                <label for="n-notas" class="form-label">Notas</label>
                <textarea id="n-notas" v-model="sintomaForm.notes" class="form-control" rows="2" placeholder="Notas adicionales…" />
              </div>
            </div>
          </form>
        </BaseCard>

        <BaseCard title="Historial de síntomas" :icon="IconHistory">
          <LoadingState v-if="cargandoSintomas" label="Cargando síntomas…" />
          <EmptyState
            v-else-if="sintomas.length === 0"
            size="sm"
            :icon="IconSymptoms"
            title="Sin síntomas de seguimiento"
          />
          <div v-else class="row g-3">
            <div v-for="s in sintomas" :key="s.id" class="col-md-6">
              <div class="sintoma" :class="`sintoma--${severityVariant(s.severity)}`">
                <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
                  <h4 class="sintoma__nombre">{{ s.name }}</h4>
                  <StatusBadge :text="s.severity" :variant="severityVariant(s.severity)" />
                </div>
                <DataField label="Inicio" :value="formatDate(s.startedAt)" />
                <p class="sintoma__notas">{{ s.notes || 'Sin notas.' }}</p>
              </div>
            </div>
          </div>
        </BaseCard>
      </template>

      <!-- ---- Signos vitales ---- -->
      <template v-else>
        <BaseCard title="Registrar signos vitales" :icon="IconVitals" class="mb-3">
          <AppAlert v-if="signosError" variant="danger" :message="signosError" class="mb-3" />

          <form @submit.prevent="guardarSignos">
            <div class="row g-3">
              <div v-for="campo in CAMPOS_SIGNOS" :key="campo.key" class="col-md-4">
                <label :for="`n-${campo.key}`" class="form-label d-flex align-items-center gap-2">
                  <Icon :icon="campo.icon" :size="15" tone="muted" />
                  {{ campo.label }}
                </label>
                <input
                  :id="`n-${campo.key}`"
                  v-model.number="signosForm[campo.key]"
                  type="number"
                  :step="campo.step ?? 1"
                  class="form-control"
                  :placeholder="campo.placeholder"
                />
              </div>
              <div class="col-12 d-flex justify-content-end">
                <AppButton type="submit" variant="primary" :icon="IconSave" :loading="guardandoSignos">
                  Registrar medición
                </AppButton>
              </div>
            </div>
          </form>
        </BaseCard>

        <BaseCard title="Historial de signos vitales" :icon="IconHistory" flush padding="none">
          <DataTable
            :columns="COLUMNAS_SIGNOS"
            :rows="signos"
            :loading="cargandoSignos"
            loading-label="Cargando mediciones…"
            :empty-icon="IconVitals"
            empty-title="Sin mediciones de seguimiento"
            dense
          >
            <template #cell-measuredAt="{ value }">{{ formatDateTime(value) }}</template>
            <template #cell-temperature="{ value }">{{ value != null ? `${value} °C` : '—' }}</template>
            <template #cell-heartRate="{ value }">{{ value != null ? `${value} lpm` : '—' }}</template>
            <template #cell-presion="{ row }">
              {{ row.systolicPressure != null && row.diastolicPressure != null
                ? `${row.systolicPressure}/${row.diastolicPressure}`
                : '—' }}
            </template>
            <template #cell-oxygenSaturation="{ value }">{{ value != null ? `${value} %` : '—' }}</template>
            <template #cell-glucose="{ value }">{{ value != null ? `${value} mg/dL` : '—' }}</template>
          </DataTable>
        </BaseCard>
      </template>
    </div>
  </div>
</template>

<script setup>
import { markRaw, reactive, ref, watch } from 'vue'
import { patientService } from '@/api/patients'
import { symptomService } from '@/api/symptoms'
import { vitalSignService } from '@/api/vitalSigns'
import {
  AppAlert,
  AppButton,
  BaseCard,
  DataField,
  DataTable,
  EmptyState,
  Icon,
  LoadingState,
  PageHeader,
  StatusBadge,
  TabNav
} from '@/components/ui'
import {
  IconAdd,
  IconBlood,
  IconClinical,
  IconHeartRate,
  IconHistory,
  IconOxygen,
  IconPatients,
  IconSave,
  IconSearch,
  IconSymptoms,
  IconTemperature,
  IconVitals
} from '@/lib/icons'

const TABS = [
  { id: 'sintomas', label: 'Síntomas', icon: markRaw(IconSymptoms) },
  { id: 'signos', label: 'Signos Vitales', icon: markRaw(IconVitals) }
]

// Los seis campos de medición se declaran una vez en lugar de repetir el mismo
// bloque label+input con distinto emoji, como estaba antes.
const CAMPOS_SIGNOS = [
  { key: 'temperature', label: 'Temperatura (°C)', icon: markRaw(IconTemperature), step: 0.1, placeholder: '36.5' },
  { key: 'heartRate', label: 'Frec. cardíaca (lpm)', icon: markRaw(IconHeartRate), placeholder: '72' },
  { key: 'systolicPressure', label: 'Presión sistólica', icon: markRaw(IconBlood), placeholder: '120' },
  { key: 'diastolicPressure', label: 'Presión diastólica', icon: markRaw(IconBlood), placeholder: '80' },
  { key: 'oxygenSaturation', label: 'Saturación O₂ (%)', icon: markRaw(IconOxygen), step: 0.1, placeholder: '98' },
  { key: 'glucose', label: 'Glucosa (mg/dL)', icon: markRaw(IconBlood), step: 0.1, placeholder: '90' }
]

const COLUMNAS_SIGNOS = [
  { key: 'measuredAt', label: 'Fecha' },
  { key: 'temperature', label: 'Temp.' },
  { key: 'heartRate', label: 'FC' },
  { key: 'presion', label: 'PA' },
  { key: 'oxygenSaturation', label: 'O₂' },
  { key: 'glucose', label: 'Glucosa' }
]

const busqueda = ref('')
const buscando = ref(false)
const buscado = ref(false)
const resultados = ref([])
const pacienteSeleccionado = ref(null)
const tab = ref('sintomas')

const sintomas = ref([])
const signos = ref([])
const cargandoSintomas = ref(false)
const cargandoSignos = ref(false)

const sintomaError = ref('')
const signosError = ref('')
const guardandoSintoma = ref(false)
const guardandoSignos = ref(false)

const sintomaForm = reactive({ name: '', severity: 'Moderado', startedAt: '', notes: '' })
const signosForm = reactive({
  temperature: null,
  heartRate: null,
  systolicPressure: null,
  diastolicPressure: null,
  oxygenSaturation: null,
  glucose: null
})

const SEVERITY_VARIANTS = { Leve: 'low', Moderado: 'moderate', Severo: 'severe' }
const severityVariant = (s) => SEVERITY_VARIANTS[s] ?? 'secondary'

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' })
  } catch {
    return dateStr
  }
}

const formatDateTime = (dateStr) => {
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

const buscarPaciente = async () => {
  if (!busqueda.value.trim()) return
  buscando.value = true
  buscado.value = false
  try {
    const res = await patientService.search(busqueda.value)
    resultados.value = res.data || []
  } catch {
    resultados.value = []
  } finally {
    buscado.value = true
    buscando.value = false
  }
}

const cargarSintomas = async () => {
  cargandoSintomas.value = true
  try {
    const res = await symptomService.getByPatient(pacienteSeleccionado.value.id)
    sintomas.value = res.data || []
  } catch {
    sintomas.value = []
  } finally {
    cargandoSintomas.value = false
  }
}

const cargarSignos = async () => {
  cargandoSignos.value = true
  try {
    const res = await vitalSignService.getByPatient(pacienteSeleccionado.value.id)
    signos.value = res.data || []
  } catch {
    signos.value = []
  } finally {
    cargandoSignos.value = false
  }
}

const seleccionarPaciente = async (p) => {
  pacienteSeleccionado.value = p
  await Promise.all([cargarSintomas(), cargarSignos()])
}

const limpiar = () => {
  busqueda.value = ''
  resultados.value = []
  buscado.value = false
  pacienteSeleccionado.value = null
  sintomas.value = []
  signos.value = []
}

const guardarSintoma = async () => {
  sintomaError.value = ''
  guardandoSintoma.value = true
  try {
    await symptomService.create({
      patientId: pacienteSeleccionado.value.id,
      name: sintomaForm.name,
      severity: sintomaForm.severity,
      startedAt: sintomaForm.startedAt,
      notes: sintomaForm.notes || null
    })
    sintomaForm.name = ''
    sintomaForm.severity = 'Moderado'
    sintomaForm.startedAt = ''
    sintomaForm.notes = ''
    await cargarSintomas()
  } catch (err) {
    sintomaError.value = err.response?.data?.message || 'Error al guardar el síntoma.'
  } finally {
    guardandoSintoma.value = false
  }
}

const guardarSignos = async () => {
  signosError.value = ''
  guardandoSignos.value = true
  try {
    await vitalSignService.createFollowUp({
      patientId: pacienteSeleccionado.value.id,
      temperature: signosForm.temperature || null,
      heartRate: signosForm.heartRate || null,
      systolicPressure: signosForm.systolicPressure || null,
      diastolicPressure: signosForm.diastolicPressure || null,
      oxygenSaturation: signosForm.oxygenSaturation || null,
      glucose: signosForm.glucose || null
    })
    Object.keys(signosForm).forEach((k) => (signosForm[k] = null))
    await cargarSignos()
  } catch (err) {
    signosError.value = err.response?.data?.message || 'Error al guardar los signos vitales.'
  } finally {
    guardandoSignos.value = false
  }
}

watch(tab, (t) => {
  if (!pacienteSeleccionado.value) return
  if (t === 'sintomas') cargarSintomas()
  else cargarSignos()
})
</script>

<style scoped>
.resultados {
  list-style: none;
  margin: 0.75rem 0 0;
  padding: 0;
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius);
  overflow: hidden;
}

.resultados li + li {
  border-top: 1px solid var(--app-border);
}

.resultados__item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.75rem;
  width: 100%;
  padding: 0.5rem 0.875rem;
  border: 0;
  background: transparent;
  color: inherit;
  text-align: start;
  cursor: pointer;
}

.resultados__item:hover {
  background-color: var(--app-surface-sunken);
}

.resultados__item--sel {
  background-color: var(--bs-primary-bg-subtle);
  color: var(--bs-primary-text-emphasis);
}

.resultados__item:focus-visible {
  outline: none;
  box-shadow: var(--app-ring);
}

.sintoma {
  height: 100%;
  padding: 0.875rem 1rem;
  background-color: var(--app-surface-sunken);
  border: 1px solid var(--app-border);
  border-inline-start: 3px solid var(--app-border-strong);
  border-radius: var(--app-radius-lg);
}

.sintoma--low {
  border-inline-start-color: var(--c-success-500);
}
.sintoma--moderate {
  border-inline-start-color: var(--c-warning-500);
}
.sintoma--severe {
  border-inline-start-color: var(--c-danger-500);
}

.sintoma__nombre {
  margin: 0;
  font-size: 0.9375rem;
  font-weight: 600;
  color: var(--app-text-strong);
}

.sintoma__notas {
  margin: 0.5rem 0 0;
  font-size: 0.85rem;
  color: var(--app-text-muted);
}
</style>
