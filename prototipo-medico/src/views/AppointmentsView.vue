<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Agenda de Citas</h3>
        <p class="text-muted">Gestiona las consultas médicas programadas</p>
      </div>
      <button @click="abrirCrear" class="btn btn-primary px-4 shadow-sm">+ Agendar Cita</button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <div v-else-if="vista === 'crear'" class="animation-fade">
      <div class="card shadow-sm border-0 rounded-4">
        <div class="card-body p-5">
          <h5 class="fw-bold mb-4">Nueva Cita Médica</h5>
          <div v-if="formError" class="alert alert-danger border-0 rounded-3 py-2 small mb-4">{{ formError }}</div>
          <form @submit.prevent="crearCita">
            <div class="row g-4">
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Paciente</label>
                <select v-model="form.patientId" class="form-select bg-light border-0 py-2" required>
                  <option value="">Seleccione un paciente...</option>
                  <option v-for="p in pacientes" :key="p.id" :value="p.id">{{ p.fullName }} - {{ p.identificationNumber }}</option>
                </select>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Fecha de la Cita</label>
                <input v-model="form.appointmentDate" type="datetime-local" class="form-control bg-light border-0 py-2" required>
              </div>
              <div class="col-md-12">
                <label class="form-label text-muted small fw-bold text-uppercase">Motivo de Consulta</label>
                <input v-model="form.reason" type="text" class="form-control bg-light border-0 py-2" placeholder="Razón principal de la consulta" required>
              </div>
              <div class="col-md-12">
                <label class="form-label text-muted small fw-bold text-uppercase">Notas (opcional)</label>
                <textarea v-model="form.notes" class="form-control bg-light border-0 p-3" rows="3" placeholder="Notas adicionales..."></textarea>
              </div>
            </div>
            <div class="d-flex justify-content-end gap-3 mt-5">
              <button type="button" @click="cancelarForm" class="btn btn-light border px-4 py-2">Cancelar</button>
              <button type="submit" class="btn btn-primary px-4 py-2 shadow-sm" :disabled="saving">
                <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                {{ saving ? 'Guardando...' : 'Crear Cita' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <template v-else>
      <div class="card shadow-sm overflow-hidden border-0">
        <div v-if="citas.length === 0" class="text-center py-5 text-muted">
          <p>No hay citas registradas.</p>
        </div>
        <div v-else class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="bg-light text-muted">
              <tr>
                <th class="ps-4 py-3 fw-medium">Paciente</th>
                <th class="py-3 fw-medium">Médico</th>
                <th class="py-3 fw-medium">Fecha de Cita</th>
                <th class="py-3 fw-medium">Motivo de Consulta</th>
                <th class="py-3 fw-medium">Estado</th>
                <th class="pe-4 py-3 fw-medium text-end">Acciones</th>
              </tr>
            </thead>
            <tbody class="border-top-0">
              <tr v-for="c in citas" :key="c.id">
                <td class="ps-4 py-3 fw-bold text-dark">{{ getPatientName(c.patientId) }}</td>
                <td class="py-3 text-muted">{{ getDoctorName(c.doctorId) }}</td>
                <td class="py-3 text-muted">{{ formatDateTime(c.appointmentDate) }}</td>
                <td class="py-3 text-muted">{{ c.reason }}</td>
                <td class="py-3">
                  <StatusBadge :text="c.status" :variant="statusVariant(c.status)" />
                </td>
                <td class="pe-4 py-3 text-end">
                  <button @click="cambiarEstado(c)" class="btn btn-sm btn-light border text-success fw-medium px-3 me-2" v-if="c.status === 'Pending'">Iniciar</button>
                  <RouterLink :to="`/citas/${c.id}`" class="btn btn-sm btn-light border text-primary fw-medium px-3">Ir al Detalle</RouterLink>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { appointmentService } from '@/api/appointments'
import { patientService } from '@/api/patients'
import { authStore } from '@/stores/auth'
import StatusBadge from '@/components/StatusBadge.vue'

const router = useRouter()
const auth = authStore
const loading = ref(true)
const error = ref('')
const saving = ref(false)
const formError = ref('')
const citas = ref([])
const pacientes = ref([])
const doctores = ref([])
const vista = ref('lista')

const form = reactive({
  patientId: '',
  appointmentDate: '',
  reason: '',
  notes: ''
})

const statusVariant = (status) => {
  const map = { Pending: 'pending', InProgress: 'inprogress', Completed: 'completed', Cancelled: 'cancelled' }
  return map[status] || 'secondary'
}

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

const getPatientName = (id) => {
  const p = pacientes.value.find(p => p.id === id)
  return p ? p.fullName : `Paciente #${id}`
}

const getDoctorName = (id) => {
  const d = doctores.value.find(d => d.id === id)
  return d ? `${d.name} ${d.lastName}` : `Dr. #${id}`
}

const cargarCitas = async () => {
  loading.value = true
  error.value = ''
  try {
    const [citasRes, pacientesRes] = await Promise.all([
      appointmentService.getAll(),
      patientService.getAll()
    ])
    citas.value = citasRes.data || []
    pacientes.value = pacientesRes.data || []

    const userData = localStorage.getItem('user')
    if (userData) {
      const user = JSON.parse(userData)
      doctores.value = [{ id: user.id, name: user.name, lastName: user.lastName }]
    }
  } catch (err) {
    if (err.response?.status === 404 || err.response?.status === 204) {
      citas.value = []
    } else {
      error.value = 'Error al cargar citas'
    }
  } finally {
    loading.value = false
  }
}

const abrirCrear = () => {
  vista.value = 'crear'
  formError.value = ''
  form.patientId = ''
  form.appointmentDate = ''
  form.reason = ''
  form.notes = ''
}

const cancelarForm = () => {
  vista.value = 'lista'
}

const crearCita = async () => {
  saving.value = true
  formError.value = ''
  try {
    const payload = {
      patientId: Number(form.patientId),
      appointmentDate: form.appointmentDate,
      reason: form.reason,
      notes: form.notes || null
    }
    await appointmentService.create(payload)
    vista.value = 'lista'
    await cargarCitas()
  } catch (err) {
    const data = err.response?.data
    formError.value = data?.errors?.join(', ') || data?.message || 'Error al crear la cita'
  } finally {
    saving.value = false
  }
}

const cambiarEstado = async (c) => {
  try {
    await appointmentService.changeStatus(c.id, 'InProgress')
    await cargarCitas()
  } catch (err) {
    error.value = 'Error al cambiar estado de la cita'
  }
}

onMounted(cargarCitas)
</script>

<style scoped>
.animation-fade { animation: fadeIn 0.3s ease-in-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
input:focus, select:focus, textarea:focus { background-color: #fff !important; box-shadow: 0 0 0 0.25rem rgba(14, 165, 233, 0.25); }
</style>
