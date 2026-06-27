import api from './axios'

export const auditLogService = {
  getAll() {
    return api.get('/api/AuditLog')
  },
  getById(id) {
    return api.get(`/api/AuditLog/${id}`)
  },
  getFiltered(params) {
    return api.get('/api/AuditLog/filter', { params })
  }
}
