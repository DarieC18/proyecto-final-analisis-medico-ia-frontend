<template>
  <div class="container">

    <div v-if="vistaActual === 'lista'" class="animation-fade">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="fw-bold mb-0">Gestión de Pacientes</h3>
          <p class="text-muted">Directorio general de pacientes registrados</p>
        </div>
        <button @click="vistaActual = 'crear'" class="btn btn-primary px-4 shadow-sm">+ Crear Paciente</button>
      </div>

      <div class="card shadow-sm mb-4 border-0">
        <div class="card-body p-3">
          <div class="d-flex gap-3">
            <div class="input-group">
              <span class="input-group-text bg-light border-0">🔍</span>
              <input v-model="busqueda" type="text" class="form-control bg-light border-0" placeholder="Buscar por nombre completo o identificación...">
            </div>
          </div>
        </div>
      </div>

      <div v-if="pacientesFiltrados.length === 0" class="text-center py-5 text-muted">
        <p>No se encontraron pacientes.</p>
      </div>
      <div v-else class="card shadow-sm overflow-hidden border-0">
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
              <tr v-for="(p, idx) in pacientesFiltrados" :key="idx">
                <td class="ps-4 py-3 fw-bold text-dark">{{ p.nombre }}</td>
                <td class="py-3 text-muted">{{ p.identificacion }}</td>
                <td class="py-3 text-muted">{{ p.fechaNacimiento }}</td>
                <td class="py-3 text-muted">{{ p.telefono }}</td>
                <td class="py-3"><span class="badge bg-secondary bg-opacity-10 text-secondary rounded-pill px-3 py-2">{{ p.genero }}</span></td>
                <td class="py-3 text-muted">{{ p.registro }}</td>
                <td class="pe-4 py-3 text-end">
                  <button @click="editarPaciente(p)" class="btn btn-sm btn-light border text-warning fw-medium px-3 me-2">Editar</button>
                  <button @click="confirmarEliminar(p)" class="btn btn-sm btn-light border text-danger fw-medium px-3">Eliminar</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <div v-else-if="vistaActual === 'crear' || vistaActual === 'editar'" class="animation-fade">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="fw-bold mb-0">{{ vistaActual === 'crear' ? 'Nuevo Paciente' : 'Editar Paciente' }}</h3>
          <p v-if="vistaActual === 'editar'" class="text-muted">Modificando datos de: <strong class="text-dark">{{ form.nombre }}</strong></p>
        </div>
        <button @click="vistaActual = 'lista'" class="btn btn-light border text-muted shadow-sm">Volver al listado</button>
      </div>

      <div class="card shadow-sm border-0 rounded-4">
        <div class="card-body p-5">
          <form @submit.prevent="guardarPaciente">
            <div class="row g-4">
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Nombre Completo</label>
                <input v-model="form.nombre" type="text" class="form-control bg-light border-0 py-2" required>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Identificación</label>
                <input v-model="form.identificacion" type="text" class="form-control bg-light border-0 py-2" required>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Fecha de Nacimiento</label>
                <input v-model="form.fechaNacimiento" type="date" class="form-control bg-light border-0 py-2" required>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Género</label>
                <select v-model="form.genero" class="form-select bg-light border-0 py-2" required>
                  <option value="Masculino">Masculino</option>
                  <option value="Femenino">Femenino</option>
                  <option value="Otro">Otro</option>
                </select>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Teléfono</label>
                <input v-model="form.telefono" type="text" class="form-control bg-light border-0 py-2" required>
              </div>
            </div>
            <div class="d-flex justify-content-end gap-3 mt-5">
              <button type="button" @click="vistaActual = 'lista'" class="btn btn-light border px-4 py-2">Cancelar</button>
              <button type="submit" class="btn btn-primary px-4 py-2 shadow-sm">Guardar Cambios</button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <ConfirmDialog
      :visible="deleteDialog"
      title="Eliminar Paciente"
      message="¿Está seguro que desea eliminar este paciente y todo su historial clínico? Esta acción no se puede deshacer."
      confirmText="Eliminar"
      :danger="true"
      @confirm="eliminarPaciente"
      @cancel="deleteDialog = false"
    />
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import ConfirmDialog from '@/components/ConfirmDialog.vue'

const busqueda = ref('')
const vistaActual = ref('lista')
const deleteDialog = ref(false)
const deleteTarget = ref(null)
const editIndex = ref(-1)

const pacientes = ref([
  { nombre: 'Juan Pérez', identificacion: '402-1234567-8', fechaNacimiento: '12 Abr 1985', telefono: '809-555-1234', genero: 'Masculino', registro: '15 Jun 2026' },
  { nombre: 'María Gómez', identificacion: '001-2345678-9', fechaNacimiento: '22 Oct 1990', telefono: '809-555-5678', genero: 'Femenino', registro: '14 Jun 2026' },
  { nombre: 'Luis Medina', identificacion: '402-9876543-2', fechaNacimiento: '05 Mar 1978', telefono: '829-555-9012', genero: 'Masculino', registro: '12 Jun 2026' },
  { nombre: 'Ana Rodríguez', identificacion: '001-8765432-1', fechaNacimiento: '18 Jul 1995', telefono: '849-555-3456', genero: 'Femenino', registro: '10 Jun 2026' },
  { nombre: 'Carlos Santana', identificacion: '402-6543210-5', fechaNacimiento: '30 Ene 1982', telefono: '809-555-7890', genero: 'Masculino', registro: '08 Jun 2026' }
])

const form = reactive({
  nombre: '', identificacion: '', fechaNacimiento: '', telefono: '', genero: 'Masculino'
})

const pacientesFiltrados = computed(() => {
  if (!busqueda.value) return pacientes.value
  const q = busqueda.value.toLowerCase()
  return pacientes.value.filter(p =>
    p.nombre.toLowerCase().includes(q) || p.identificacion.includes(q)
  )
})

const editarPaciente = (p) => {
  editIndex.value = pacientes.value.indexOf(p)
  Object.assign(form, { ...p })
  vistaActual.value = 'editar'
}

const guardarPaciente = () => {
  if (vistaActual.value === 'crear') {
    pacientes.value.push({
      ...form,
      registro: new Date().toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' })
    })
  } else if (editIndex.value >= 0) {
    pacientes.value[editIndex.value] = { ...form, registro: pacientes.value[editIndex.value].registro }
  }
  vistaActual.value = 'lista'
}

const confirmarEliminar = (p) => {
  deleteTarget.value = p
  deleteDialog.value = true
}

const eliminarPaciente = () => {
  const idx = pacientes.value.indexOf(deleteTarget.value)
  if (idx >= 0) pacientes.value.splice(idx, 1)
  deleteDialog.value = false
}
</script>

<style scoped>
.animation-fade { animation: fadeIn 0.3s ease-in-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
input:focus, select:focus { background-color: #fff !important; box-shadow: 0 0 0 0.25rem rgba(14, 165, 233, 0.25); }
</style>
