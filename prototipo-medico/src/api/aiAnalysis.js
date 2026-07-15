import api from './axios'

export const aiAnalysisService = {
  generate(data) {
    return api.post('/api/v1/AiAnalysis/generate', data)
  },
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/AiAnalysis/by-appointment/${appointmentId}`)
  },
  getByPatient(patientId) {
    return api.get(`/api/v1/AiAnalysis/by-patient/${patientId}`)
  },
  getById(id) {
    return api.get(`/api/v1/AiAnalysis/${id}`)
  },
  markAsReviewed(id) {
    return api.patch(`/api/v1/AiAnalysis/${id}/review`)
  }
}
