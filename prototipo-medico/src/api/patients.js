import api from './axios'

export const patientService = {
  getAll() {
    return api.get('/api/v1/patient')
  },
  search(query) {
    return api.get('/api/v1/patient/search', { params: { query } })
  },
  getById(id) {
    return api.get(`/api/v1/patient/${id}`)
  },
  getDetails(id) {
    return api.get(`/api/v1/patient/${id}/details`)
  },
  create(data) {
    return api.post('/api/v1/patient/create', data)
  },
  update(id, data) {
    return api.put(`/api/v1/patient/${id}`, data)
  },
  deactivate(id) {
    return api.patch(`/api/v1/patient/${id}/deactivate`)
  },
  remove(id) {
    return api.delete(`/api/v1/patient/${id}`)
  }
}
