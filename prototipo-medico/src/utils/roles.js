const ROLE_LABELS = {
  Administrator: 'Administrador',
  Doctor: 'Médico',
  Nurse: 'Enfermero',
  Patient: 'Paciente'
}

export const translateRole = (role) => ROLE_LABELS[role] ?? role