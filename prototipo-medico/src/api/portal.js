import api from './axios'

export const portalService = {
  getProfile() {
    return api.get('/api/v1/portal/profile')
  },
  updateProfile(data) {
    return api.patch('/api/v1/portal/profile', data)
  },
  getDoctors() {
    return api.get('/api/v1/portal/doctors')
  },
  getAppointments() {
    return api.get('/api/v1/portal/appointments')
  },
  requestAppointment(data) {
    return api.post('/api/v1/portal/appointments', data)
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
  getResults() {
    return api.get('/api/v1/portal/results')
  },
  getDocuments() {
    return api.get('/api/v1/portal/documents')
  }
}
