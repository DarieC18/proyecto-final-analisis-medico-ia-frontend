import api from './axios'

export const accountService = {
  getAll() {
    return api.get('/api/v1/account')
  },
  create(data) {
    return api.post('/api/v1/account', data)
  },
  update(id, data) {
    return api.put(`/api/v1/account/${id}`, data)
  },
  toggleStatus(id) {
    return api.patch(`/api/v1/account/${id}/status`)
  },
  remove(id) {
    return api.delete(`/api/v1/account/${id}`)
  }
}
