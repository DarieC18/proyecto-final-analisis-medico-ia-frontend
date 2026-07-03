<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h2 class="fw-bold mb-0">Hola, {{ nombreUsuario }} 👋</h2>
        <p class="text-muted">Resumen de tu jornada clínica de hoy</p>
      </div>
      <p class="text-muted fw-medium border bg-white px-3 py-2 rounded-pill shadow-sm">{{ fechaActual }}</p>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
      <p class="text-muted mt-2">Cargando estadísticas...</p>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <template v-else>
      <div class="row g-4 mb-5">
        <div class="col-md-3">
          <StatCard label="Total Pacientes" :value="stats.totalPatients" color="info" />
        </div>
        <div class="col-md-3">
          <StatCard label="Citas del Día" :value="stats.totalAppointmentsToday" color="success" />
        </div>
        <div class="col-md-3">
          <StatCard label="Análisis IA Pendientes" :value="stats.pendingAiAnalyses" color="warning" />
        </div>
        <div class="col-md-3">
          <StatCard label="Alertas Activas" :value="stats.activeAlerts" color="danger" />
        </div>
      </div>

      <div class="row g-4 mb-5">
        <div class="col-md-4">
          <div class="card bg-light border-0 shadow-sm p-3 text-center rounded-4">
            <h6 class="text-muted fw-bold text-uppercase mb-1">Completadas Hoy</h6>
            <h3 class="fw-bold text-success mb-0">{{ stats.completedAppointmentsToday }}</h3>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card bg-light border-0 shadow-sm p-3 text-center rounded-4">
            <h6 class="text-muted fw-bold text-uppercase mb-1">Pendientes Hoy</h6>
            <h3 class="fw-bold text-warning mb-0">{{ stats.pendingAppointmentsToday }}</h3>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card bg-light border-0 shadow-sm p-3 text-center rounded-4">
            <h6 class="text-muted fw-bold text-uppercase mb-1">Análisis IA Revisados</h6>
            <h3 class="fw-bold text-primary mb-0">{{ stats.approvedAiAnalyses + stats.rejectedAiAnalyses }}</h3>
          </div>
        </div>
      </div>

      <div class="bg-white p-4 rounded-4 shadow-sm">
        <h5 class="fw-bold mb-4">Accesos Rápidos</h5>
        <div class="d-flex gap-3 flex-wrap">
          <RouterLink to="/pacientes" class="btn btn-light border px-4 py-2 text-primary fw-medium">👥 Gestión de Pacientes</RouterLink>
          <RouterLink to="/citas" class="btn btn-light border px-4 py-2 text-success fw-medium">📅 Agendar Nueva Cita</RouterLink>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { authStore } from '@/stores/auth'
import { dashboardService } from '@/api/dashboard'
import StatCard from '@/components/StatCard.vue'

const auth = authStore
const loading = ref(true)
const error = ref('')
const stats = ref({
  totalPatients: 0,
  activeAlerts: 0,
  totalAppointmentsToday: 0,
  completedAppointmentsToday: 0,
  pendingAppointmentsToday: 0,
  totalAiAnalyses: 0,
  pendingAiAnalyses: 0,
  approvedAiAnalyses: 0,
  rejectedAiAnalyses: 0
})

const nombreUsuario = computed(() => {
  return auth.user?.name ? `${auth.user.name} ${auth.user.lastName}` : 'Usuario'
})

const fechaActual = computed(() => {
  return new Date().toLocaleDateString('es-ES', {
    day: 'numeric', month: 'long', year: 'numeric'
  })
})

onMounted(async () => {
  try {
    const res = await dashboardService.getDoctorStats()
    stats.value = res.data
  } catch (err) {
    if (err.response?.status === 404 || err.response?.status === 204) {
      stats.value = {
        totalPatients: 0,
        activeAlerts: 0,
        totalAppointmentsToday: 0,
        completedAppointmentsToday: 0,
        pendingAppointmentsToday: 0,
        totalAiAnalyses: 0,
        pendingAiAnalyses: 0,
        approvedAiAnalyses: 0,
        rejectedAiAnalyses: 0
      }
    } else {
      error.value = 'Error al cargar estadísticas del dashboard'
    }
  } finally {
    loading.value = false
  }
})
</script>
