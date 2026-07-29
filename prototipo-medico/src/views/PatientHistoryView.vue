<template>
  <div class="container mt-4">
    <div class="mb-4">
      <h3 class="fw-bold mb-0">Historial Clínico</h3>
      <p class="text-muted">Registro de tus consultas médicas</p>
    </div>
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>
    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>
    <div v-else-if="records.length === 0" class="text-center py-5 text-muted">
      <p>No hay registros clínicos disponibles.</p>
    </div>
    <div v-else class="card shadow-sm border-0 rounded-4 overflow-hidden">
      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="bg-light text-muted">
            <tr>
              <th class="ps-4 py-3 fw-medium">Fecha</th>
              <th class="py-3 fw-medium">Médico</th>
              <th class="py-3 fw-medium">Motivo</th>
              <th class="pe-4 py-3 fw-medium">Estado</th>
            </tr>
          </thead>
          <tbody class="border-top-0">
            <tr v-for="r in records" :key="r.id">
              <td class="ps-4 py-3">{{ formatDate(r.appointmentDate) }}</td>
              <td class="py-3">{{ r.doctorName || '-' }}</td>
              <td class="py-3">{{ r.reason || '-' }}</td>
              <td class="pe-4 py-3"><StatusBadge :text="r.appointmentStatus" :variant="r.appointmentStatus?.toLowerCase()" /></td>
            </tr>
          </tbody>
        </table>
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
const records = ref([])

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' })
}

onMounted(async () => {
  try {
    const res = await portalService.getMedicalRecords()
    records.value = res.data || []
  } catch (err) {
    if (err.response?.status === 404) {
      error.value = 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
    } else {
      error.value = 'Error al cargar historial'
    }
  } finally {
    loading.value = false
  }
})
</script>
