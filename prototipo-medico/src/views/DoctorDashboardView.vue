<template>
  <div>
    <PageHeader :title="`Hola, ${nombreUsuario} 👋`" subtitle="Resumen de tu jornada clínica de hoy" :icon="IconDashboard">
      <template #actions>
        <span class="dashboard-date">{{ fechaActual }}</span>
      </template>
    </PageHeader>

    <LoadingState v-if="loading" label="Cargando estadísticas…" />

    <AppAlert v-else-if="error" variant="danger" :message="error" />

    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-md-3">
          <StatTile label="Total Pacientes" :value="stats.totalPatients" :icon="IconPatients" tone="brand" variant="soft" />
        </div>
        <div class="col-md-3">
          <StatTile label="Citas del Día" :value="stats.totalAppointmentsToday" :icon="IconAppointment" tone="success" variant="soft" />
        </div>
        <div class="col-md-3">
          <StatTile label="Análisis IA Pendientes" :value="stats.pendingAiAnalyses" :icon="IconAi" tone="warning" variant="soft" />
        </div>
        <div class="col-md-3">
          <StatTile label="Alertas Activas" :value="stats.activeAlerts" :icon="IconAlert" tone="danger" variant="solid" />
        </div>
      </div>

      <div class="row g-3 mb-4">
        <div class="col-md-4">
          <StatTile label="Completadas Hoy" :value="stats.completedAppointmentsToday" tone="success" variant="outline" />
        </div>
        <div class="col-md-4">
          <StatTile label="Pendientes Hoy" :value="stats.pendingAppointmentsToday" tone="warning" variant="outline" />
        </div>
        <div class="col-md-4">
          <StatTile
            label="Análisis IA Revisados"
            :value="stats.approvedAiAnalyses + stats.rejectedAiAnalyses"
            tone="brand"
            variant="outline"
          />
        </div>
      </div>

      <BaseCard title="Accesos Rápidos" :icon="IconFast">
        <div class="d-flex gap-3 flex-wrap">
          <AppButton to="/citas" variant="soft" :icon="IconAppointment">Citas</AppButton>
          <AppButton to="/reportes" variant="soft" :icon="IconReport">Reportes</AppButton>
          <AppButton to="/recomendaciones" variant="soft" :icon="IconRecommendation">Recomendaciones</AppButton>
        </div>
      </BaseCard>
    </template>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { authStore } from '@/stores/auth'
import { dashboardService } from '@/api/dashboard'
import { PageHeader, BaseCard, StatTile, AppButton, AppAlert, LoadingState } from '@/components/ui'
import { IconDashboard, IconPatients, IconAppointment, IconAi, IconAlert, IconFast, IconReport, IconRecommendation } from '@/lib/icons'

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
