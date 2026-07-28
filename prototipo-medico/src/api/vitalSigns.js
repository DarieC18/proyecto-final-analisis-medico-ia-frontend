import api from './axios'

export const vitalSignService = {
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/vital-sign/by-appointment/${appointmentId}`)
  },
  getById(id) {
    return api.get(`/api/v1/vital-sign/${id}`)
  },
  create(data) {
    return api.post('/api/v1/vital-sign', data)
  },
  update(id, data) {
    return api.put(`/api/v1/vital-sign/${id}`, data)
  }
}
