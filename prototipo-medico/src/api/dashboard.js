import api from './axios'

export const dashboardService = {
  getAdminStats() {
    return api.get('/api/v1/dashboard')
  },
  getDoctorStats() {
    return api.get('/api/v1/dashboard/doctor')
  }
}
