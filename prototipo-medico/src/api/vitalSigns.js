import api from './axios'

export const vitalSignService = {
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/VitalSign/by-appointment/${appointmentId}`)
  },
  getById(id) {
    return api.get(`/api/v1/VitalSign/${id}`)
  },
  create(data) {
    return api.post('/api/v1/VitalSign', data)
  },
  update(id, data) {
    return api.put(`/api/v1/VitalSign/${id}`, data)
  }
}
