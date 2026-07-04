<template>
  <div class="container">
    
    <div v-if="vistaActual === 'lista'" class="animation-fade">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="fw-bold mb-0">Gestión de Pacientes</h3>
          <p class="text-muted">Directorio general de pacientes registrados</p>
        </div>
        <button class="btn btn-primary px-4 shadow-sm">+ Crear Paciente</button>
      </div>

      <div class="card shadow-sm mb-4 border-0">
        <div class="card-body p-3">
          <form class="d-flex gap-3" @submit.prevent>
            <div class="input-group">
              <span class="input-group-text bg-light border-0"><i class="bi bi-search"></i>🔍</span>
              <input type="text" class="form-control bg-light border-0" placeholder="Buscar por nombre completo o identificación...">
            </div>
            <button class="btn btn-dark px-4">Buscar</button>
          </form>
        </div>
      </div>

      <div class="card shadow-sm overflow-hidden border-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="bg-light text-muted">
              <tr>
                <th class="ps-4 py-3 fw-medium">Nombre Completo</th>
                <th class="py-3 fw-medium">Identificación</th>
                <th class="py-3 fw-medium">F. Nacimiento</th>
                <th class="py-3 fw-medium">Teléfono</th>
                <th class="py-3 fw-medium">Género</th>
                <th class="py-3 fw-medium">Registro</th>
                <th class="pe-4 py-3 fw-medium text-end">Acciones</th>
              </tr>
            </thead>
            <tbody class="border-top-0">
              <tr>
                <td class="ps-4 py-3 fw-bold text-dark">Juan Pérez</td>
                <td class="py-3 text-muted">402-1234567-8</td>
                <td class="py-3 text-muted">12 Abr 1985</td>
                <td class="py-3 text-muted">809-555-1234</td>
                <td class="py-3"><span class="badge bg-secondary bg-opacity-10 text-secondary rounded-pill px-3 py-2">Masculino</span></td>
                <td class="py-3 text-muted">15 Jun 2026</td>
                <td class="pe-4 py-3 text-end">
                  <button @click="abrirEdicion" class="btn btn-sm btn-light border text-warning fw-medium px-3 me-2">Editar</button>
                  <button @click="abrirEliminacion" class="btn btn-sm btn-light border text-danger fw-medium px-3">Eliminar</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <div v-else-if="vistaActual === 'editar'" class="animation-fade">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="fw-bold mb-0">Editar Paciente</h3>
          <p class="text-muted">Modificando datos de: <strong class="text-dark">Juan Pérez</strong></p>
        </div>
        <button @click="volverALista" class="btn btn-light border text-muted shadow-sm">Volver al listado</button>
      </div>

      <div class="card shadow-sm border-0 rounded-4">
        <div class="card-body p-5">
          <form @submit.prevent="volverALista">
            <div class="row g-4">
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Nombre Completo</label>
                <input type="text" class="form-control bg-light border-0 py-2" value="Juan Pérez" required>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Identificación</label>
                <input type="text" class="form-control bg-light border-0 py-2" value="402-1234567-8" required>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Fecha de Nacimiento</label>
                <input type="date" class="form-control bg-light border-0 py-2" value="1985-04-12" required>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Género</label>
                <select class="form-select bg-light border-0 py-2" required>
                  <option value="Masculino" selected>Masculino</option>
                  <option value="Femenino">Femenino</option>
                  <option value="Otro">Otro</option>
                </select>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Teléfono</label>
                <input type="text" class="form-control bg-light border-0 py-2" value="809-555-1234" required>
              </div>
            </div>
            <div class="d-flex justify-content-end gap-3 mt-5">
              <button type="button" @click="volverALista" class="btn btn-light border px-4 py-2">Cancelar</button>
              <button type="submit" class="btn btn-primary px-4 py-2 shadow-sm">Guardar Cambios</button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <div v-else-if="vistaActual === 'eliminar'" class="animation-fade d-flex justify-content-center mt-5">
      <div class="card shadow border-0 rounded-4 text-center p-5" style="max-width: 500px;">
        <div class="mb-4">
          <div class="display-1 text-danger">🗑️</div>
        </div>
        <h4 class="fw-bold mb-3">Eliminar Paciente</h4>
        <p class="text-muted mb-4">¿Está seguro que desea eliminar este paciente y todo su historial clínico?  Esta acción no se puede deshacer.</p>
        <div class="d-flex justify-content-center gap-3">
          <button @click="volverALista" class="btn btn-light border px-4 py-2 fw-medium">Cancelar</button> [cite: 102]
          <button @click="volverALista" class="btn btn-danger px-4 py-2 fw-medium shadow-sm">Sí, Aceptar</button> [cite: 102]
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref } from 'vue'

const vistaActual = ref('lista')

const abrirEdicion = () => {
  vistaActual.value = 'editar'
}

const abrirEliminacion = () => {
  vistaActual.value = 'eliminar'
}

const volverALista = () => {
  vistaActual.value = 'lista'
}
</script>

<style scoped>
.animation-fade {
  animation: fadeIn 0.3s ease-in-out;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

input:focus, select:focus {
  background-color: #fff !important;
  box-shadow: 0 0 0 0.25rem rgba(14, 165, 233, 0.25);
}
</style>