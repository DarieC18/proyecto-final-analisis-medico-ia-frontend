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
    return api.post('/api/v1/patient', data)
  },
  update(id, data) {
    return api.put(`/api/v1/patient/${id}`, data)
  },
  remove(id) {
    return api.delete(`/api/v1/patient/${id}`)
  }
}
