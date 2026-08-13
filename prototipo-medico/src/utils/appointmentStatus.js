/**
 * Estados de cita: traducción y tono visual.
 * Estos dos mapas estaban duplicados en varias vistas; al centralizarlos,
 * añadir un estado nuevo deja de requerir tocar cada una.
 */

const LABELS = {
  Pending: 'Pendiente',
  InProgress: 'En progreso',
  Completed: 'Completada',
  Cancelled: 'Cancelada'
}

const VARIANTS = {
  Pending: 'pending',
  InProgress: 'inprogress',
  Completed: 'completed',
  Cancelled: 'cancelled'
}

export const translateStatus = (status) => LABELS[status] ?? status ?? '—'
export const statusVariant = (status) => VARIANTS[status] ?? 'secondary'
