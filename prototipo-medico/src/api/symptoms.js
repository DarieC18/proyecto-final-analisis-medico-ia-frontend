import api from './axios'

export const symptomService = {
  getByAppointment(appointmentId) {
    return api.get(`/api/v1/symptom/by-appointment/${appointmentId}`)
  },
  getByPatient(patientId) {
    return api.get(`/api/v1/symptom/by-patient/${patientId}`)
  },
  getById(id) {
    return api.get(`/api/v1/symptom/${id}`)
  },
  create(data) {
    return api.post('/api/v1/symptom', data)
  },
  update(id, data) {
    return api.put(`/api/v1/symptom/${id}`, data)
  },
  remove(id) {
    return api.delete(`/api/v1/symptom/${id}`)
  }
}
