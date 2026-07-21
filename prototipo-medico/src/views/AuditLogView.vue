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
        <div class="row g-2 align-items-end">
          <div class="col-md-3">
            <label class="form-label fw-medium">👤 Usuario ID</label>
            <input v-model="filtros.userId" type="text" class="form-control form-filter" placeholder="ID del usuario">
          </div>
          <div class="col-md-2">
            <label class="form-label fw-medium">⚡ Acción</label>
            <input v-model="filtros.action" type="text" class="form-control form-filter" placeholder="Ej: Login">
          </div>
          <div class="col-md-2">
            <label class="form-label fw-medium">📅 Desde</label>
            <input v-model="filtros.from" type="date" class="form-control form-filter">
          </div>
          <div class="col-md-2">
            <label class="form-label fw-medium">📅 Hasta</label>
            <input v-model="filtros.to" type="date" class="form-control form-filter">
          </div>
          <div class="col-md-3 d-flex gap-2">
            <button @click="cargarLogs" class="btn btn-dark rounded-pill px-4">🔍 Filtrar</button>
            <button @click="limpiarFiltros" class="btn btn-outline-secondary rounded-pill px-3">Limpiar</button>
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
              <th class="pe-4 py-3 fw-medium">Recurso</th>
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
                <td class="pe-4 py-3">
                  <span v-if="log.action === 'Login'" class="text-muted small">Cuenta de {{ log.userName }}</span>
                  <code v-else class="small text-muted" :title="log.entityId">{{ log.entityId?.slice(0, 8) }}…</code>
                </td>
              </tr>
              <tr v-if="detalleId === log.id">
                <td colspan="5" class="p-4 bg-light">
                  <div class="d-flex justify-content-between align-items-start">
                    <div class="w-100">
                      <h6 class="fw-bold mb-3">📋 Detalle de la acción</h6>
                      <div class="detail-grid">
                        <div class="detail-item">
                          <span class="detail-label">🆔 Registro</span>
                          <span class="detail-value">#{{ log.id }}</span>
                        </div>
                        <div class="detail-item">
                          <span class="detail-label">👤 Usuario</span>
                          <span class="detail-value">{{ log.userName }}</span>
                          <span class="detail-raw-id">{{ log.userId }}</span>
                        </div>
                        <div class="detail-item">
                          <span class="detail-label">🎭 Rol</span>
                          <span class="detail-value">{{ log.userRole }}</span>
                        </div>
                        <div class="detail-item">
                          <span class="detail-label">⚡ Acción</span>
                          <span class="detail-value">
                            <StatusBadge :text="log.action" variant="info" />
                          </span>
                        </div>
                        <div class="detail-item">
                          <span class="detail-label">📎 Recurso afectado</span>
                          <span class="detail-value">{{ descripcionEntidad(log) }}</span>
                          <span v-if="log.entityId" class="detail-raw-id">{{ log.entityId }}</span>
                        </div>
                        <div class="detail-item col-12" v-if="log.details">
                          <span class="detail-label">📄 Detalles adicionales</span>
                          <pre class="mt-2 mb-0 p-3 bg-white border rounded-3 small">{{ JSON.stringify(log.details, null, 2) }}</pre>
                        </div>
                      </div>
                    </div>
                    <button @click.stop="detalleId = null" class="btn btn-outline-secondary rounded-pill px-3 ms-3">✕ Cerrar</button>
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

const descripcionEntidad = (log) => {
  if (log.action === 'Login') return `Inicio de sesión de ${log.userName}`
  if (log.action === 'Create') return `Registro creado`
  if (log.action === 'Update') return `Registro actualizado`
  if (log.action === 'Delete') return `Registro eliminado`
  if (log.entityId) return log.entityId.slice(0, 20) + '…'
  return '—'
}

const formatFecha = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleString('es-ES', {
    day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
  })
}

onMounted(cargarLogs)
</script>

<style scoped>
.form-filter {
  border: 1px solid #d1d5db;
  background: #fff;
  padding: 10px 12px;
  border-radius: 8px;
  transition: all 0.2s ease;
}
.form-filter:focus {
  border-color: #0d6efd;
  box-shadow: 0 0 0 3px rgba(13, 110, 253, 0.15);
  background: #fff;
}
.form-filter::placeholder {
  color: #94a3b8;
  font-size: 0.9rem;
}
.form-label {
  font-size: 0.85rem;
  color: #334155;
  margin-bottom: 4px;
}
.detail-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 16px;
}
.detail-item {
  display: flex;
  flex-direction: column;
  gap: 2px;
}
.detail-label {
  font-size: 0.8rem;
  font-weight: 600;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.3px;
}
.detail-value {
  font-weight: 500;
  color: #1e293b;
}
.detail-raw-id {
  font-size: 0.75rem;
  color: #94a3b8;
  font-family: monospace;
  word-break: break-all;
}
</style>