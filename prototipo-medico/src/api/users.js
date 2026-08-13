import api from './axios'

export const userService = {
  getDoctors() {
    return api.get('/api/v1/Auth/users/by-role/Doctor')
  }
}
