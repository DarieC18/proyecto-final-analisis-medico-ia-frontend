import api from './axios'

export const accountService = {
  getAll() {
    return api.get('/api/Account')
  },
  create(data) {
    return api.post('/api/Account', data)
  },
  update(id, data) {
    return api.put(`/api/Account/${id}`, data)
  },
  toggleStatus(id) {
    return api.patch(`/api/Account/${id}/status`)
  },
  remove(id) {
    return api.delete(`/api/Account/${id}`)
  }
}
