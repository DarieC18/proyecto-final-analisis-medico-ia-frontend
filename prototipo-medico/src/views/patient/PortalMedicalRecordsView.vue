<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Historial Clínico</h3>
        <p class="text-muted">Registros médicos de tus consultas</p>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <div v-else-if="records.length === 0" class="text-center py-5 text-muted">
      <div class="display-4 mb-3">📋</div>
      <p>No hay registros clínicos disponibles.</p>
    </div>

    <div v-else class="row g-4">
      <div class="col-12" v-for="r in records" :key="r.id">
        <div class="card border-0 shadow-sm rounded-4">
          <div class="card-body p-4" v-if="r.appointmentDate">
            <div class="d-flex justify-content-between align-items-start mb-3">
              <h6 class="fw-bold mb-0">Consulta - {{ formatDate(r.appointmentDate) }}</h6>
              <StatusBadge :text="r.appointmentStatus" :variant="r.appointmentStatus?.toLowerCase()" />
            </div>
            <p class="mb-1"><strong>Médico:</strong> {{ r.doctorName }}</p>
            <p class="mb-1"><strong>Motivo:</strong> {{ r.reason }}</p>
            <p class="mb-0"><strong>Diagnóstico:</strong> {{ r.diagnosisInitial || 'Pendiente' }}</p>
            <p class="mb-0 mt-2" v-if="r.notes"><strong>Notas:</strong> {{ r.notes }}</p>
          </div>
          <div class="card-body p-4" v-else>
            <div class="d-flex justify-content-between align-items-start mb-3">
              <h6 class="fw-bold mb-0">Registro #{{ r.id }}</h6>
              <small class="text-muted">{{ formatDate(r.createdAt) }}</small>
            </div>
            <p class="mb-1"><strong>Diagnóstico:</strong> {{ r.diagnosisInitial || 'N/A' }}</p>
            <p class="mb-0"><strong>Notas:</strong> {{ r.notes || 'N/A' }}</p>
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
const records = ref([])

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch { return dateStr }
}

onMounted(async () => {
  loading.value = true
  try {
    const res = await portalService.getMedicalRecords()
    records.value = res.data || []
  } catch (err) {
    if (err.response?.status !== 204) error.value = 'Error al cargar historial clínico'
  } finally {
    loading.value = false
  }
})
</script>
