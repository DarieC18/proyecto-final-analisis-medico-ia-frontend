<template>
  <div>
    <PageHeader
      title="Registro de auditoría"
      subtitle="Trazabilidad de acciones en el sistema"
      :icon="IconAudit"
      tone="warning"
    />

    <FilterBar>
      <div style="min-width: 200px; flex: 1 1 200px">
        <label for="al-user" class="form-label">Nombre de usuario</label>
        <input
          id="al-user"
          v-model="filtros.userName"
          type="search"
          class="form-control"
          placeholder="Nombre del usuario"
          @keyup.enter="aplicarFiltros"
        >
      </div>
      <div style="min-width: 180px; flex: 1 1 180px">
        <label for="al-action" class="form-label">Acción</label>
        <select id="al-action" v-model="filtros.action" class="form-select">
          <option value="">Todas</option>
          <option v-for="(label, key) in ACCIONES" :key="key" :value="key">{{ label }}</option>
        </select>
      </div>
      <div style="min-width: 150px">
        <label for="al-from" class="form-label">Desde</label>
        <input id="al-from" v-model="filtros.from" type="date" class="form-control">
      </div>
      <div style="min-width: 150px">
        <label for="al-to" class="form-label">Hasta</label>
        <input id="al-to" v-model="filtros.to" type="date" class="form-control">
      </div>
      <template #actions>
        <AppButton variant="primary" :icon="IconFilter" @click="aplicarFiltros">Filtrar</AppButton>
        <AppButton variant="soft" @click="limpiarFiltros">Limpiar</AppButton>
      </template>
    </FilterBar>

    <BaseCard flush padding="none">
      <LoadingState v-if="loading" label="Cargando registros…" />

      <EmptyState
        v-else-if="!logs.length"
        :icon="IconAudit"
        title="No hay registros de auditoría"
        message="Ninguna acción coincide con los filtros aplicados."
      />

      <template v-else>
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-head">
              <tr>
                <th scope="col">Fecha</th>
                <th scope="col">Usuario</th>
                <th scope="col">Rol</th>
                <th scope="col">Acción</th>
                <th scope="col">Recurso</th>
                <th scope="col" class="text-end pe-4"><span class="visually-hidden">Detalle</span></th>
              </tr>
            </thead>
            <tbody>
              <template v-for="log in logs" :key="log.id">
                <!-- La fila entera es un disparador: se usa <button> en la última
                     celda en lugar de @click en el <tr>, para que sea alcanzable
                     por teclado y se anuncie el estado expandido. -->
                <tr>
                  <td class="text-app-muted">{{ formatFecha(log.createdAt) }}</td>
                  <td class="fw-semibold text-body-emphasis">{{ log.userName }}</td>
                  <td><StatusBadge :text="translateRole(log.userRole)" variant="info" size="sm" /></td>
                  <td><StatusBadge :text="log.action" variant="secondary" size="sm" /></td>
                  <td>
                    <span v-if="log.action === 'Login'" class="text-app-muted small">
                      Cuenta de {{ log.userName }}
                    </span>
                    <code v-else class="small text-app-muted" :title="log.entityId">
                      {{ log.entityId?.slice(0, 8) }}…
                    </code>
                  </td>
                  <td class="text-end pe-4">
                    <AppButton
                      variant="ghost"
                      size="sm"
                      :icon="detalleId === log.id ? IconChevronDown : IconChevronRight"
                      :aria-expanded="detalleId === log.id"
                      :aria-label="detalleId === log.id ? 'Ocultar detalle' : 'Ver detalle'"
                      @click="toggleDetalle(log)"
                    />
                  </td>
                </tr>
                <tr v-if="detalleId === log.id">
                  <td colspan="6" class="audit-detail">
                    <div class="row g-3">
                      <div class="col-sm-6 col-lg-3">
                        <DataField label="Registro" :value="`#${log.id}`" />
                      </div>
                      <div class="col-sm-6 col-lg-3">
                        <DataField label="Usuario">
                          {{ log.userName }}
                          <span class="audit-detail__raw">{{ log.userId }}</span>
                        </DataField>
                      </div>
                      <div class="col-sm-6 col-lg-3">
                        <DataField label="Rol" :value="translateRole(log.userRole)" />
                      </div>
                      <div class="col-sm-6 col-lg-3">
                        <DataField label="Acción">
                          <StatusBadge :text="log.action" variant="info" size="sm" />
                        </DataField>
                      </div>
                      <div class="col-12">
                        <DataField label="Recurso afectado">
                          {{ descripcionEntidad(log) }}
                          <span v-if="log.entityId" class="audit-detail__raw">{{ log.entityId }}</span>
                        </DataField>
                      </div>
                      <div v-if="log.details" class="col-12">
                        <DataField label="Detalles adicionales">
                          <pre class="audit-detail__json">{{ JSON.stringify(log.details, null, 2) }}</pre>
                        </DataField>
                      </div>
                    </div>
                  </td>
                </tr>
              </template>
            </tbody>
          </table>
        </div>

        <div class="audit-pagination">
          <small class="text-app-muted">
            {{ totalCount }} registros · página {{ currentPage }} de {{ totalPages }}
          </small>
          <nav aria-label="Paginación de registros">
            <ul class="pagination pagination-sm mb-0">
              <li class="page-item" :class="{ disabled: currentPage === 1 }">
                <button class="page-link" :disabled="currentPage === 1" @click="irAPagina(currentPage - 1)">
                  Anterior
                </button>
              </li>
              <li
                v-for="p in paginasVisibles"
                :key="p"
                class="page-item"
                :class="{ active: p === currentPage }"
              >
                <button class="page-link" :aria-current="p === currentPage ? 'page' : undefined" @click="irAPagina(p)">
                  {{ p }}
                </button>
              </li>
              <li class="page-item" :class="{ disabled: currentPage >= totalPages }">
                <button class="page-link" :disabled="currentPage >= totalPages" @click="irAPagina(currentPage + 1)">
                  Siguiente
                </button>
              </li>
            </ul>
          </nav>
        </div>
      </template>
    </BaseCard>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import {
  AppButton,
  BaseCard,
  DataField,
  EmptyState,
  FilterBar,
  LoadingState,
  PageHeader,
  StatusBadge
} from '@/components/ui'
import { IconAudit, IconChevronDown, IconChevronRight, IconFilter } from '@/lib/icons'
import { auditLogService } from '@/api/auditLog'
import { translateRole } from '@/utils/roles'

// Un solo mapa acción -> etiqueta: antes las opciones del filtro y las
// descripciones del detalle eran dos listas separadas que podían divergir.
const ACCIONES = {
  Login: 'Inicio de sesión',
  CreateUser: 'Usuario creado por administrador',
  UpdateUser: 'Usuario actualizado',
  RegisterPatient: 'Registro público de paciente',
  ToggleStatus: 'Cambio de estado de cuenta',
  Delete: 'Cuenta eliminada',
  CreatePatient: 'Expediente de paciente creado',
  CreateAppointment: 'Cita registrada',
  GenerateAiAnalysis: 'Análisis IA generado',
  UploadDocument: 'Documento médico subido',
  DeletePatient: 'Paciente eliminado',
  DeleteAppointment: 'Cita eliminada'
}

const PAGE_SIZE = 20

const loading = ref(true)
const logs = ref([])
const detalleId = ref(null)
const currentPage = ref(1)
const totalCount = ref(0)

const filtros = reactive({ userName: '', action: '', from: '', to: '' })

const totalPages = computed(() => Math.ceil(totalCount.value / PAGE_SIZE) || 1)

const paginasVisibles = computed(() => {
  const delta = 2
  const range = []
  const start = Math.max(1, currentPage.value - delta)
  const end = Math.min(totalPages.value, currentPage.value + delta)
  for (let i = start; i <= end; i++) range.push(i)
  return range
})

const irAPagina = (p) => {
  currentPage.value = p
  detalleId.value = null
  cargarLogs()
}

const toggleDetalle = (log) => {
  detalleId.value = detalleId.value === log.id ? null : log.id
}

const cargarLogs = async () => {
  loading.value = true
  try {
    const params = { pageNumber: currentPage.value, pageSize: PAGE_SIZE }
    if (filtros.userName) params.userName = filtros.userName
    if (filtros.action) params.action = filtros.action
    if (filtros.from) params.from = filtros.from
    if (filtros.to) params.to = filtros.to

    const res = await auditLogService.getFiltered(params)
    logs.value = res.data?.items || []
    totalCount.value = res.data?.totalCount || 0
  } catch (err) {
    if (err.response?.status === 204) {
      logs.value = []
      totalCount.value = 0
    }
  } finally {
    loading.value = false
  }
}

const aplicarFiltros = () => {
  currentPage.value = 1
  cargarLogs()
}

const limpiarFiltros = () => {
  Object.assign(filtros, { userName: '', action: '', from: '', to: '' })
  currentPage.value = 1
  cargarLogs()
}

const descripcionEntidad = (log) => ACCIONES[log.action] ?? log.entityName ?? '—'

const formatFecha = (dateStr) => {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleString('es-ES', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

onMounted(cargarLogs)
</script>

<style scoped>
.audit-detail {
  background-color: var(--bs-tertiary-bg);
  padding: 1.25rem;
}

.audit-detail__raw {
  display: block;
  font-family: var(--bs-font-monospace);
  font-size: 0.7rem;
  font-weight: 400;
  color: var(--app-text-subtle);
  word-break: break-all;
}

.audit-detail__json {
  margin: 0.375rem 0 0;
  padding: 0.75rem;
  background-color: var(--app-surface);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius-sm);
  font-size: 0.75rem;
  color: var(--app-text);
  max-height: 320px;
  overflow: auto;
}

.audit-pagination {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1.25rem;
  border-top: 1px solid var(--app-border);
}
</style>
