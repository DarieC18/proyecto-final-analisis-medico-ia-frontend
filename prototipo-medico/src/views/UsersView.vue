<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Gestión de Usuarios</h3>
        <p class="text-muted">Administración de cuentas del sistema</p>
      </div>
      <button @click="abrirCrear" class="btn btn-dark px-4 shadow-sm">+ Crear Usuario</button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <template v-else>
      <div v-if="vista === 'crear' || vista === 'editar'" class="animation-fade">
        <div class="card shadow-sm border-0 rounded-4">
          <div class="card-body p-5">
            <h5 class="fw-bold mb-4">{{ vista === 'editar' ? 'Editar Usuario' : 'Nuevo Usuario' }}</h5>
            <div v-if="formError" class="alert alert-danger border-0 rounded-3 py-2 small">{{ formError }}</div>
            <form @submit.prevent="crearUsuario">
              <div class="form-section mb-4">
                <div class="section-header">
                  <span class="section-icon">👤</span>
                  <span>Datos del Usuario</span>
                </div>
                <div class="section-body">
                  <div class="row g-3">
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Nombre</label>
                      <input v-model="form.name" type="text" class="form-control" placeholder="Nombre del usuario" required>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Apellido</label>
                      <input v-model="form.lastName" type="text" class="form-control" placeholder="Apellido del usuario" required>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Usuario</label>
                      <input v-model="form.userName" type="text" class="form-control" placeholder="Nombre de usuario" required>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Identificación</label>
                      <input v-model="form.numberIdentification" type="text" class="form-control" maxlength="20" placeholder="Ej: 001-1234567-8" required>
                      <small v-if="form.numberIdentification.length === 20" class="text-danger">Máximo 20 caracteres</small>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Email</label>
                      <input v-model="form.email" type="email" class="form-control" placeholder="correo@ejemplo.com" required>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Rol</label>
                      <select v-model="form.role" class="form-select" :disabled="vista === 'editar' && form.role === 'Patient'" required>
                        <option v-if="form.role === 'Patient'" value="Patient">Paciente</option>
                        <template v-else>
                          <option value="Doctor">Médico</option>
                          <option value="Nurse">Enfermero</option>
                          <option value="Administrator">Administrador</option>
                        </template>
                      </select>
                    </div>
                  </div>
                </div>
              </div>

              <div v-if="vista === 'crear'" class="alert alert-info border-0 rounded-3 py-2 small mb-4">
                🔑 Se generará una contraseña automática y se enviará por correo al nuevo usuario.
              </div>

              <hr class="my-4">

              <div class="d-flex justify-content-end gap-3">
                <button type="button" @click="cancelarForm" class="btn btn-outline-secondary rounded-pill px-4 py-2">Cancelar</button>
                <button type="submit" class="btn btn-primary rounded-pill px-5 py-2 shadow" :disabled="saving">
                  <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                  {{ saving ? 'Guardando...' : vista === 'editar' ? '💾 Actualizar Usuario' : '👤 Crear Usuario' }}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>

      <div v-else class="card shadow-sm overflow-hidden border-0">
        <div v-if="users.length === 0" class="text-center py-5 text-muted">
          <p>No hay usuarios registrados.</p>
        </div>
        <div v-else class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-dark">
              <tr>
                <th class="ps-4 py-3 fw-medium">Nombre Completo</th>
                <th class="py-3 fw-medium">Usuario</th>
                <th class="py-3 fw-medium">Correo</th>
                <th class="py-3 fw-medium">Rol</th>
                <th class="py-3 fw-medium">Estado</th>
                <th class="pe-4 py-3 fw-medium text-end">Acciones</th>
              </tr>
            </thead>
            <tbody class="border-top-0">
              <tr v-for="u in users" :key="u.id">
                <td class="ps-4 py-3 fw-bold text-dark">{{ u.name }} {{ u.lastName }}</td>
                <td class="py-3 text-muted">{{ u.userName }}</td>
                <td class="py-3 text-muted">{{ u.email }}</td>
                <td class="py-3">
                  <span class="badge bg-info bg-opacity-10 text-info rounded-pill px-3 py-2">{{ translateRole(u.role) }}</span>
                </td>
                <td class="py-3">
                  <StatusBadge :text="u.status ? 'Activo' : 'Inactivo'" :variant="u.status ? 'active' : 'inactive'" />
                </td>
                <td class="pe-4 py-3 text-end">
                  <button v-if="u.id === auth.user?.id" class="btn btn-sm btn-secondary px-3" disabled>Mi Cuenta</button>
                  <template v-else>
                    <button @click="abrirEditar(u)" class="btn btn-sm btn-light border text-warning fw-medium px-3 me-2">Editar</button>
                    <button @click="toggleStatus(u)" class="btn btn-sm px-3" :class="u.status ? 'btn-light border text-danger' : 'btn-light border text-success'">
                      {{ u.status ? 'Desactivar' : 'Activar' }}
                    </button>
                  </template>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { accountService } from '@/api/account'
import { authStore } from '@/stores/auth'
import StatusBadge from '@/components/StatusBadge.vue'
import { translateRole } from '@/utils/roles'

const auth = authStore
const loading = ref(true)
const error = ref('')
const saving = ref(false)
const formError = ref('')
const users = ref([])
const vista = ref('lista')

const form = reactive({
  name: '', lastName: '', userName: '', email: '',
  numberIdentification: '',
  role: 'Doctor'
})

const cargarUsuarios = async () => {
  loading.value = true
  error.value = ''
  try {
    const res = await accountService.getAll()
    users.value = res.data || []
  } catch (err) {
    if (err.response?.status === 404 || err.response?.status === 204) {
      users.value = []
    } else {
      error.value = 'Error al cargar usuarios'
    }
  } finally {
    loading.value = false
  }
}

const abrirCrear = () => {
  vista.value = 'crear'
  Object.assign(form, { name: '', lastName: '', userName: '', email: '', numberIdentification: '', role: 'Doctor' })
  formError.value = ''
}

const cancelarForm = () => {
  vista.value = 'lista'
}

const crearUsuario = async () => {
  saving.value = true
  formError.value = ''
  try {
    let res
    if (vista.value === 'editar') {
      res = await accountService.update(form.id, { ...form })
    } else {
      res = await accountService.create({ ...form })
    }
    if (res.data.hasError) {
      formError.value = res.data.errors?.join(', ') || 'Error al guardar usuario'
      return
    }
    vista.value = 'lista'
    await cargarUsuarios()
  } catch (err) {
    const data = err.response?.data
    formError.value = data?.errors?.join(', ') || 'Error al guardar usuario'
  } finally {
    saving.value = false
  }
}

const abrirEditar = (u) => {
  vista.value = 'editar'
  Object.assign(form, {
    id: u.id, name: u.name, lastName: u.lastName,
    userName: u.userName, email: u.email,
    numberIdentification: u.numberIdentification, role: u.role
  })
  formError.value = ''
}

const toggleStatus = async (u) => {
  try {
    const res = await accountService.toggleStatus(u.id)
    if (res.data.hasError) {
      error.value = res.data.errors?.join(', ')
      return
    }
    await cargarUsuarios()
  } catch (err) {
    error.value = err.response?.data?.message || 'Error al cambiar estado'
  }
}

onMounted(cargarUsuarios)
</script>

<style scoped>
.animation-fade { animation: fadeIn 0.3s ease-in-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }

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
