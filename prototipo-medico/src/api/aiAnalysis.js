import api from './axios'

export const aiAnalysisService = {
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/aiAnalysis/by-appointment/${appointmentId}`)
  },
  getByPatient(patientId) {
    return api.get(`/api/v1/aiAnalysis/by-patient/${patientId}`)
  },
  getById(id) {
    return api.get(`/api/v1/aiAnalysis/${id}`)
  },
  generate(data) {
    return api.post('/api/v1/aiAnalysis/generate', data)
  },
  markAsReviewed(id) {
    return api.patch(`/api/v1/aiAnalysis/${id}/review`)
  }
}
