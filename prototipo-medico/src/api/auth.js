import api from './axios'

export const authService = {
  login(userName, password) {
    return api.post('/api/v1/Auth/login', { userName, password })
  },
  register(data) {
    return api.post('/api/v1/Auth/register', data)
  },
  me() {
    return api.get('/api/v1/Auth/me')
  },
  forgotPassword(userName) {
    return api.post('/api/v1/Auth/forgot-password', { userName })
  },
  resetPassword(id, token, password) {
    return api.post('/api/v1/Auth/reset-password', { id, token, password })
  }
}
