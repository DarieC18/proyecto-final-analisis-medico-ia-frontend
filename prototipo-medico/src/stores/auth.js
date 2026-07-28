import { reactive } from 'vue'

const userData = localStorage.getItem('user')
const parsed = userData ? JSON.parse(userData) : null

export const authStore = reactive({
  isAuthenticated: !!localStorage.getItem('accessToken'),
  user: parsed,
  token: localStorage.getItem('accessToken') || '',

  setSession(response) {
    this.token = response.accessToken || ''
    this.user = {
      id: response.id || '',
      name: response.name || '',
      lastName: response.lastName || '',
      roles: response.roles || [],
      email: response.email || ''
    }
    localStorage.setItem('accessToken', this.token)
    localStorage.setItem('user', JSON.stringify(this.user))
    this.isAuthenticated = true
  },

  setUserFromMe(userDto) {
    this.user = {
      id: userDto.id || '',
      name: userDto.name || '',
      lastName: userDto.lastName || '',
      roles: [userDto.role || ''],
      email: userDto.email || '',
      userName: userDto.userName || '',
      numberIdentification: userDto.numberIdentification || '',
      isVerified: userDto.isVerified || false,
      status: userDto.status || false,
      createdAt: userDto.createdAt || ''
    }
    localStorage.setItem('user', JSON.stringify(this.user))
  },

  hasRole(role) {
    return this.user?.roles?.includes(role) || false
  },

  isAdmin() {
    return this.hasRole('Administrator')
  },

  isDoctor() {
    return this.hasRole('Doctor')
  },

  logout() {
    this.token = ''
    this.user = null
    this.isAuthenticated = false
    localStorage.removeItem('accessToken')
    localStorage.removeItem('user')
  }
})
