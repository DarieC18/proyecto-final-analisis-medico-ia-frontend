import axios from 'axios'

const api = axios.create({
  baseURL: '',
  timeout: 15000,
  headers: { 'Content-Type': 'application/json' }
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('accessToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  response => response,
  error => {
    if (error.code === 'ECONNABORTED') {
      return Promise.reject(new Error('La solicitud tardó demasiado. Intenta de nuevo.'))
    }
    if (error.response?.status === 401 && !window.location.pathname.includes('/login')) {
      localStorage.removeItem('accessToken')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default api
