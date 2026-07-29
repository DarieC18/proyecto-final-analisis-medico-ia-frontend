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
            <div class="input-group flex-grow-1">
              <span class="input-group-text bg-light border-0">🔍</span>
              <input v-model="busqueda" type="text" class="form-control bg-light border-0" placeholder="Buscar por nombre completo o identificación..." @keyup.enter="buscarApi">
            </div>
            <button @click="buscarApi" class="btn btn-dark px-4" :disabled="buscandoApi">
              <span v-if="buscandoApi" class="spinner-border spinner-border-sm me-1"></span>
              Buscar
            </button>
            <button @click="limpiarBusqueda" class="btn btn-light border px-3" v-if="busqueda">Limpiar</button>
          </div>
        </div>
      </div>

      <div v-if="loading" class="text-center py-5">
        <div class="spinner-border text-primary" role="status"></div>
      </div>

      <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

      <div v-else-if="pacientesFiltrados.length === 0" class="text-center py-5 text-muted">
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
              <tr v-for="p in pacientesFiltrados" :key="p.id">
                <td class="ps-4 py-3 fw-bold text-dark">{{ p.fullName }}</td>
                <td class="py-3 text-muted">{{ p.identificationNumber }}</td>
                <td class="py-3 text-muted">{{ formatDate(p.birthDate) }}</td>
                <td class="py-3 text-muted">{{ p.phoneNumber }}</td>
                <td class="py-3"><span class="badge bg-secondary bg-opacity-10 text-secondary rounded-pill px-3 py-2">{{ p.gender }}</span></td>
                <td class="py-3 text-muted">{{ formatDate(p.createdAt) }}</td>
                <td class="pe-4 py-3 text-end">
                  <button @click="abrirDetalle(p)" class="btn btn-sm btn-light border text-info fw-medium px-3 me-2">Ver</button>
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
          <p v-if="vistaActual === 'editar'" class="text-muted">Modificando datos de: <strong class="text-dark">{{ form.fullName }}</strong></p>
        </div>
        <button @click="cancelarForm" class="btn btn-outline-secondary rounded-pill px-4 shadow-sm">
          ← Volver
        </button>
      </div>

      <div class="card shadow border-0 rounded-4 form-card">
        <div class="card-body p-4 p-lg-5">
          <div v-if="formError" class="alert alert-danger border-0 rounded-3 py-2 small mb-4">{{ formError }}</div>
          <form @submit.prevent="guardarPaciente">

            <div class="form-section mb-4">
              <div class="section-header">
                <span class="section-icon">🆔</span>
                <span>Identificación</span>
              </div>
              <div class="section-body">
                <div class="row g-3">
                  <div class="col-md-4">
                    <label class="form-label fw-medium">Tipo ID</label>
                    <select v-model="form.identificationType" class="form-select" required>
                      <option value="Cédula">Cédula</option>
                      <option value="Pasaporte">Pasaporte</option>
                    </select>
                  </div>
                  <div class="col-md-8">
                    <label class="form-label fw-medium">Número de Identificación</label>
                    <input v-model="form.identificationNumber" type="text" class="form-control" placeholder="Ej: 001-1234567-8" required>
                  </div>
                </div>
              </div>
            </div>

            <div class="form-section mb-4">
              <div class="section-header">
                <span class="section-icon">👤</span>
                <span>Datos Personales</span>
              </div>
              <div class="section-body">
                <div class="row g-3">
                  <div class="col-12">
                    <label class="form-label fw-medium">Nombre Completo</label>
                    <input v-model="form.fullName" type="text" class="form-control" placeholder="Nombre y apellidos del paciente" required>
                  </div>
                  <div class="col-md-6">
                    <label class="form-label fw-medium">Fecha de Nacimiento</label>
                    <input v-model="form.birthDate" type="date" class="form-control" required>
                  </div>
                  <div class="col-md-6">
                    <label class="form-label fw-medium">Género</label>
                    <select v-model="form.gender" class="form-select" required>
                      <option value="Masculino">Masculino</option>
                      <option value="Femenino">Femenino</option>
                      <option value="Otro">Otro</option>
                    </select>
                  </div>
                </div>
              </div>
            </div>

            <div class="form-section mb-4">
              <div class="section-header">
                <span class="section-icon">📞</span>
                <span>Contacto y Clasificación</span>
              </div>
              <div class="section-body">
                <div class="row g-3">
                  <div class="col-md-4">
                    <label class="form-label fw-medium">Email</label>
                    <input v-model="form.email" type="email" class="form-control" placeholder="correo@ejemplo.com" required>
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">Teléfono</label>
                    <input v-model="form.phoneNumber" type="tel" class="form-control" placeholder="(809) 555-1234" required>
                  </div>
                  <div class="col-md-4">
                    <label class="form-label fw-medium">Tipo de Paciente</label>
                    <select v-model="form.patientType" class="form-select" required>
                      <option value="Asegurado">Asegurado</option>
                      <option value="No Asegurado">No Asegurado</option>
                    </select>
                  </div>
                </div>
              </div>
            </div>

            <hr class="my-4">

            <div class="d-flex justify-content-end gap-3">
              <button type="button" @click="cancelarForm" class="btn btn-outline-secondary rounded-pill px-4 py-2">
                Cancelar
              </button>
              <button type="submit" class="btn btn-primary rounded-pill px-5 py-2 shadow" :disabled="saving">
                <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                {{ saving ? 'Guardando...' : '💾 Guardar Paciente' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <div v-else-if="vistaActual === 'detalle'" class="animation-fade">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="fw-bold mb-0">{{ detalle?.fullName }}</h3>
          <p class="text-muted">Expediente completo del paciente</p>
        </div>
        <button @click="cancelarForm" class="btn btn-light border text-muted shadow-sm">Volver al listado</button>
      </div>
      <div class="card shadow-sm border-0 rounded-4">
        <div class="card-body p-5">
          <div class="row g-4 mb-4">
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Identificación</label>
              <p class="fw-medium">{{ detalle?.identificationNumber }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Teléfono</label>
              <p class="fw-medium">{{ detalle?.phoneNumber }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Género</label>
              <p class="fw-medium">{{ detalle?.gender }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Fecha Nacimiento</label>
              <p class="fw-medium">{{ formatDate(detalle?.birthDate) }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Tipo Paciente</label>
              <p class="fw-medium">{{ detalle?.patientType }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Registrado</label>
              <p class="fw-medium">{{ formatDate(detalle?.createdAt) }}</p>
            </div>
          </div>

          <h5 class="fw-bold mt-5 mb-3">Historial de Citas</h5>
          <div v-if="detalle?.appointments?.length" class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="bg-light text-muted">
                <tr>
                  <th class="ps-4 py-3 fw-medium">Fecha</th>
                  <th class="py-3 fw-medium">Médico</th>
                  <th class="py-3 fw-medium">Motivo</th>
                  <th class="py-3 fw-medium">Estado</th>
                </tr>
              </thead>
              <tbody class="border-top-0">
                <tr v-for="r in detalle.appointments" :key="r.id">
                  <td class="ps-4 py-3">{{ formatDate(r.appointmentDate) }}</td>
                  <td class="py-3">{{ r.doctorName }}</td>
                  <td class="py-3">{{ r.reason }}</td>
                  <td class="py-3">
                    <StatusBadge :text="r.status" :variant="r.status?.toLowerCase()" />
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div v-else class="text-muted text-center py-4">No hay citas registradas para este paciente.</div>
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
import { ref, reactive, computed, onMounted } from 'vue'
import { patientService } from '@/api/patients'
import ConfirmDialog from '@/components/ConfirmDialog.vue'
import StatusBadge from '@/components/StatusBadge.vue'

const busqueda = ref('')
const vistaActual = ref('lista')
const loading = ref(true)
const error = ref('')
const saving = ref(false)
const formError = ref('')
const deleteDialog = ref(false)
const deleteTarget = ref(null)
const pacientes = ref([])
const detalle = ref(null)
const editId = ref(null)
const editUserId = ref(null)
const buscandoApi = ref(false)

const form = reactive({
  fullName: '', identificationNumber: '', identificationType: 'Cédula',
  birthDate: '', gender: 'Masculino', phoneNumber: '', email: '',
  patientType: 'Asegurado'
})

const pacientesFiltrados = computed(() => {
  let list = pacientes.value.filter(p => p.fullName)
  if (!busqueda.value) return list
  const q = busqueda.value.toLowerCase()
  return list.filter(p =>
    p.fullName.toLowerCase().includes(q) || p.identificationNumber?.includes(q)
  )
})

const formatDate = (dateStr) => {
  if (!dateStr || dateStr.startsWith('0001-01-01')) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' })
  } catch {
    return dateStr
  }
}

const cargarPacientes = async () => {
  loading.value = true
  error.value = ''
  try {
    const res = await patientService.getAll()
    pacientes.value = res.data || []
  } catch (err) {
    if (err.response?.status === 404 || err.response?.status === 204) {
      pacientes.value = []
    } else {
      error.value = 'Error al cargar pacientes'
    }
  } finally {
    loading.value = false
  }
}

const abrirDetalle = async (p) => {
  vistaActual.value = 'detalle'
  try {
    const res = await patientService.getDetails(p.id)
    detalle.value = { ...p, ...res.data }
  } catch {
    detalle.value = { ...p }
  }
}

const editarPaciente = async (p) => {
  editId.value = p.id
  try {
    const res = await patientService.getById(p.id)
    editUserId.value = res.data?.userId || p.userId
  } catch {
    editUserId.value = p.userId
  }
  Object.assign(form, {
    fullName: p.fullName || '',
    identificationNumber: p.identificationNumber || '',
    identificationType: p.identificationType || 'Cédula',
    birthDate: p.birthDate ? p.birthDate.substring(0, 10) : '',
    gender: p.gender || 'Masculino',
    phoneNumber: p.phoneNumber || '',
    email: p.email || '',
    patientType: p.patientType || 'Asegurado'
  })
  vistaActual.value = 'editar'
  formError.value = ''
}

const guardarPaciente = async () => {
  saving.value = true
  formError.value = ''
  try {
    if (vistaActual.value === 'crear') {
      await patientService.create({ ...form })
    } else {
      await patientService.update(editId.value, { ...form, userId: editUserId.value })
    }
    vistaActual.value = 'lista'
    await cargarPacientes()
  } catch (err) {
    const data = err.response?.data
    if (data?.errors && typeof data.errors === 'object') {
      formError.value = Object.values(data.errors).flat().join(', ')
    } else if (data?.errors && Array.isArray(data.errors)) {
      formError.value = data.errors.join(', ')
    } else {
      formError.value = data?.message || data?.detail || 'Error al guardar paciente'
    }
  } finally {
    saving.value = false
  }
}

const cancelarForm = () => {
  vistaActual.value = 'lista'
  detalle.value = null
  editId.value = null
  editUserId.value = null
  Object.assign(form, {
    fullName: '', identificationNumber: '', identificationType: 'Cédula',
    birthDate: '', gender: 'Masculino', phoneNumber: '', email: '',
    patientType: 'Asegurado'
  })
}

const confirmarEliminar = (p) => {
  deleteTarget.value = p
  deleteDialog.value = true
}

const buscarApi = async () => {
  if (!busqueda.value) {
    await cargarPacientes()
    return
  }
  buscandoApi.value = true
  try {
    const res = await patientService.search(busqueda.value)
    pacientes.value = res.data || []
  } catch (err) {
    error.value = 'Error en la búsqueda'
  } finally {
    buscandoApi.value = false
  }
}

const limpiarBusqueda = () => {
  busqueda.value = ''
  cargarPacientes()
}

const eliminarPaciente = async () => {
  deleteDialog.value = false
  try {
    await patientService.remove(deleteTarget.value.id)
    await cargarPacientes()
  } catch (err) {
    error.value = 'Error al eliminar paciente'
  }
}

onMounted(cargarPacientes)
</script>

<style scoped>
.animation-fade { animation: fadeIn 0.3s ease-in-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }

.form-card {
  border-top: 4px solid #0d6efd !important;
}

.form-section {
  background: #f8fafc;
  border: 1px solid #e9ecef;
  border-radius: 12px;
  overflow: hidden;
  transition: box-shadow 0.2s;
}
.form-section:hover {
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}

.section-header {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  background: #fff;
  border-bottom: 1px solid #e9ecef;
  font-weight: 600;
  font-size: 0.95rem;
  color: #1e293b;
}

.section-icon {
  font-size: 1.2rem;
}

.section-body {
  padding: 16px;
}

.form-section .form-label {
  font-size: 0.85rem;
  color: #334155;
  margin-bottom: 4px;
}

.form-section .form-control,
.form-section .form-select {
  border: 1px solid #d1d5db;
  background: #fff;
  padding: 10px 12px;
  border-radius: 8px;
  transition: all 0.2s ease;
}
.form-section .form-control:focus,
.form-section .form-select:focus {
  border-color: #0d6efd;
  box-shadow: 0 0 0 3px rgba(13, 110, 253, 0.15);
  background: #fff;
}
.form-section .form-control::placeholder {
  color: #94a3b8;
  font-size: 0.9rem;
}
</style>
