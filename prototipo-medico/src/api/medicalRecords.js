import api from './axios'

export const medicalRecordService = {
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/medical-record/by-appointment/${appointmentId}`)
  },
  getByPatient(patientId) {
    return api.get(`/api/v1/medical-record/by-patient/${patientId}`)
  },
  getById(id) {
    return api.get(`/api/v1/medical-record/${id}`)
  },
  create(data) {
    return api.post('/api/v1/medical-record', data)
  },
  update(id, data) {
    return api.put(`/api/v1/medical-record/${id}`, data)
  }
}
