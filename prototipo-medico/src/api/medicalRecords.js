import api from './axios'

export const medicalRecordService = {
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/MedicalRecord/by-appointment/${appointmentId}`)
  },
  getByPatient(patientId) {
    return api.get(`/api/v1/MedicalRecord/by-patient/${patientId}`)
  },
  getDetailByPatient(patientId) {
    return api.get(`/api/v1/MedicalRecord/detail-by-patient/${patientId}`)
  },
  getById(id) {
    return api.get(`/api/v1/MedicalRecord/${id}`)
  },
  create(data) {
    return api.post('/api/v1/MedicalRecord', data)
  },
  update(id, data) {
    return api.put(`/api/v1/MedicalRecord/${id}`, data)
  }
}
