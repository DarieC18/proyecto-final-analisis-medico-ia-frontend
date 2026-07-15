<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Mis Citas</h3>
        <p class="text-muted">Gestiona tus citas médicas</p>
      </div>
      <button @click="showForm = !showForm" class="btn btn-primary px-4 shadow-sm">
        {{ showForm ? 'Cancelar' : '+ Solicitar Cita' }}
      </button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <div v-if="showForm" class="card shadow-sm border-0 rounded-4 mb-4">
      <div class="card-body p-5">
        <h5 class="fw-bold mb-4">Solicitar Nueva Cita</h5>
        <div v-if="formError" class="alert alert-danger border-0 rounded-3 py-2 small mb-4">{{ formError }}</div>
        <form @submit.prevent="solicitarCita">
          <div class="row g-4">
            <div class="col-md-6">
              <label class="form-label text-muted small fw-bold text-uppercase">Médico</label>
              <select v-model="citaForm.doctorId" class="form-select bg-light border-0 py-2">
                <option value="">Asignación automática (Medicina General)</option>
                <option v-for="d in doctores" :key="d.id" :value="d.id">{{ d.fullName }} {{ d.specialty ? `- ${d.specialty}` : '' }}</option>
              </select>
            </div>
            <div class="col-md-6">
              <label class="form-label text-muted small fw-bold text-uppercase">Fecha y Hora</label>
              <input v-model="citaForm.appointmentDate" type="datetime-local" class="form-control bg-light border-0 py-2" required>
            </div>
            <div class="col-md-12">
              <label class="form-label text-muted small fw-bold text-uppercase">Motivo de Consulta</label>
              <input v-model="citaForm.reason" type="text" class="form-control bg-light border-0 py-2" placeholder="Razón principal de la consulta" required>
            </div>
            <div class="col-md-12">
              <label class="form-label text-muted small fw-bold text-uppercase">Notas (opcional)</label>
              <textarea v-model="citaForm.notes" class="form-control bg-light border-0 p-3" rows="3" placeholder="Notas adicionales..."></textarea>
            </div>
          </div>
          <div class="d-flex justify-content-end gap-3 mt-5">
            <button type="button" @click="showForm = false" class="btn btn-light border px-4 py-2">Cancelar</button>
            <button type="submit" class="btn btn-primary px-4 py-2 shadow-sm" :disabled="enviando">
              <span v-if="enviando" class="spinner-border spinner-border-sm me-2"></span>
              Solicitar Cita
            </button>
          </div>
        </form>
      </div>
    </div>

    <div v-if="!loading && citas.length === 0 && !showForm" class="text-center py-5 text-muted">
      <div class="display-4 mb-3">📅</div>
      <p>No tienes citas registradas.</p>
    </div>

    <div v-else-if="!loading" class="row g-4">
      <div class="col-md-6" v-for="c in citas" :key="c.id">
        <div class="card border-0 shadow-sm rounded-4 h-100">
          <div class="card-body p-4">
            <div class="d-flex justify-content-between align-items-start mb-3">
              <div>
                <h6 class="fw-bold mb-1">Cita #{{ c.id }}</h6>
                <small class="text-muted">{{ formatDate(c.appointmentDate) }}</small>
              </div>
              <StatusBadge :text="c.status" :variant="statusVariant(c.status)" />
            </div>
            <p class="mb-2"><strong>Motivo:</strong> {{ c.reason }}</p>
            <p class="mb-2" v-if="c.notes"><strong>Notas:</strong> {{ c.notes }}</p>
            <div class="d-flex justify-content-end mt-3" v-if="c.status === 'Pending'">
              <button @click="cancelarCita(c)" class="btn btn-sm btn-light border text-danger px-3" :disabled="cancelando === c.id">
                <span v-if="cancelando === c.id" class="spinner-border spinner-border-sm me-1"></span>
                Cancelar Cita
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { portalService } from '@/api/portal'
import StatusBadge from '@/components/StatusBadge.vue'

const loading = ref(true)
const error = ref('')
const citas = ref([])
const doctores = ref([])
const showForm = ref(false)
const enviando = ref(false)
const formError = ref('')
const cancelando = ref(null)

const citaForm = ref({
  doctorId: '', appointmentDate: '', reason: '', notes: ''
})

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch { return dateStr }
}

const statusVariant = (status) => {
  const map = { Pending: 'pending', InProgress: 'inprogress', Completed: 'completed', Cancelled: 'cancelled' }
  return map[status] || 'secondary'
}

const cargarCitas = async () => {
  try {
    const res = await portalService.getAppointments()
    citas.value = res.data || []
  } catch {
    citas.value = []
  }
}

const cargarDoctores = async () => {
  try {
    const res = await portalService.getDoctors()
    doctores.value = res.data || []
  } catch {
    doctores.value = []
  }
}

const solicitarCita = async () => {
  if (!citaForm.value.appointmentDate || !citaForm.value.reason) return
  enviando.value = true
  formError.value = ''
  try {
    await portalService.requestAppointment({ ...citaForm.value })
    showForm.value = false
    citaForm.value = { doctorId: '', appointmentDate: '', reason: '', notes: '' }
    await cargarCitas()
  } catch (err) {
    formError.value = err.response?.data?.message || 'Error al solicitar cita'
  } finally {
    enviando.value = false
  }
}

const cancelarCita = async (c) => {
  cancelando.value = c.id
  try {
    await portalService.cancelAppointment(c.id)
    c.status = 'Cancelled'
  } catch (err) {
    error.value = err.response?.data?.message || 'Error al cancelar cita'
  } finally {
    cancelando.value = null
  }
}

onMounted(async () => {
  loading.value = true
  await Promise.all([cargarCitas(), cargarDoctores()])
  loading.value = false
})
</script>
