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
            <tr>
              <td class="ps-4 py-3 fw-bold text-dark">Juan Pérez</td>
              <td class="py-3 text-muted">Dra. Ana López</td>
              <td class="py-3 text-muted">16 Jun 2026, 10:00 AM</td>
              <td class="py-3 text-muted">Dolor de cabeza severo</td>
              <td class="py-3"><span class="badge bg-warning bg-opacity-10 text-warning rounded-pill px-3 py-2 fw-bold">Pendiente</span></td>
              <td class="pe-4 py-3 text-end">
                <RouterLink to="/citas/detalle" class="btn btn-sm btn-light border text-primary fw-medium px-3">Ir al Detalle</RouterLink> 
              </td>
            </tr>
            <tr>
              <td class="ps-4 py-3 fw-bold text-dark">María Gómez</td>
              <td class="py-3 text-muted">Dr. Carlos Ruiz</td>
              <td class="py-3 text-muted">15 Jun 2026, 02:30 PM</td>
              <td class="py-3 text-muted">Chequeo general anual</td>
              <td class="py-3"><span class="badge bg-success bg-opacity-10 text-success rounded-pill px-3 py-2 fw-bold">Completada</span></td>
              <td class="pe-4 py-3 text-end">
                <RouterLink to="/citas/detalle" class="btn btn-sm btn-light border text-primary fw-medium px-3">Ir al Detalle</RouterLink>
              </td>
            </tr>
             <tr>
              <td class="ps-4 py-3 fw-bold text-dark">Luis Medina</td>
              <td class="py-3 text-muted">Dra. Ana López</td>
              <td class="py-3 text-muted">14 Jun 2026, 09:00 AM</td>
              <td class="py-3 text-muted">Lectura de análisis</td>
              <td class="py-3"><span class="badge bg-danger bg-opacity-10 text-danger rounded-pill px-3 py-2 fw-bold">Cancelada</span></td>
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
</script>