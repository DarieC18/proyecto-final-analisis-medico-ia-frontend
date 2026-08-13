<template>
  <div>
    <PageHeader
      title="Alertas Clínicas"
      subtitle="Monitor de alertas generadas por signos vitales fuera de rango"
      :icon="IconAlert"
      tone="danger"
    >
      <template #actions>
        <AppButton
          :variant="filtro === 'activas' ? 'primary' : 'soft'"
          size="sm"
          @click="filtro = 'activas'"
        >
          Activas
        </AppButton>
        <AppButton :variant="filtro === 'todas' ? 'primary' : 'soft'" size="sm" @click="filtro = 'todas'">
          Todas
        </AppButton>
      </template>
    </PageHeader>

    <LoadingState v-if="loading" label="Cargando alertas…" />

    <AppAlert v-else-if="error" variant="danger" :message="error" />

    <BaseCard v-else-if="alertas.length === 0" flush padding="none">
      <EmptyState
        :icon="IconAlert"
        :title="filtro === 'activas' ? 'Sin alertas activas' : 'Sin alertas registradas'"
        message="Las alertas se generan automáticamente cuando unos signos vitales quedan fuera de rango."
      />
    </BaseCard>

    <div v-else class="row g-3">
      <div v-for="a in alertas" :key="a.id" class="col-md-6">
        <BaseCard class="h-100" :tone="a.isResolved ? 'muted' : 'danger'">
          <div class="d-flex justify-content-between align-items-start gap-2 mb-3">
            <div class="u-min-w-0">
              <h3 class="alerta__tipo">{{ a.alertType || 'Alerta clínica' }}</h3>
              <small class="text-app-muted">{{ formatDate(a.createdAt) }}</small>
            </div>
            <StatusBadge
              :text="a.isResolved ? 'Resuelta' : 'Activa'"
              :variant="a.isResolved ? 'completed' : 'cancelled'"
              dot
            />
          </div>

          <div class="d-grid gap-2 mb-3">
            <DataField label="Paciente" :value="a.patientName || `#${a.patientId}`" :icon="IconPatients" />
            <DataField v-if="a.appointmentId" label="Cita" :value="`#${a.appointmentId}`" :icon="IconAppointment" />
            <DataField v-if="a.description" label="Descripción" :value="a.description" />
            <DataField v-if="a.severity" label="Severidad">
              <StatusBadge :text="a.severity" :variant="severityVariant(a.severity)" />
            </DataField>
          </div>

          <div v-if="!a.isResolved" class="d-flex justify-content-end">
            <AppButton
              variant="success"
              size="sm"
              :icon="IconCheck"
              :loading="resolviendo === a.id"
              @click="resolverAlerta(a)"
            >
              Marcar resuelta
            </AppButton>
          </div>
        </BaseCard>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import { alertService } from '@/api/alerts'
import {
  AppAlert,
  AppButton,
  BaseCard,
  DataField,
  EmptyState,
  LoadingState,
  PageHeader,
  StatusBadge
} from '@/components/ui'
import { IconAlert, IconAppointment, IconCheck, IconPatients } from '@/lib/icons'

const loading = ref(true)
const error = ref('')
const alertas = ref([])
const filtro = ref('activas')
const resolviendo = ref(null)

const formatDate = (dateStr) => {
  if (!dateStr) return '—'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch {
    return dateStr
  }
}

const SEVERITY_VARIANTS = {
  Leve: 'low',
  Moderado: 'moderate',
  Severo: 'severe',
  Critical: 'critical'
}
const severityVariant = (sev) => SEVERITY_VARIANTS[sev] ?? 'secondary'

const cargarAlertas = async () => {
  loading.value = true
  error.value = ''
  try {
    const res = filtro.value === 'activas' ? await alertService.getActive() : await alertService.getAll()
    alertas.value = res.data || []
  } catch (err) {
    if (err.response?.status === 204) {
      alertas.value = []
    } else {
      error.value = 'Error al cargar alertas'
    }
  } finally {
    loading.value = false
  }
}

const resolverAlerta = async (a) => {
  resolviendo.value = a.id
  try {
    await alertService.resolve(a.id)
    alertas.value = alertas.value.filter((x) => x.id !== a.id)
  } catch {
    error.value = 'Error al resolver la alerta'
  } finally {
    resolviendo.value = null
  }
}

watch(filtro, cargarAlertas)

onMounted(cargarAlertas)
</script>

<style scoped>
.alerta__tipo {
  margin: 0;
  font-size: 0.9375rem;
  font-weight: 600;
  color: var(--app-text-strong);
}
</style>
