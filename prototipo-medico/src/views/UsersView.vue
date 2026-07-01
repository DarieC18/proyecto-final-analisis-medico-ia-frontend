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
            <h5 class="fw-bold mb-4">Nuevo Usuario</h5>
            <div v-if="formError" class="alert alert-danger border-0 rounded-3 py-2 small">{{ formError }}</div>
            <form @submit.prevent="crearUsuario">
              <div class="row g-4">
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Nombre</label>
                  <input v-model="form.name" type="text" class="form-control bg-light border-0 py-2" required>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Apellido</label>
                  <input v-model="form.lastName" type="text" class="form-control bg-light border-0 py-2" required>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Usuario</label>
                  <input v-model="form.userName" type="text" class="form-control bg-light border-0 py-2" required>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Identificación</label>
                  <input v-model="form.numberIdentification" type="text" class="form-control bg-light border-0 py-2" required>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Email</label>
                  <input v-model="form.email" type="email" class="form-control bg-light border-0 py-2" required>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Rol</label>
                  <select v-model="form.role" class="form-select bg-light border-0 py-2" required>
                    <option value="Doctor">Médico</option>
                    <option value="Nurse">Enfermera</option>
                    <option value="ConsultationUser">Usuario de consulta</option>
                    <option value="Administrator">Administrador</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Contraseña</label>
                  <input v-model="form.password" type="password" class="form-control bg-light border-0 py-2" required>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Confirmar contraseña</label>
                  <input v-model="form.confirmPassword" type="password" class="form-control bg-light border-0 py-2" required>
                </div>
              </div>
              <div class="d-flex justify-content-end gap-3 mt-5">
                <button type="button" @click="cancelarForm" class="btn btn-light border px-4 py-2">Cancelar</button>
                <button type="submit" class="btn btn-primary px-4 py-2 shadow-sm" :disabled="saving">
                  <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                  {{ saving ? 'Guardando...' : vista === 'editar' ? 'Actualizar Usuario' : 'Crear Usuario' }}
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
                  <span class="badge bg-info bg-opacity-10 text-info rounded-pill px-3 py-2">{{ u.role }}</span>
                </td>
                <td class="py-3">
                  <StatusBadge :text="u.status ? 'Activo' : 'Inactivo'" :variant="u.status ? 'active' : 'inactive'" />
                </td>
                <td class="pe-4 py-3 text-end">
                  <button v-if="u.id === auth.user?.id" class="btn btn-sm btn-secondary px-3" disabled>Propia Cuenta</button>
                  <template v-else>
                    <button @click="abrirEditar(u)" class="btn btn-sm btn-light border text-warning fw-medium px-3 me-2">Editar</button>
                    <button @click="toggleStatus(u)" class="btn btn-sm px-3" :class="u.status ? 'btn-light border text-danger' : 'btn-light border text-success'">
                      {{ u.status ? 'Inactivar' : 'Activar' }}
                    </button>
                    <button @click="confirmarEliminar(u)" class="btn btn-sm btn-light border text-danger fw-medium px-3 ms-2">Eliminar</button>
                  </template>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <ConfirmDialog
      :visible="deleteDialog"
      title="Eliminar Usuario"
      :message="`¿Está seguro que desea eliminar a ${deleteTarget?.name} ${deleteTarget?.lastName}?`"
      confirmText="Eliminar"
      :danger="true"
      @confirm="eliminarUsuario"
      @cancel="deleteDialog = false"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { accountService } from '@/api/account'
import { authStore } from '@/stores/auth'
import StatusBadge from '@/components/StatusBadge.vue'
import ConfirmDialog from '@/components/ConfirmDialog.vue'

const auth = authStore
const loading = ref(true)
const error = ref('')
const saving = ref(false)
const formError = ref('')
const users = ref([])
const vista = ref('lista')

const form = reactive({
  name: '', lastName: '', userName: '', email: '',
  numberIdentification: '', password: '', confirmPassword: '',
  role: 'Doctor'
})

const deleteDialog = ref(false)
const deleteTarget = ref(null)

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
  Object.assign(form, { name: '', lastName: '', userName: '', email: '', numberIdentification: '', password: '', confirmPassword: '', role: 'Doctor' })
  formError.value = ''
}

const cancelarForm = () => {
  vista.value = 'lista'
}

const crearUsuario = async () => {
  if (vista.value === 'crear' && form.password !== form.confirmPassword) {
    formError.value = 'Las contraseñas no coinciden'
    return
  }
  saving.value = true
  formError.value = ''
  try {
    let res
    if (vista.value === 'editar') {
      const { password, confirmPassword, ...data } = form
      res = await accountService.update(form.id, data)
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
    numberIdentification: u.numberIdentification, role: u.role,
    password: '', confirmPassword: ''
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

const confirmarEliminar = (u) => {
  deleteTarget.value = u
  deleteDialog.value = true
}

const eliminarUsuario = async () => {
  deleteDialog.value = false
  try {
    await accountService.remove(deleteTarget.value.id)
    await cargarUsuarios()
  } catch (err) {
    error.value = 'Error al eliminar usuario'
  }
}

onMounted(cargarUsuarios)
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
