<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Mis Citas</h3>
        <p class="text-muted">Historial de citas médicas</p>
      </div>
      <button v-if="vista === 'lista'" @click="abrirCrear" class="btn btn-primary px-4 shadow-sm">+ Solicitar Cita</button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <div v-else-if="vista === 'crear'" class="animation-fade">
      <div class="card shadow-sm border-0 rounded-4">
        <div class="card-body p-5">
          <h5 class="fw-bold mb-4">Solicitar Nueva Cita</h5>
          <div v-if="formError" class="alert alert-danger border-0 rounded-3 py-2 small mb-4">{{ formError }}</div>
          <form @submit.prevent="solicitar">
            <div class="row g-3 mb-4">
              <div class="col-md-6">
                <label class="form-label fw-medium">Doctor</label>
                <select v-model="form.doctorId" class="form-select" required>
                  <option value="" disabled>Selecciona un doctor...</option>
                  <option v-for="d in doctores" :key="d.id" :value="d.id">{{ d.fullName }}<span v-if="d.specialty"> — {{ d.specialty }}</span></option>
                </select>
              </div>
              <div class="col-md-6">
                <label class="form-label fw-medium">Fecha de la Cita</label>
                <input v-model="form.appointmentDate" type="datetime-local" class="form-control" :min="minDate" required>
              </div>
              <div class="col-12">
                <label class="form-label fw-medium">Motivo de Consulta</label>
                <input v-model="form.reason" type="text" class="form-control" placeholder="Razón principal de la consulta" required>
              </div>
              <div class="col-12">
                <label class="form-label fw-medium">Notas <span class="text-muted fw-normal">(opcional)</span></label>
                <textarea v-model="form.notes" class="form-control" rows="3" placeholder="Notas adicionales..."></textarea>
              </div>
            </div>

            <hr class="my-4">

            <div class="d-flex justify-content-end gap-3">
              <button type="button" @click="vista = 'lista'" class="btn btn-outline-secondary rounded-pill px-4 py-2">Cancelar</button>
              <button type="submit" class="btn btn-primary rounded-pill px-5 py-2 shadow" :disabled="saving">
                <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                {{ saving ? 'Enviando...' : 'Solicitar Cita' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <template v-else>
      <div v-if="citas.length === 0" class="text-center py-5 text-muted">
        <p>No tienes citas registradas.</p>
      </div>
      <div v-else class="row g-4">
        <div v-for="c in citas" :key="c.id" class="col-md-6">
          <div class="card shadow-sm border-0 rounded-4 h-100">
            <div class="card-body p-4">
              <div class="d-flex justify-content-between align-items-start mb-3">
                <div>
                  <h6 class="fw-bold mb-1">{{ c.reason || 'Sin motivo' }}</h6>
                  <small class="text-muted">{{ c.doctorName || 'Médico no asignado' }}</small>
                </div>
                <StatusBadge :text="c.status" :variant="c.status?.toLowerCase()" />
              </div>
              <div class="d-flex justify-content-between align-items-center">
                <small class="text-muted">
                  <span class="fw-medium">{{ formatDate(c.appointmentDate) }}</span>
                </small>
                <button v-if="c.status === 'Pending'" @click="cancelarCita(c)" class="btn btn-sm btn-outline-danger rounded-pill px-3" :disabled="cancelling === c.id">
                  <span v-if="cancelling === c.id" class="spinner-border spinner-border-sm me-1"></span>
                  Cancelar
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { portalService } from '@/api/portal'
import StatusBadge from '@/components/StatusBadge.vue'

const loading = ref(true)
const error = ref('')
const citas = ref([])
const cancelling = ref(null)
const vista = ref('lista')
const doctores = ref([])
const saving = ref(false)
const formError = ref('')
const form = ref({ doctorId: '', appointmentDate: '', reason: '', notes: '' })

const minDate = computed(() => {
  const d = new Date()
  d.setDate(d.getDate() + 1)
  d.setHours(0, 0, 0, 0)
  return d.toISOString().slice(0, 16)
})

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit' })
}

const cargarCitas = async () => {
  try {
    const res = await portalService.getAppointments()
    citas.value = res.data || []
  } catch (err) {
    if (err.response?.status === 404) {
      error.value = 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
    } else {
      error.value = 'Error al cargar citas'
    }
  }
}

const abrirCrear = async () => {
  formError.value = ''
  form.value = { doctorId: '', appointmentDate: '', reason: '', notes: '' }
  vista.value = 'crear'
  if (doctores.value.length === 0) {
    try {
      const res = await portalService.getDoctors()
      doctores.value = res.data || []
    } catch {
      // si falla, el select solo mostrará "Sin preferencia"
    }
  }
}

const solicitar = async () => {
  formError.value = ''
  if (!form.value.doctorId) {
    formError.value = 'Debes seleccionar un doctor para continuar.'
    return
  }
  if (form.value.appointmentDate) {
    const selected = new Date(form.value.appointmentDate)
    const tomorrow = new Date()
    tomorrow.setDate(tomorrow.getDate() + 1)
    tomorrow.setHours(0, 0, 0, 0)
    if (selected < tomorrow) {
      formError.value = 'No puedes agendar una cita para el mismo día ni en el pasado. Selecciona una fecha a partir de mañana.'
      return
    }
  }
  saving.value = true
  try {
    await portalService.requestAppointment({
      doctorId: form.value.doctorId,
      appointmentDate: form.value.appointmentDate,
      reason: form.value.reason,
      notes: form.value.notes || null
    })
    await cargarCitas()
    vista.value = 'lista'
  } catch (err) {
    formError.value = err.response?.data?.message || err.response?.data?.detail || 'Error al solicitar la cita. Intenta de nuevo.'
  } finally {
    saving.value = false
  }
}

const cancelarCita = async (c) => {
  cancelling.value = c.id
  try {
    await portalService.cancelAppointment(c.id)
    c.status = 'Cancelled'
  } catch {
    error.value = 'Error al cancelar la cita'
  } finally {
    cancelling.value = null
  }
}

onMounted(async () => {
  await cargarCitas()
  loading.value = false
})
</script>
