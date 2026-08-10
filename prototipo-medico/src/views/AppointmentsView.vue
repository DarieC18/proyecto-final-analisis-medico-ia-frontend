<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Agenda de Citas</h3>
        <p class="text-muted">Gestiona las consultas médicas programadas</p>
      </div>
      <button v-if="auth.hasRole('Nurse')" @click="abrirCrear" class="btn btn-primary px-4 shadow-sm">+ Agendar Cita</button>
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
            <div class="form-section mb-4">
              <div class="section-header">
                <span class="section-icon">📋</span>
                <span>Información de la Cita</span>
              </div>
              <div class="section-body">
                <div class="row g-3">
                  <div class="col-md-6">
                    <label class="form-label fw-medium">Paciente</label>
                    <select v-model="form.patientId" class="form-select" required>
                      <option value="">Seleccione un paciente...</option>
                      <option v-for="p in pacientes" :key="p.id" :value="p.id">{{ p.fullName }} - {{ p.identificationNumber }}</option>
                    </select>
                  </div>
                  <div class="col-md-6">
                    <label class="form-label fw-medium">Doctor</label>
                    <select v-model="form.doctorId" class="form-select" required>
                      <option value="">Seleccione un doctor...</option>
                      <option v-for="d in doctores" :key="d.id" :value="d.id">{{ d.name }} {{ d.lastName }}</option>
                    </select>
                  </div>
                  <div class="col-md-6">
                    <label class="form-label fw-medium">Fecha de la Cita</label>
                    <input v-model="form.appointmentDate" type="datetime-local" class="form-control" required>
                  </div>
                </div>
              </div>
            </div>

            <div class="form-section mb-4">
              <div class="section-header">
                <span class="section-icon">📝</span>
                <span>Detalle de la Consulta</span>
              </div>
              <div class="section-body">
                <div class="row g-3">
                  <div class="col-12">
                    <label class="form-label fw-medium">Motivo de Consulta</label>
                    <input v-model="form.reason" type="text" class="form-control" placeholder="Razón principal de la consulta" required>
                  </div>
                  <div class="col-12">
                    <label class="form-label fw-medium">Notas <span class="text-muted fw-normal">(opcional)</span></label>
                    <textarea v-model="form.notes" class="form-control" rows="3" placeholder="Notas adicionales..."></textarea>
                  </div>
                </div>
              </div>
            </div>

            <hr class="my-4">

            <div class="d-flex justify-content-end gap-3">
              <button type="button" @click="cancelarForm" class="btn btn-outline-secondary rounded-pill px-4 py-2">Cancelar</button>
              <button type="submit" class="btn btn-primary rounded-pill px-5 py-2 shadow" :disabled="saving">
                <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                {{ saving ? 'Guardando...' : '📅 Crear Cita' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <template v-else>
      <div class="mb-3">
        <input
          v-model="filtro"
          type="text"
          class="form-control"
          placeholder="Buscar por paciente o médico..."
          style="max-width: 360px;"
        >
      </div>
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
              <tr v-for="c in citasFiltradas" :key="c.id">
                <td class="ps-4 py-3 fw-bold text-dark">{{ c.patientName || getPatientName(c.patientId) }}</td>
                <td class="py-3 text-muted">{{ c.doctorName || getDoctorName(c.doctorId) }}</td>
                <td class="py-3 text-muted">{{ formatDateTime(c.appointmentDate) }}</td>
                <td class="py-3 text-muted">{{ c.reason }}</td>
                <td class="py-3">
                  <StatusBadge :text="translateStatus(c.status)" :variant="statusVariant(c.status)" />
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
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { appointmentService } from '@/api/appointments'
import { patientService } from '@/api/patients'
import { userService } from '@/api/users'
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
const filtro = ref('')

const citasFiltradas = computed(() => {
  const q = filtro.value.trim().toLowerCase()
  if (!q) return citas.value
  return citas.value.filter(c =>
    c.patientName?.toLowerCase().includes(q) ||
    c.doctorName?.toLowerCase().includes(q)
  )
})

const form = reactive({
  patientId: '',
  doctorId: '',
  appointmentDate: '',
  reason: '',
  notes: ''
})

const statusVariant = (status) => {
  const map = { Pending: 'pending', InProgress: 'inprogress', Completed: 'completed', Cancelled: 'cancelled' }
  return map[status] || 'secondary'
}

const translateStatus = (status) => {
  const map = { Pending: 'Pendiente', InProgress: 'En progreso', Completed: 'Completada', Cancelled: 'Cancelada' }
  return map[status] || status
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

    if (auth.hasRole('Nurse')) {
      try {
        const doctoresRes = await userService.getDoctors()
        doctores.value = doctoresRes.data || []
      } catch {
        doctores.value = []
      }
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
  form.doctorId = ''
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
      doctorId: form.doctorId,
      appointmentDate: form.appointmentDate,
      reason: form.reason,
      notes: form.notes || null
    }
    await appointmentService.create(payload)
    vista.value = 'lista'
    await cargarCitas()
  } catch (err) {
    const data = err.response?.data
    if (data?.errors && typeof data.errors === 'object') {
      formError.value = Object.values(data.errors).flat().join(', ')
    } else if (data?.errors && Array.isArray(data.errors)) {
      formError.value = data.errors.join(', ')
    } else {
      formError.value = data?.message || data?.detail || 'Error al crear la cita'
    }
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

.form-section {
  background: #f8fafc;
  border: 1px solid #e9ecef;
  border-radius: 12px;
  overflow: hidden;
  transition: box-shadow 0.2s;
}
.form-section:hover {
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}
.section-header {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  background: #fff;
  border-bottom: 1px solid #e9ecef;
  font-weight: 600;
  font-size: 0.95rem;
  color: #1e293b;
}
.section-icon {
  font-size: 1.2rem;
}
.section-body {
  padding: 16px;
}
.form-section .form-label {
  font-size: 0.85rem;
  color: #334155;
  margin-bottom: 4px;
}
.form-section .form-control,
.form-section .form-select {
  border: 1px solid #d1d5db;
  background: #fff;
  padding: 10px 12px;
  border-radius: 8px;
  transition: all 0.2s ease;
}
.form-section .form-control:focus,
.form-section .form-select:focus,
.form-section textarea:focus {
  border-color: #0d6efd;
  box-shadow: 0 0 0 3px rgba(13, 110, 253, 0.15);
  background: #fff;
}
.form-section .form-control::placeholder,
.form-section textarea::placeholder {
  color: #94a3b8;
  font-size: 0.9rem;
}
</style>
