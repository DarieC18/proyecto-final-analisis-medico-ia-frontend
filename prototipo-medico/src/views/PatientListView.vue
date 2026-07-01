<template>
  <div class="container">

    <div v-if="vistaActual === 'lista'" class="animation-fade">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="fw-bold mb-0">Gestión de Pacientes</h3>
          <p class="text-muted">Directorio general de pacientes registrados</p>
        </div>
        <button @click="vistaActual = 'crear'" class="btn btn-primary px-4 shadow-sm">+ Crear Paciente</button>
      </div>

      <div class="card shadow-sm mb-4 border-0">
        <div class="card-body p-3">
          <div class="d-flex gap-3">
            <div class="input-group">
              <span class="input-group-text bg-light border-0">🔍</span>
              <input v-model="busqueda" type="text" class="form-control bg-light border-0" placeholder="Buscar por nombre completo o identificación...">
            </div>
          </div>
        </div>
      </div>

      <div v-if="loading" class="text-center py-5">
        <div class="spinner-border text-primary" role="status"></div>
      </div>

      <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

      <div v-else-if="pacientesFiltrados.length === 0" class="text-center py-5 text-muted">
        <p>No se encontraron pacientes.</p>
      </div>
      <div v-else class="card shadow-sm overflow-hidden border-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="bg-light text-muted">
              <tr>
                <th class="ps-4 py-3 fw-medium">Nombre Completo</th>
                <th class="py-3 fw-medium">Identificación</th>
                <th class="py-3 fw-medium">F. Nacimiento</th>
                <th class="py-3 fw-medium">Teléfono</th>
                <th class="py-3 fw-medium">Género</th>
                <th class="py-3 fw-medium">Registro</th>
                <th class="pe-4 py-3 fw-medium text-end">Acciones</th>
              </tr>
            </thead>
            <tbody class="border-top-0">
              <tr v-for="p in pacientesFiltrados" :key="p.id">
                <td class="ps-4 py-3 fw-bold text-dark">{{ p.fullName }}</td>
                <td class="py-3 text-muted">{{ p.identificationNumber }}</td>
                <td class="py-3 text-muted">{{ formatDate(p.birthDate) }}</td>
                <td class="py-3 text-muted">{{ p.phoneNumber }}</td>
                <td class="py-3"><span class="badge bg-secondary bg-opacity-10 text-secondary rounded-pill px-3 py-2">{{ p.gender }}</span></td>
                <td class="py-3 text-muted">{{ formatDate(p.createdAt) }}</td>
                <td class="pe-4 py-3 text-end">
                  <button @click="abrirDetalle(p)" class="btn btn-sm btn-light border text-info fw-medium px-3 me-2">Ver</button>
                  <button @click="editarPaciente(p)" class="btn btn-sm btn-light border text-warning fw-medium px-3 me-2">Editar</button>
                  <button @click="confirmarEliminar(p)" class="btn btn-sm btn-light border text-danger fw-medium px-3">Eliminar</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <div v-else-if="vistaActual === 'crear' || vistaActual === 'editar'" class="animation-fade">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="fw-bold mb-0">{{ vistaActual === 'crear' ? 'Nuevo Paciente' : 'Editar Paciente' }}</h3>
          <p v-if="vistaActual === 'editar'" class="text-muted">Modificando datos de: <strong class="text-dark">{{ form.fullName }}</strong></p>
        </div>
        <button @click="cancelarForm" class="btn btn-light border text-muted shadow-sm">Volver al listado</button>
      </div>

      <div class="card shadow-sm border-0 rounded-4">
        <div class="card-body p-5">
          <div v-if="formError" class="alert alert-danger border-0 rounded-3 py-2 small mb-4">{{ formError }}</div>
          <form @submit.prevent="guardarPaciente">
            <div class="row g-4">
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Nombre Completo</label>
                <input v-model="form.fullName" type="text" class="form-control bg-light border-0 py-2" required>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Identificación</label>
                <input v-model="form.identificationNumber" type="text" class="form-control bg-light border-0 py-2" required>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Tipo ID</label>
                <select v-model="form.identificationType" class="form-select bg-light border-0 py-2" required>
                  <option value="Cédula">Cédula</option>
                  <option value="Pasaporte">Pasaporte</option>
                </select>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Fecha de Nacimiento</label>
                <input v-model="form.birthDate" type="date" class="form-control bg-light border-0 py-2" required>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Género</label>
                <select v-model="form.gender" class="form-select bg-light border-0 py-2" required>
                  <option value="Masculino">Masculino</option>
                  <option value="Femenino">Femenino</option>
                  <option value="Otro">Otro</option>
                </select>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Teléfono</label>
                <input v-model="form.phoneNumber" type="text" class="form-control bg-light border-0 py-2" required>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Tipo Paciente</label>
                <select v-model="form.patientType" class="form-select bg-light border-0 py-2" required>
                  <option value="General">General</option>
                  <option value="Crónico">Crónico</option>
                  <option value="Emergencia">Emergencia</option>
                </select>
              </div>
            </div>
            <div class="d-flex justify-content-end gap-3 mt-5">
              <button type="button" @click="cancelarForm" class="btn btn-light border px-4 py-2">Cancelar</button>
              <button type="submit" class="btn btn-primary px-4 py-2 shadow-sm" :disabled="saving">
                <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                {{ saving ? 'Guardando...' : 'Guardar Cambios' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <div v-else-if="vistaActual === 'detalle'" class="animation-fade">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="fw-bold mb-0">{{ detalle?.fullName }}</h3>
          <p class="text-muted">Expediente completo del paciente</p>
        </div>
        <button @click="cancelarForm" class="btn btn-light border text-muted shadow-sm">Volver al listado</button>
      </div>
      <div class="card shadow-sm border-0 rounded-4">
        <div class="card-body p-5">
          <div class="row g-4 mb-4">
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Identificación</label>
              <p class="fw-medium">{{ detalle?.identificationNumber }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Teléfono</label>
              <p class="fw-medium">{{ detalle?.phoneNumber }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Género</label>
              <p class="fw-medium">{{ detalle?.gender }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Fecha Nacimiento</label>
              <p class="fw-medium">{{ formatDate(detalle?.birthDate) }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Tipo Paciente</label>
              <p class="fw-medium">{{ detalle?.patientType }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Registrado</label>
              <p class="fw-medium">{{ formatDate(detalle?.createdAt) }}</p>
            </div>
          </div>

          <h5 class="fw-bold mt-5 mb-3">Historial de Citas</h5>
          <div v-if="detalle?.appointments?.length" class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="bg-light text-muted">
                <tr>
                  <th class="ps-4 py-3 fw-medium">Fecha</th>
                  <th class="py-3 fw-medium">Médico</th>
                  <th class="py-3 fw-medium">Motivo</th>
                  <th class="py-3 fw-medium">Estado</th>
                </tr>
              </thead>
              <tbody class="border-top-0">
                <tr v-for="r in detalle.appointments" :key="r.id">
                  <td class="ps-4 py-3">{{ formatDate(r.appointmentDate) }}</td>
                  <td class="py-3">{{ r.doctorName }}</td>
                  <td class="py-3">{{ r.reason }}</td>
                  <td class="py-3">
                    <StatusBadge :text="r.status" :variant="r.status?.toLowerCase()" />
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div v-else class="text-muted text-center py-4">No hay citas registradas para este paciente.</div>
        </div>
      </div>
    </div>

    <ConfirmDialog
      :visible="deleteDialog"
      title="Eliminar Paciente"
      message="¿Está seguro que desea eliminar este paciente y todo su historial clínico? Esta acción no se puede deshacer."
      confirmText="Eliminar"
      :danger="true"
      @confirm="eliminarPaciente"
      @cancel="deleteDialog = false"
    />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { patientService } from '@/api/patients'
import ConfirmDialog from '@/components/ConfirmDialog.vue'
import StatusBadge from '@/components/StatusBadge.vue'

const busqueda = ref('')
const vistaActual = ref('lista')
const loading = ref(true)
const error = ref('')
const saving = ref(false)
const formError = ref('')
const deleteDialog = ref(false)
const deleteTarget = ref(null)
const pacientes = ref([])
const detalle = ref(null)
const editId = ref(null)

const form = reactive({
  fullName: '', identificationNumber: '', identificationType: 'Cédula',
  birthDate: '', gender: 'Masculino', phoneNumber: '', patientType: 'General'
})

const pacientesFiltrados = computed(() => {
  if (!busqueda.value) return pacientes.value
  const q = busqueda.value.toLowerCase()
  return pacientes.value.filter(p =>
    p.fullName?.toLowerCase().includes(q) || p.identificationNumber?.includes(q)
  )
})

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' })
  } catch {
    return dateStr
  }
}

const cargarPacientes = async () => {
  loading.value = true
  error.value = ''
  try {
    const res = await patientService.getAll()
    pacientes.value = res.data || []
  } catch (err) {
    if (err.response?.status === 404 || err.response?.status === 204) {
      pacientes.value = []
    } else {
      error.value = 'Error al cargar pacientes'
    }
  } finally {
    loading.value = false
  }
}

const abrirDetalle = async (p) => {
  vistaActual.value = 'detalle'
  try {
    const res = await patientService.getDetails(p.id)
    detalle.value = res.data || p
  } catch {
    error.value = 'Error al cargar detalle del paciente'
  }
}

const editarPaciente = (p) => {
  editId.value = p.id
  Object.assign(form, {
    fullName: p.fullName || '',
    identificationNumber: p.identificationNumber || '',
    identificationType: p.identificationType || 'Cédula',
    birthDate: p.birthDate ? p.birthDate.substring(0, 10) : '',
    gender: p.gender || 'Masculino',
    phoneNumber: p.phoneNumber || '',
    patientType: p.patientType || 'General'
  })
  vistaActual.value = 'editar'
  formError.value = ''
}

const guardarPaciente = async () => {
  saving.value = true
  formError.value = ''
  try {
    if (vistaActual.value === 'crear') {
      await patientService.create({ ...form })
    } else {
      await patientService.update(editId.value, { ...form })
    }
    vistaActual.value = 'lista'
    await cargarPacientes()
  } catch (err) {
    const data = err.response?.data
    formError.value = data?.errors?.join(', ') || data?.message || 'Error al guardar paciente'
  } finally {
    saving.value = false
  }
}

const cancelarForm = () => {
  vistaActual.value = 'lista'
  detalle.value = null
  editId.value = null
  Object.assign(form, {
    fullName: '', identificationNumber: '', identificationType: 'Cédula',
    birthDate: '', gender: 'Masculino', phoneNumber: '', patientType: 'General'
  })
}

const confirmarEliminar = (p) => {
  deleteTarget.value = p
  deleteDialog.value = true
}

const eliminarPaciente = async () => {
  deleteDialog.value = false
  try {
    await patientService.remove(deleteTarget.value.id)
    await cargarPacientes()
  } catch (err) {
    error.value = 'Error al eliminar paciente'
  }
}

onMounted(cargarPacientes)
</script>

<style scoped>
.animation-fade { animation: fadeIn 0.3s ease-in-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
input:focus, select:focus { background-color: #fff !important; box-shadow: 0 0 0 0.25rem rgba(14, 165, 233, 0.25); }
</style>
