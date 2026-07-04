<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h2 class="fw-bold mb-0">Hola, Dr. Ruiz 👋</h2>
        <p class="text-muted">Resumen de tu jornada clínica de hoy</p>
      </div>
      <p class="text-muted fw-medium border bg-white px-3 py-2 rounded-pill shadow-sm">15 de Junio, 2026</p>
    </div>

    <div class="row g-4 mb-5">
      <div class="col-md-3">
        <div class="card shadow-sm h-100 border-start border-4 border-info">
          <div class="card-body">
            <h6 class="text-muted fw-bold text-uppercase mb-2">Total Pacientes</h6>
            <h2 class="fw-bold text-dark mb-0">142</h2>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card shadow-sm h-100 border-start border-4 border-success">
          <div class="card-body">
            <h6 class="text-muted fw-bold text-uppercase mb-2">Citas del Día</h6>
            <h2 class="fw-bold text-dark mb-0">8</h2>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card shadow-sm h-100 border-start border-4 border-warning">
          <div class="card-body">
            <h6 class="text-muted fw-bold text-uppercase mb-2">Análisis IA Pendientes</h6>
            <h2 class="fw-bold text-dark mb-0">3</h2>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card shadow-sm h-100 border-start border-4 border-danger">
          <div class="card-body">
            <h6 class="text-muted fw-bold text-uppercase mb-2">Alertas Activas</h6>
            <h2 class="fw-bold text-danger mb-0">2</h2>
          </div>
        </div>
      </div>
    </div>
    
    <div class="bg-white p-4 rounded-4 shadow-sm">
      <h5 class="fw-bold mb-4">Accesos Rápidos</h5> 
      <div class="d-flex gap-3">
        <RouterLink to="/pacientes" class="btn btn-light border px-4 py-2 text-primary fw-medium"><i class="bi bi-people"></i> Gestión de Pacientes</RouterLink> 
        <RouterLink to="/citas" class="btn btn-light border px-4 py-2 text-success fw-medium"><i class="bi bi-calendar"></i> Agendar Nueva Cita</RouterLink> 
      </div>
    </div>
  </div>
</template>

<script setup>
</script>