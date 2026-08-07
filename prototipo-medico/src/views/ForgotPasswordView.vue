<template>
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-5 col-lg-4 mt-5">
        <div class="text-center mb-4">
          <h1 class="text-primary fw-bold mb-1">Recuperar Contraseña</h1>
          <p class="text-muted">Ingresa tu correo electrónico para recibir un enlace de restablecimiento</p>
        </div>

        <div class="card shadow-sm p-4">
          <div class="card-body">
            <div v-if="successMsg" class="alert alert-success border-0 rounded-3 py-2 small">{{ successMsg }}</div>
            <div v-if="errorMsg" class="alert alert-danger border-0 rounded-3 py-2 small">{{ errorMsg }}</div>
            <form @submit.prevent="enviarSolicitud" class="form-card">
              <div class="mb-4">
                <label class="form-label fw-medium">Correo electrónico</label>
                <input v-model="email" type="email" class="form-control form-control-lg" placeholder="correo@ejemplo.com" required>
              </div>
              <button type="submit" class="btn btn-primary btn-lg w-100 shadow-sm rounded-pill mb-3" :disabled="loading">
                <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                {{ loading ? 'Enviando...' : 'Enviar enlace' }}
              </button>
            </form>
            <div class="text-center mt-3">
              <RouterLink to="/login" class="text-decoration-none text-muted small">Volver a inicio de sesión</RouterLink>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { authService } from '@/api/auth'

const email = ref('')
const loading = ref(false)
const successMsg = ref('')
const errorMsg = ref('')

const enviarSolicitud = async () => {
  loading.value = true
  successMsg.value = ''
  errorMsg.value = ''
  try {
    const res = await authService.forgotPassword(email.value)
    if (res.data.hasError) {
      errorMsg.value = res.data.errors?.join(', ') || 'Error al enviar solicitud'
      return
    }
    successMsg.value = 'Si el correo está registrado, recibirás un enlace para restablecer tu contraseña.'
    email.value = ''
  } catch (err) {
    const data = err.response?.data
    errorMsg.value = data?.errors?.join(', ') || 'Error al enviar solicitud'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.form-card .form-control-lg {
  border: 1px solid #d1d5db;
  background: #fff;
  padding: 12px 16px;
  border-radius: 10px;
  transition: all 0.2s ease;
}
.form-card .form-control-lg:focus {
  border-color: #0d6efd;
  box-shadow: 0 0 0 3px rgba(13, 110, 253, 0.15);
  background: #fff;
}
.form-card .form-label {
  font-size: 0.9rem;
  color: #334155;
  margin-bottom: 6px;
}
</style>
