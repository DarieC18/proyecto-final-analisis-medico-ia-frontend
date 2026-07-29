import api from './axios'

const splitFullName = (fullName) => {
  const idx = fullName.indexOf(' ')
  if (idx === -1) return { firstName: fullName, lastName: '' }
  return { firstName: fullName.slice(0, idx), lastName: fullName.slice(idx + 1) }
}

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
    const { fullName, identificationNumber, identificationType, ...rest } = data
    const { firstName, lastName } = splitFullName(fullName || '')
    return api.post('/api/v1/Patient/create', {
      firstName,
      lastName,
      numberIdentification: identificationNumber,
      ...rest
    })
  },
  update(id, data) {
    const { identificationType, email, ...rest } = data
    return api.put(`/api/v1/patient/${id}`, { id, ...rest })
  },
  remove(id) {
    return api.delete(`/api/v1/patient/${id}`)
  }
}
