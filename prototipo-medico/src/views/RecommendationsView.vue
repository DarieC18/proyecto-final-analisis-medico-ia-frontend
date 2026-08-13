<template>
  <div>
    <PageHeader
      title="Recomendaciones"
      subtitle="Recomendaciones generadas por el sistema por paciente"
      :icon="IconRecommendation"
    />

    <BaseCard class="mb-3">
      <div v-if="pacienteSeleccionado" class="d-flex align-items-center gap-2 flex-wrap">
        <StatusBadge
          :text="`${pacienteSeleccionado.fullName} · ${pacienteSeleccionado.identificationNumber}`"
          variant="primary"
          size="lg"
        />
        <AppButton variant="soft" size="sm" :icon="IconClose" @click="limpiarBusqueda">Cambiar</AppButton>
      </div>

      <div v-else>
        <div class="row g-2 align-items-end">
          <div class="col-md-6">
            <label for="r-busqueda" class="form-label">Buscar paciente</label>
            <input
              id="r-busqueda"
              v-model="busqueda"
              type="text"
              class="form-control"
              placeholder="Nombre o número de identificación"
              @keyup.enter="buscarPacientes"
            />
          </div>
          <div class="col-md-3">
            <AppButton
              variant="primary"
              block
              :icon="IconSearch"
              :loading="buscando"
              @click="buscarPacientes"
            >
              Buscar
            </AppButton>
          </div>
          <div class="col-md-3">
            <AppButton variant="soft" block @click="limpiarBusqueda">Limpiar</AppButton>
          </div>
        </div>

        <ul v-if="pacientesEncontrados.length" class="resultados">
          <li v-for="p in pacientesEncontrados" :key="p.id">
            <button type="button" class="resultados__item" @click="seleccionarPaciente(p)">
              <span class="fw-medium">{{ p.fullName }}</span>
              <small class="text-app-muted">{{ p.identificationNumber }}</small>
            </button>
          </li>
        </ul>

        <p v-else-if="buscado && !buscando" class="text-app-muted small mb-0 mt-2">
          No se encontraron pacientes.
        </p>
      </div>
    </BaseCard>

    <LoadingState v-if="loading" label="Cargando recomendaciones…" />

    <AppAlert v-else-if="error" variant="danger" :message="error" />

    <BaseCard v-else-if="recomendaciones.length === 0" flush padding="none">
      <EmptyState
        :icon="IconRecommendation"
        :title="pacienteSeleccionado ? 'Sin recomendaciones' : 'Busca un paciente'"
        :message="
          pacienteSeleccionado
            ? 'Este paciente todavía no tiene recomendaciones registradas.'
            : 'Busca por nombre o identificación para ver las recomendaciones de un paciente.'
        "
      />
    </BaseCard>

    <div v-else class="row g-3">
      <div v-for="r in recomendaciones" :key="r.id" class="col-md-6">
        <BaseCard class="h-100">
          <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
            <h3 class="rec__title">{{ r.title || 'Recomendación' }}</h3>
            <small class="text-app-muted text-nowrap">{{ formatDate(r.createdAt) }}</small>
          </div>
          <p v-if="r.description" class="rec__desc">{{ r.description }}</p>
          <div class="d-flex align-items-center gap-2 flex-wrap">
            <StatusBadge v-if="r.priority" :text="r.priority" :variant="priorityVariant(r.priority)" />
            <small v-if="r.appointmentId" class="text-app-muted">Relacionado a cita #{{ r.appointmentId }}</small>
          </div>
        </BaseCard>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { recommendationService } from '@/api/recommendations'
import { patientService } from '@/api/patients'
import {
  AppAlert,
  AppButton,
  BaseCard,
  EmptyState,
  LoadingState,
  PageHeader,
  StatusBadge
} from '@/components/ui'
import { IconClose, IconRecommendation, IconSearch } from '@/lib/icons'

const loading = ref(false)
const error = ref('')
const recomendaciones = ref([])
const busqueda = ref('')
const buscando = ref(false)
const buscado = ref(false)
const pacientesEncontrados = ref([])
const pacienteSeleccionado = ref(null)

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

const PRIORITY_VARIANTS = { Alta: 'severe', Media: 'moderate', Baja: 'low' }
const priorityVariant = (p) => PRIORITY_VARIANTS[p] ?? 'info'

const buscarPacientes = async () => {
  if (!busqueda.value.trim()) return
  buscando.value = true
  buscado.value = false
  pacientesEncontrados.value = []
  try {
    const res = await patientService.search(busqueda.value)
    pacientesEncontrados.value = res.data || []
  } catch {
    pacientesEncontrados.value = []
  } finally {
    buscado.value = true
    buscando.value = false
  }
}

const seleccionarPaciente = async (p) => {
  pacienteSeleccionado.value = p
  pacientesEncontrados.value = []
  loading.value = true
  error.value = ''
  try {
    const res = await recommendationService.getByPatient(p.id)
    recomendaciones.value = res.data || []
  } catch (err) {
    if (err.response?.status === 204 || err.response?.status === 404) {
      recomendaciones.value = []
    } else {
      error.value = 'Error al cargar recomendaciones'
    }
  } finally {
    loading.value = false
  }
}

const limpiarBusqueda = () => {
  busqueda.value = ''
  buscado.value = false
  pacientesEncontrados.value = []
  pacienteSeleccionado.value = null
  recomendaciones.value = []
}
</script>

<style scoped>
.resultados {
  list-style: none;
  margin: 0.5rem 0 0;
  padding: 0;
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius);
  overflow: hidden;
}

.resultados li + li {
  border-top: 1px solid var(--app-border);
}

/* <button> y no <div @click>: la lista es navegable con teclado y anunciable
   por lector de pantalla sin añadir roles ni tabindex a mano. */
.resultados__item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.75rem;
  width: 100%;
  padding: 0.5rem 0.875rem;
  border: 0;
  background: transparent;
  color: inherit;
  text-align: start;
  cursor: pointer;
}

.resultados__item:hover {
  background-color: var(--app-surface-sunken);
}

.resultados__item:focus-visible {
  outline: none;
  box-shadow: var(--app-ring);
}

.rec__title {
  margin: 0;
  font-size: 0.9375rem;
  font-weight: 600;
  color: var(--app-text-strong);
}

.rec__desc {
  margin: 0 0 0.5rem;
  font-size: 0.875rem;
  color: var(--app-text-muted);
}
</style>
