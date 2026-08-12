<template>
  <div>
    <PageHeader title="Panel de Administración" subtitle="Resumen general del sistema" :icon="IconDashboard">
      <template #actions>
        <span class="dashboard-date">{{ fechaActual }}</span>
      </template>
    </PageHeader>

    <LoadingState v-if="loading" label="Cargando estadísticas…" />

    <AppAlert v-else-if="error" variant="danger" :message="error" />

    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-md-4">
          <StatTile
            label="Total de Usuarios"
            :value="stats.totalUsers"
            :hint="`${stats.totalActiveUsers} activos · ${stats.totalInactiveUsers} inactivos`"
            :icon="IconRoles"
            tone="brand"
            variant="solid"
          />
        </div>
        <div class="col-md-4">
          <StatTile label="Total de Pacientes" :value="stats.totalPatients" :icon="IconPatients" tone="success" variant="solid" />
        </div>
        <div class="col-md-4">
          <StatTile label="Citas Registradas" :value="totalCitas" :icon="IconAppointment" tone="warning" variant="solid" />
        </div>
        <div class="col-md-6">
          <StatTile label="Análisis IA Generados" :value="stats.totalAiAnalyses" :icon="IconAi" tone="info" variant="solid" />
        </div>
        <div class="col-md-6">
          <StatTile label="Alertas Clínicas Activas" :value="stats.activeAlerts" :icon="IconAlert" tone="danger" variant="solid" />
        </div>
      </div>

      <BaseCard title="Citas por Estado" :icon="IconAppointment">
        <div v-if="Object.keys(stats.appointmentsByStatus || {}).length" class="row g-3">
          <div v-for="(count, status) in stats.appointmentsByStatus" :key="status" class="col-md-3 col-6">
            <StatTile :label="statusLabel(status)" :value="count" :tone="statusTone(status)" variant="outline" />
          </div>
        </div>
        <EmptyState v-else title="Sin citas registradas" />
      </BaseCard>
    </template>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { dashboardService } from '@/api/dashboard'
import { PageHeader, BaseCard, StatTile, AppAlert, LoadingState, EmptyState } from '@/components/ui'
import { IconDashboard, IconRoles, IconPatients, IconAppointment, IconAi, IconAlert } from '@/lib/icons'
import { statusVariant } from '@/utils/appointmentStatus'

const STATUS_TONES = { pending: 'warning', inprogress: 'brand', completed: 'success', cancelled: 'danger' }
const statusTone = (status) => STATUS_TONES[statusVariant(status)] ?? 'brand'

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

const statusLabel = (status) => ({ Pending: 'Pendiente', InProgress: 'En Progreso', Completed: 'Completada', Cancelled: 'Cancelada' })[status] || status

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

<style scoped>
.dashboard-date {
  font-size: 0.8125rem;
  font-weight: 500;
  color: var(--app-text-muted);
  background-color: var(--app-surface);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius-pill);
  padding: 0.4rem 0.9rem;
  white-space: nowrap;
}
</style>
