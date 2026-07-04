<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Registro de Auditoría</h3>
        <p class="text-muted">Trazabilidad de acciones en el sistema</p>
      </div>
    </div>

    <div class="card shadow-sm mb-4 border-0">
      <div class="card-body p-3">
        <div class="row g-2">
          <div class="col-md-3">
            <label class="form-label text-muted small fw-bold text-uppercase">Usuario</label>
            <input v-model="filtros.userId" type="text" class="form-control bg-light border-0" placeholder="ID de usuario">
          </div>
          <div class="col-md-2">
            <label class="form-label text-muted small fw-bold text-uppercase">Acción</label>
            <input v-model="filtros.action" type="text" class="form-control bg-light border-0" placeholder="Ej: Login">
          </div>
          <div class="col-md-2">
            <label class="form-label text-muted small fw-bold text-uppercase">Desde</label>
            <input v-model="filtros.from" type="date" class="form-control bg-light border-0">
          </div>
          <div class="col-md-2">
            <label class="form-label text-muted small fw-bold text-uppercase">Hasta</label>
            <input v-model="filtros.to" type="date" class="form-control bg-light border-0">
          </div>
          <div class="col-md-3 d-flex align-items-end gap-2">
            <button @click="cargarLogs" class="btn btn-dark px-4">Filtrar</button>
            <button @click="limpiarFiltros" class="btn btn-light border px-3">Limpiar</button>
          </div>
        </div>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="logs.length === 0" class="text-center py-5 text-muted">
      <p>No hay registros de auditoría.</p>
    </div>

    <div v-else class="card shadow-sm overflow-hidden border-0">
      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-dark">
            <tr>
              <th class="ps-4 py-3 fw-medium">Fecha</th>
              <th class="py-3 fw-medium">Usuario</th>
              <th class="py-3 fw-medium">Rol</th>
              <th class="py-3 fw-medium">Acción</th>
              <th class="py-3 fw-medium">Entidad</th>
              <th class="pe-4 py-3 fw-medium">ID Entidad</th>
            </tr>
          </thead>
          <tbody class="border-top-0">
            <template v-for="log in logs" :key="log.id">
              <tr @click="toggleDetalle(log)" class="cursor-pointer" style="cursor: pointer;">
                <td class="ps-4 py-3 text-muted">{{ formatFecha(log.createdAt) }}</td>
                <td class="py-3 fw-bold text-dark">{{ log.userName }}</td>
                <td class="py-3">
                  <span class="badge bg-info bg-opacity-10 text-info rounded-pill px-3 py-2">{{ log.userRole }}</span>
                </td>
                <td class="py-3">
                  <StatusBadge :text="log.action" variant="info" />
                </td>
                <td class="py-3 text-muted">{{ log.entityName }}</td>
                <td class="pe-4 py-3 text-muted small">{{ log.entityId }}</td>
              </tr>
              <tr v-if="detalleId === log.id">
                <td colspan="6" class="p-4 bg-light">
                  <div class="d-flex justify-content-between align-items-start">
                    <div>
                      <h6 class="fw-bold mb-3">Detalle completo del registro</h6>
                      <div class="row g-3">
                        <div class="col-md-4"><strong>ID Registro:</strong> {{ log.id }}</div>
                        <div class="col-md-4"><strong>Usuario ID:</strong> {{ log.userId }}</div>
                        <div class="col-md-4"><strong>Rol:</strong> {{ log.userRole }}</div>
                        <div class="col-md-4"><strong>Acción:</strong> {{ log.action }}</div>
                        <div class="col-md-4"><strong>Entidad:</strong> {{ log.entityName }}</div>
                        <div class="col-md-4"><strong>ID Entidad:</strong> {{ log.entityId }}</div>
                        <div class="col-md-12" v-if="log.details">
                          <strong>Detalles adicionales:</strong>
                          <pre class="mt-2 mb-0 p-3 bg-white rounded-3 small">{{ JSON.stringify(log.details, null, 2) }}</pre>
                        </div>
                      </div>
                    </div>
                    <button @click.stop="detalleId = null" class="btn btn-sm btn-light border px-3">Cerrar</button>
                  </div>
                </td>
              </tr>
            </template>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { auditLogService } from '@/api/auditLog'
import StatusBadge from '@/components/StatusBadge.vue'

const loading = ref(true)
const logs = ref([])
const detalleId = ref(null)

const toggleDetalle = (log) => {
  detalleId.value = detalleId.value === log.id ? null : log.id
}

const filtros = reactive({
  userId: '', action: '', from: '', to: ''
})

const cargarLogs = async () => {
  loading.value = true
  try {
    const params = {}
    if (filtros.userId) params.userId = filtros.userId
    if (filtros.action) params.action = filtros.action
    if (filtros.from) params.from = filtros.from
    if (filtros.to) params.to = filtros.to

    const hasFilters = Object.keys(params).length > 0
    let res
    if (hasFilters) {
      res = await auditLogService.getFiltered(params)
    } else {
      res = await auditLogService.getAll()
    }
    logs.value = res.data || []
  } catch (err) {
    if (err.response?.status === 204) {
      logs.value = []
    }
  } finally {
    loading.value = false
  }
}

const limpiarFiltros = () => {
  filtros.userId = ''
  filtros.action = ''
  filtros.from = ''
  filtros.to = ''
  cargarLogs()
}

const formatFecha = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleString('es-ES', {
    day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
  })
}

onMounted(cargarLogs)
</script>
