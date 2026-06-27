import api from './axios'

export const dashboardService = {
  getStats() {
    return api.get('/api/AdminDashboard')
  }
}
