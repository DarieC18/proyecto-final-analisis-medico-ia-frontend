<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Mis Citas</h3>
        <p class="text-muted">Historial de citas médicas</p>
      </div>
    </div>
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>
    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>
    <div v-else-if="citas.length === 0" class="text-center py-5 text-muted">
      <p>No tienes citas registradas.</p>
    </div>
    <div v-else>
      <div class="row g-4">
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
const cancelling = ref(null)

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit' })
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
  try {
    const res = await portalService.getAppointments()
    citas.value = res.data || []
  } catch (err) {
    if (err.response?.status === 404) {
      error.value = 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
    } else {
      error.value = 'Error al cargar citas'
    }
  } finally {
    loading.value = false
  }
})
</script>
