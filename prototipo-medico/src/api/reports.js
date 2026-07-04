import api from './axios'

export const reportService = {
  getAppointmentReport(id) {
    return api.get(`/api/v1/report/appointment/${id}`)
  },
  getPatientReport(id) {
    return api.get(`/api/v1/report/patient/${id}`)
  },
  getAiAnalysesForReport(appointmentId) {
    return api.get(`/api/v1/report/ai-analyses/${appointmentId}`)
  },
  getRecommendationsForReport(patientId) {
    return api.get(`/api/v1/report/recommendations/${patientId}`)
  }
}
