import api from './axios'

export const medicalDocumentService = {
  getByPatient(patientId) {
    return api.get(`/api/v1/medical-document/by-patient/${patientId}`)
  },
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/medical-document/by-appointment/${appointmentId}`)
  },
  getById(id) {
    return api.get(`/api/v1/medical-document/${id}`)
  },
  upload(data) {
    return api.post('/api/v1/medical-document/upload', data, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
  },
  remove(id) {
    return api.delete(`/api/v1/medical-document/${id}`)
  }
}
