import api from './axios'

export const aiAnalysisService = {
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/ai-analysis/by-appointment/${appointmentId}`)
  },
  getByPatient(patientId) {
    return api.get(`/api/v1/ai-analysis/by-patient/${patientId}`)
  },
  getById(id) {
    return api.get(`/api/v1/ai-analysis/${id}`)
  },
  markAsReviewed(id) {
    return api.patch(`/api/v1/ai-analysis/${id}/review`)
  }
}
