import api from './axios'

export const medicalRecordService = {
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/MedicalRecord/by-appointment/${appointmentId}`)
  },
  getByPatient(patientId) {
    return api.get(`/api/v1/MedicalRecord/by-patient/${patientId}`)
  },
  getById(id) {
    return api.get(`/api/v1/MedicalRecord/${id}`)
  },
  create(data) {
    const userData = localStorage.getItem('user')
    const user = userData ? JSON.parse(userData) : null
    return api.post('/api/v1/MedicalRecord', {
      id: 0,
      createdByUserId: user?.id || '',
      ...data
    })
  },
  update(id, data) {
    const userData = localStorage.getItem('user')
    const user = userData ? JSON.parse(userData) : null
    return api.put(`/api/v1/MedicalRecord/${id}`, {
      id: Number(id),
      createdByUserId: user?.id || '',
      ...data
    })
  }
}
