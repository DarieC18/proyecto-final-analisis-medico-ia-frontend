import api from './axios'

export const auditLogService = {
  getAll() {
    return api.get('/api/v1/AuditLog')
  },
  getById(id) {
    return api.get(`/api/v1/AuditLog/${id}`)
  },
  getFiltered(params) {
    return api.get('/api/v1/AuditLog/filter', { params })
  }
}
