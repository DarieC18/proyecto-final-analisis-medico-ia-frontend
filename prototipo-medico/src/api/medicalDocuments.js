import api from './axios'

export const medicalDocumentService = {
  getByPatient(patientId) {
    return api.get(`/api/v1/medicalDocument/by-patient/${patientId}`)
  },
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/medicalDocument/by-appointment/${appointmentId}`)
  },
  getById(id) {
    return api.get(`/api/v1/medicalDocument/${id}`)
  },
  upload(data) {
    return api.post('/api/v1/medicalDocument/upload', data, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
  },
  remove(id) {
    return api.delete(`/api/v1/medicalDocument/${id}`)
  }
}
