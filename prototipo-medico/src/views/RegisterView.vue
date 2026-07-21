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
              <div class="form-section mb-4">
                <div class="section-header">
                  <span class="section-icon">👤</span>
                  <span>Datos Personales</span>
                </div>
                <div class="section-body">
                  <div class="row g-3">
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Nombre</label>
                      <input v-model="form.name" type="text" class="form-control" placeholder="Tu nombre" required>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Apellido</label>
                      <input v-model="form.lastName" type="text" class="form-control" placeholder="Tu apellido" required>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Nombre de usuario</label>
                      <input v-model="form.userName" type="text" class="form-control" placeholder="Elige un nombre de usuario" required>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Cédula / Identificación</label>
                      <input v-model="form.numberIdentification" type="text" class="form-control" placeholder="Ej: 001-1234567-8" required>
                    </div>
                    <div class="col-12">
                      <label class="form-label fw-medium">Correo electrónico</label>
                      <input v-model="form.email" type="email" class="form-control" placeholder="correo@ejemplo.com" required>
                    </div>
                  </div>
                </div>
              </div>

              <div class="form-section mb-4">
                <div class="section-header">
                  <span class="section-icon">🔐</span>
                  <span>Credenciales</span>
                </div>
                <div class="section-body">
                  <div class="row g-3">
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Contraseña</label>
                      <input v-model="form.password" type="password" class="form-control" placeholder="••••••••" required>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Confirmar contraseña</label>
                      <input v-model="form.confirmPassword" type="password" class="form-control" placeholder="••••••••" required>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label fw-medium">Rol</label>
                      <select v-model="form.role" class="form-select">
                        <option value="">Seleccione un rol...</option>
                        <option value="ConsultationUser">Usuario de consulta</option>
                      </select>
                    </div>
                  </div>
                </div>
              </div>

              <button type="submit" class="btn btn-success w-100 rounded-pill shadow-sm py-2" :disabled="loading">
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
