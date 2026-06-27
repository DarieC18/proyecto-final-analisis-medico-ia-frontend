<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Agenda de Citas</h3>
        <p class="text-muted">Gestiona las consultas médicas programadas</p>
      </div>
      <button class="btn btn-primary px-4 shadow-sm">+ Agendar Cita</button>
    </div>

    <div class="card shadow-sm overflow-hidden border-0">
      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="bg-light text-muted">
            <tr>
              <th class="ps-4 py-3 fw-medium">Paciente</th>
              <th class="py-3 fw-medium">Médico Asignado</th>
              <th class="py-3 fw-medium">Fecha de Cita</th>
              <th class="py-3 fw-medium">Motivo de Consulta</th>
              <th class="py-3 fw-medium">Estado</th>
              <th class="pe-4 py-3 fw-medium text-end">Acciones</th>
            </tr>
          </thead>
          <tbody class="border-top-0">
            <tr v-for="(c, idx) in citas" :key="idx">
              <td class="ps-4 py-3 fw-bold text-dark">{{ c.paciente }}</td>
              <td class="py-3 text-muted">{{ c.medico }}</td>
              <td class="py-3 text-muted">{{ c.fecha }}</td>
              <td class="py-3 text-muted">{{ c.motivo }}</td>
              <td class="py-3">
                <StatusBadge :text="c.estado" :variant="c.variant" />
              </td>
              <td class="pe-4 py-3 text-end">
                <RouterLink to="/citas/detalle" class="btn btn-sm btn-light border text-primary fw-medium px-3">Ir al Detalle</RouterLink>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import StatusBadge from '@/components/StatusBadge.vue'

const citas = ref([
  { paciente: 'Juan Pérez', medico: 'Dra. Ana López', fecha: '16 Jun 2026, 10:00 AM', motivo: 'Dolor de cabeza severo', estado: 'Pendiente', variant: 'pending' },
  { paciente: 'María Gómez', medico: 'Dr. Carlos Ruiz', fecha: '15 Jun 2026, 02:30 PM', motivo: 'Chequeo general anual', estado: 'Completada', variant: 'completed' },
  { paciente: 'Luis Medina', medico: 'Dra. Ana López', fecha: '14 Jun 2026, 09:00 AM', motivo: 'Lectura de análisis', estado: 'Cancelada', variant: 'cancelled' },
  { paciente: 'Ana Rodríguez', medico: 'Dr. Carlos Ruiz', fecha: '17 Jun 2026, 11:00 AM', motivo: 'Dolor lumbar crónico', estado: 'Pendiente', variant: 'pending' },
  { paciente: 'Carlos Santana', medico: 'Dra. Ana López', fecha: '18 Jun 2026, 08:30 AM', motivo: 'Control de glucosa', estado: 'Pendiente', variant: 'pending' }
])
</script>
