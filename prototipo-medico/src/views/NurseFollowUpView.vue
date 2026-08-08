<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Seguimiento de Pacientes</h3>
        <p class="text-muted">Registro de síntomas y signos vitales fuera de cita</p>
      </div>
    </div>

    <!-- Buscador de pacientes -->
    <div class="card shadow-sm border-0 mb-4">
      <div class="card-body p-4">
        <h6 class="fw-bold mb-3">Buscar Paciente</h6>
        <div class="d-flex gap-3">
          <input
            v-model="busqueda"
            type="text"
            class="form-control"
            placeholder="Nombre, identificación o teléfono..."
            @keyup.enter="buscarPaciente"
          >
          <button @click="buscarPaciente" class="btn btn-dark px-4" :disabled="buscando">
            <span v-if="buscando" class="spinner-border spinner-border-sm me-1"></span>
            Buscar
          </button>
          <button v-if="busqueda" @click="limpiar" class="btn btn-light border px-3">Limpiar</button>
        </div>
        <div v-if="resultados.length" class="mt-3">
          <div
            v-for="p in resultados"
            :key="p.id"
            @click="seleccionarPaciente(p)"
            class="resultado-item p-3 border rounded-3 mb-2 cursor-pointer"
            :class="{ 'border-primary bg-primary bg-opacity-10': pacienteSeleccionado?.id === p.id }"
          >
            <strong>{{ p.fullName }}</strong>
            <span class="text-muted ms-2">{{ p.identificationNumber }}</span>
          </div>
        </div>
        <p v-else-if="buscado && !resultados.length" class="text-muted mt-3 mb-0">No se encontraron pacientes.</p>
      </div>
    </div>

    <!-- Panel del paciente seleccionado -->
    <div v-if="pacienteSeleccionado" class="animation-fade">
      <div class="alert alert-success border-0 rounded-3 mb-4">
        <strong>Paciente:</strong> {{ pacienteSeleccionado.fullName }}
        <span class="ms-3 text-muted">{{ pacienteSeleccionado.identificationNumber }}</span>
      </div>

      <!-- Tabs -->
      <div class="card shadow-sm p-2 mb-4">
        <ul class="nav nav-pills nav-fill gap-2 p-1">
          <li class="nav-item">
            <a class="nav-link rounded-pill fw-medium" :class="tab === 'sintomas' ? 'active bg-primary shadow-sm' : 'text-muted'" @click="tab = 'sintomas'" href="#">🤒 Síntomas</a>
          </li>
          <li class="nav-item">
            <a class="nav-link rounded-pill fw-medium" :class="tab === 'signos' ? 'active bg-primary shadow-sm' : 'text-muted'" @click="tab = 'signos'" href="#">❤️ Signos Vitales</a>
          </li>
        </ul>
      </div>

      <!-- TAB SÍNTOMAS -->
      <div v-if="tab === 'sintomas'" class="animation-fade">
        <div class="card shadow-sm border-0 mb-4">
          <div class="card-body p-4">
            <h6 class="fw-bold mb-3">Registrar Síntoma</h6>
            <div v-if="sintomaError" class="alert alert-danger border-0 py-2 small">{{ sintomaError }}</div>
            <form @submit.prevent="guardarSintoma">
              <div class="row g-3">
                <div class="col-md-4">
                  <label class="form-label fw-medium">Síntoma</label>
                  <input v-model="sintomaForm.name" type="text" class="form-control" placeholder="Ej: Fiebre" required>
                </div>
                <div class="col-md-3">
                  <label class="form-label fw-medium">Severidad</label>
                  <select v-model="sintomaForm.severity" class="form-select" required>
                    <option value="Leve">Leve</option>
                    <option value="Moderado">Moderado</option>
                    <option value="Severo">Severo</option>
                  </select>
                </div>
                <div class="col-md-3">
                  <label class="form-label fw-medium">Inicio</label>
                  <input v-model="sintomaForm.startedAt" type="date" class="form-control" required>
                </div>
                <div class="col-md-2 d-flex align-items-end">
                  <button type="submit" class="btn btn-success w-100 rounded-pill" :disabled="guardandoSintoma">
                    <span v-if="guardandoSintoma" class="spinner-border spinner-border-sm"></span>
                    <span v-else>+ Agregar</span>
                  </button>
                </div>
              </div>
              <div class="mt-3">
                <label class="form-label fw-medium">Notas</label>
                <textarea v-model="sintomaForm.notes" class="form-control" rows="2" placeholder="Notas adicionales..."></textarea>
              </div>
            </form>
          </div>
        </div>

        <!-- Historial de síntomas -->
        <div class="card shadow-sm border-0">
          <div class="card-body p-4">
            <h6 class="fw-bold mb-3">Historial de Síntomas</h6>
            <div v-if="cargandoSintomas" class="text-center py-3"><div class="spinner-border text-primary" role="status"></div></div>
            <div v-else-if="sintomas.length === 0" class="text-muted text-center py-4">No hay síntomas de seguimiento registrados.</div>
            <div v-else class="row g-3">
              <div class="col-md-6" v-for="s in sintomas" :key="s.id">
                <div class="card bg-light border-0 border-start border-4 shadow-sm h-100 p-3 rounded-4" :class="`border-${severityColor(s.severity)}`">
                  <div class="d-flex justify-content-between align-items-start mb-2">
                    <h6 class="fw-bold mb-0">{{ s.name }}</h6>
                    <span class="badge rounded-pill px-3" :class="severityBadge(s.severity)">{{ s.severity }}</span>
                  </div>
                  <p class="text-muted small mb-1"><strong>Inicio:</strong> {{ s.startedAt }}</p>
                  <p class="text-dark small mb-0">{{ s.notes || 'Sin notas.' }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- TAB SIGNOS VITALES -->
      <div v-if="tab === 'signos'" class="animation-fade">
        <div class="card shadow-sm border-0 mb-4">
          <div class="card-body p-4">
            <h6 class="fw-bold mb-3">Registrar Signos Vitales</h6>
            <div v-if="signosError" class="alert alert-danger border-0 py-2 small">{{ signosError }}</div>
            <form @submit.prevent="guardarSignos">
              <div class="row g-3">
                <div class="col-md-4">
                  <label class="form-label fw-medium">🌡️ Temperatura (°C)</label>
                  <input v-model.number="signosForm.temperature" type="number" step="0.1" class="form-control" placeholder="36.5">
                </div>
                <div class="col-md-4">
                  <label class="form-label fw-medium">💓 Frec. Cardíaca (lpm)</label>
                  <input v-model.number="signosForm.heartRate" type="number" class="form-control" placeholder="72">
                </div>
                <div class="col-md-4">
                  <label class="form-label fw-medium">🩸 Presión Sistólica</label>
                  <input v-model.number="signosForm.systolicPressure" type="number" class="form-control" placeholder="120">
                </div>
                <div class="col-md-4">
                  <label class="form-label fw-medium">🩸 Presión Diastólica</label>
                  <input v-model.number="signosForm.diastolicPressure" type="number" class="form-control" placeholder="80">
                </div>
                <div class="col-md-4">
                  <label class="form-label fw-medium">💨 O₂ Saturación (%)</label>
                  <input v-model.number="signosForm.oxygenSaturation" type="number" step="0.1" class="form-control" placeholder="98">
                </div>
                <div class="col-md-4">
                  <label class="form-label fw-medium">🩻 Glucosa (mg/dL)</label>
                  <input v-model.number="signosForm.glucose" type="number" step="0.1" class="form-control" placeholder="90">
                </div>
                <div class="col-12 d-flex justify-content-end">
                  <button type="submit" class="btn btn-success rounded-pill px-5" :disabled="guardandoSignos">
                    <span v-if="guardandoSignos" class="spinner-border spinner-border-sm me-2"></span>
                    {{ guardandoSignos ? 'Guardando...' : 'Registrar Medición' }}
                  </button>
                </div>
              </div>
            </form>
          </div>
        </div>

        <!-- Historial de signos -->
        <div class="card shadow-sm border-0">
          <div class="card-body p-4">
            <h6 class="fw-bold mb-3">Historial de Signos Vitales</h6>
            <div v-if="cargandoSignos" class="text-center py-3"><div class="spinner-border text-primary" role="status"></div></div>
            <div v-else-if="signos.length === 0" class="text-muted text-center py-4">No hay signos vitales de seguimiento registrados.</div>
            <div v-else class="table-responsive">
              <table class="table table-sm table-hover align-middle mb-0">
                <thead class="bg-light text-muted">
                  <tr>
                    <th class="ps-3 py-2 fw-medium">Fecha</th>
                    <th class="py-2 fw-medium">Temp.</th>
                    <th class="py-2 fw-medium">FC</th>
                    <th class="py-2 fw-medium">PA</th>
                    <th class="py-2 fw-medium">O₂</th>
                    <th class="pe-3 py-2 fw-medium">Glucosa</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="v in signos" :key="v.id">
                    <td class="ps-3 py-2 text-muted small">{{ formatDateTime(v.measuredAt) }}</td>
                    <td class="py-2">{{ v.temperature != null ? v.temperature + '°C' : '-' }}</td>
                    <td class="py-2">{{ v.heartRate != null ? v.heartRate + ' lpm' : '-' }}</td>
                    <td class="py-2">{{ v.systolicPressure != null && v.diastolicPressure != null ? `${v.systolicPressure}/${v.diastolicPressure}` : '-' }}</td>
                    <td class="py-2">{{ v.oxygenSaturation != null ? v.oxygenSaturation + '%' : '-' }}</td>
                    <td class="pe-3 py-2">{{ v.glucose != null ? v.glucose + ' mg/dL' : '-' }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, watch } from 'vue'
import { patientService } from '@/api/patients'
import { symptomService } from '@/api/symptoms'
import { vitalSignService } from '@/api/vitalSigns'

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
  temperature: null, heartRate: null, systolicPressure: null,
  diastolicPressure: null, oxygenSaturation: null, glucose: null
})

const buscarPaciente = async () => {
  if (!busqueda.value.trim()) return
  buscando.value = true
  buscado.value = false
  try {
    const res = await patientService.search(busqueda.value)
    resultados.value = res.data || []
    buscado.value = true
  } catch {
    resultados.value = []
    buscado.value = true
  } finally {
    buscando.value = false
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
    Object.keys(signosForm).forEach(k => signosForm[k] = null)
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
  else if (t === 'signos') cargarSignos()
})

const severityColor = (s) => ({ Leve: 'success', Moderado: 'warning', Severo: 'danger' }[s] || 'secondary')
const severityBadge = (s) => ({ Leve: 'bg-success bg-opacity-75', Moderado: 'bg-warning text-dark', Severo: 'bg-danger' }[s] || 'bg-secondary')

const formatDateTime = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch {
    return dateStr
  }
}
</script>

<style scoped>
.animation-fade { animation: fadeIn 0.3s ease-in-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(8px); } to { opacity: 1; transform: translateY(0); } }

.resultado-item {
  cursor: pointer;
  transition: background-color 0.15s;
}
.resultado-item:hover {
  background-color: #f0f9ff;
}
</style>
