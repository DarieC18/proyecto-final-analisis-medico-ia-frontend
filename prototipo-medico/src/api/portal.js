import api from './axios'

export const portalService = {
  getProfile() {
    return api.get('/api/v1/portal/profile')
  },
  getAppointments() {
    return api.get('/api/v1/portal/appointments')
  },
  cancelAppointment(id) {
    return api.patch(`/api/v1/portal/appointments/${id}/cancel`)
  },
  getMedicalRecords() {
    return api.get('/api/v1/portal/medical-records')
  },
  getRecommendations() {
    return api.get('/api/v1/portal/recommendations')
  },
  getDocuments() {
    return api.get('/api/v1/portal/documents')
  },
  getResults() {
    return api.get('/api/v1/portal/results')
  },
  getDoctors() {
    return api.get('/api/v1/portal/doctors')
  }
}
