<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h2 class="fw-bold mb-0">Panel de Administración</h2>
        <p class="text-muted">Resumen general del sistema</p>
      </div>
      <p class="text-muted fw-medium border bg-white px-3 py-2 rounded-pill shadow-sm">{{ fechaActual }}</p>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
      <p class="text-muted mt-2">Cargando estadísticas...</p>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <template v-else>
      <div class="row g-4 mb-4">
        <div class="col-md-4">
          <div class="card bg-info text-white p-3 shadow-sm">
            <h6>Total de Usuarios</h6>
            <h3>{{ stats.totalUsers }}</h3>
            <small class="opacity-75">{{ stats.totalActiveUsers }} activos · {{ stats.totalInactiveUsers }} inactivos</small>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card bg-primary text-white p-3 shadow-sm">
            <h6>Total de Pacientes</h6>
            <h3>{{ stats.totalPatients }}</h3>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card bg-success text-white p-3 shadow-sm">
            <h6>Citas Registradas</h6>
            <h3>{{ totalCitas }}</h3>
          </div>
        </div>
        <div class="col-md-6">
          <div class="card bg-secondary text-white p-3 shadow-sm">
            <h6>Análisis IA Generados</h6>
            <h3>{{ stats.totalAiAnalyses }}</h3>
          </div>
        </div>
        <div class="col-md-6">
          <div class="card bg-danger text-white p-3 shadow-sm">
            <h6>Alertas Clínicas Activas</h6>
            <h3>{{ stats.activeAlerts }}</h3>
          </div>
        </div>
      </div>

      <div class="card shadow-sm border-0">
        <div class="card-header bg-white border-0 py-3">
          <h5 class="fw-bold mb-0">Citas por Estado</h5>
        </div>
        <div class="card-body" v-if="Object.keys(stats.appointmentsByStatus || {}).length">
          <div class="row g-3">
            <div v-for="(count, status) in stats.appointmentsByStatus" :key="status" class="col-md-3 col-6">
              <div class="bg-light rounded-4 p-3 text-center">
                <small class="text-muted d-block fw-bold text-uppercase">{{ status }}</small>
                <h4 class="fw-bold mt-1 mb-0">{{ count }}</h4>
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
import { dashboardService } from '@/api/dashboard'

const loading = ref(true)
const error = ref('')
const stats = ref({
  totalUsers: 0,
  totalActiveUsers: 0,
  totalInactiveUsers: 0,
  totalPatients: 0,
  appointmentsByStatus: {},
  totalAiAnalyses: 0,
  activeAlerts: 0,
  latestAuditLogs: []
})

const fechaActual = computed(() => {
  return new Date().toLocaleDateString('es-ES', {
    day: 'numeric', month: 'long', year: 'numeric'
  })
})

const totalCitas = computed(() => {
  return Object.values(stats.value.appointmentsByStatus || {}).reduce((a, b) => a + b, 0)
})

onMounted(async () => {
  try {
    const res = await dashboardService.getAdminStats()
    stats.value = res.data
  } catch (err) {
    if (err.response?.status === 404 || err.response?.status === 204) {
      stats.value = {
        totalUsers: 0,
        totalActiveUsers: 0,
        totalInactiveUsers: 0,
        totalPatients: 0,
        appointmentsByStatus: {},
        totalAiAnalyses: 0,
        activeAlerts: 0,
        latestAuditLogs: []
      }
    } else {
      error.value = 'Error al cargar estadísticas'
    }
  } finally {
    loading.value = false
  }
})
</script>
