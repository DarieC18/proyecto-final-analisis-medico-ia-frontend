<template>
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-8">
        <div class="card shadow">
          <div class="card-body">
            <h3 class="card-title text-center mb-4">Registro de Usuario</h3>
            <div v-if="successMsg" class="alert alert-success border-0 rounded-3">{{ successMsg }}</div>
            <div v-if="errorMsg" class="alert alert-danger border-0 rounded-3">{{ errorMsg }}</div>
            <form @submit.prevent="registrar">
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label text-muted small fw-bold text-uppercase">Nombre</label>
                  <input v-model="form.name" type="text" class="form-control bg-light border-0" required>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label text-muted small fw-bold text-uppercase">Apellido</label>
                  <input v-model="form.lastName" type="text" class="form-control bg-light border-0" required>
                </div>
              </div>
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label text-muted small fw-bold text-uppercase">Nombre de usuario</label>
                  <input v-model="form.userName" type="text" class="form-control bg-light border-0" required>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label text-muted small fw-bold text-uppercase">Cédula / Identificación</label>
                  <input v-model="form.numberIdentification" type="text" class="form-control bg-light border-0" required>
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label text-muted small fw-bold text-uppercase">Correo electrónico</label>
                <input v-model="form.email" type="email" class="form-control bg-light border-0" required>
              </div>
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label text-muted small fw-bold text-uppercase">Contraseña</label>
                  <input v-model="form.password" type="password" class="form-control bg-light border-0" required>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label text-muted small fw-bold text-uppercase">Confirmar contraseña</label>
                  <input v-model="form.confirmPassword" type="password" class="form-control bg-light border-0" required>
                </div>
              </div>
              <div class="mb-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Rol</label>
                <select v-model="form.role" class="form-select bg-light border-0">
                  <option value="">Seleccione un rol...</option>
                  <option value="Doctor">Médico</option>
                  <option value="Nurse">Enfermera</option>
                  <option value="ConsultationUser">Usuario de consulta</option>
                </select>
              </div>
              <button type="submit" class="btn btn-success w-100" :disabled="loading">
                <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                {{ loading ? 'Registrando...' : 'Registrar cuenta' }}
              </button>
            </form>
            <div class="text-center mt-3">
              <RouterLink to="/login" class="text-decoration-none text-muted small">¿Ya tienes cuenta? Inicia sesión</RouterLink>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { authService } from '@/api/auth'

const loading = ref(false)
const successMsg = ref('')
const errorMsg = ref('')
const form = reactive({
  name: '',
  lastName: '',
  userName: '',
  email: '',
  numberIdentification: '',
  password: '',
  confirmPassword: '',
  role: ''
})

const registrar = async () => {
  loading.value = true
  successMsg.value = ''
  errorMsg.value = ''

  if (form.password !== form.confirmPassword) {
    errorMsg.value = 'Las contraseñas no coinciden'
    loading.value = false
    return
  }

  try {
    const res = await authService.register({ ...form })
    if (res.data.hasError) {
      errorMsg.value = res.data.errors?.join(', ') || 'Error al registrar'
      return
    }
    successMsg.value = 'Cuenta registrada exitosamente. Revisa tu correo para confirmar.'
    form.name = ''
    form.lastName = ''
    form.userName = ''
    form.email = ''
    form.numberIdentification = ''
    form.password = ''
    form.confirmPassword = ''
    form.role = ''
  } catch (err) {
    const data = err.response?.data
    errorMsg.value = data?.errors?.join(', ') || 'Error al registrar el usuario'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
input:focus, select:focus {
  background-color: #fff !important;
  box-shadow: 0 0 0 0.25rem rgba(14, 165, 233, 0.25);
}
</style>
