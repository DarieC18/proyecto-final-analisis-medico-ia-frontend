import api from './axios'

export const recommendationService = {
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/recommendation/by-appointment/${appointmentId}`)
  },
  getByPatient(patientId) {
    return api.get(`/api/v1/recommendation/by-patient/${patientId}`)
  },
  getById(id) {
    return api.get(`/api/v1/recommendation/${id}`)
  }
}
