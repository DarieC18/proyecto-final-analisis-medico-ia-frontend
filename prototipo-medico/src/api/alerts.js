import api from './axios'

export const alertService = {
  getByPatient(patientId) {
    return api.get(`/api/v1/alert/by-patient/${patientId}`)
  },
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/alert/by-appointment/${appointmentId}`)
  },
  getActive() {
    return api.get('/api/v1/alert/active')
  },
  resolve(id) {
    return api.patch(`/api/v1/alert/${id}/resolve`)
  }
}
