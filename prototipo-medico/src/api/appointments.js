import api from './axios'

export const appointmentService = {
  getAll() {
    return api.get('/api/v1/appointment')
  },
  getById(id) {
    return api.get(`/api/v1/appointment/${id}`)
  },
  getDetail(id) {
    return api.get(`/api/v1/appointment/${id}/detail`)
  },
  create(data) {
    return api.post('/api/v1/appointment', data)
  },
  update(id, data) {
    return api.put(`/api/v1/appointment/${id}`, data)
  },
  changeStatus(id, status) {
    return api.patch(`/api/v1/appointment/${id}/status`, { status })
  }
}
