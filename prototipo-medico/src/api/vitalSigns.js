import api from './axios'

export const vitalSignService = {
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/VitalSign/by-appointment/${appointmentId}`)
  },
  getByPatient(patientId) {
    return api.get(`/api/v1/VitalSign/by-patient/${patientId}`)
  },
  create(data) {
    return api.post('/api/v1/VitalSign', data)
  },
  createFollowUp(data) {
    return api.post('/api/v1/VitalSign/follow-up', data)
  },
  getById(id) {
    return api.get(`/api/v1/VitalSign/${id}`)
  },
  update(id, data) {
    return api.put(`/api/v1/VitalSign/${id}`, data)
  }
}
