<template>
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-5 col-lg-4 mt-5">
        <div class="text-center mb-4">
          <h1 class="text-primary fw-bold mb-1">Recuperar Contraseña</h1>
          <p class="text-muted">Ingresa tu nombre de usuario para recibir un enlace de restablecimiento</p>
        </div>

        <div class="card shadow-sm p-4">
          <div class="card-body">
            <div v-if="successMsg" class="alert alert-success border-0 rounded-3 py-2 small">{{ successMsg }}</div>
            <div v-if="errorMsg" class="alert alert-danger border-0 rounded-3 py-2 small">{{ errorMsg }}</div>
            <form @submit.prevent="enviarSolicitud">
              <div class="mb-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Nombre de Usuario</label>
                <input v-model="userName" type="text" class="form-control form-control-lg bg-light border-0" placeholder="Tu nombre de usuario" required>
              </div>
              <button type="submit" class="btn btn-primary btn-lg w-100 shadow-sm mb-3" :disabled="loading">
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

const userName = ref('')
const loading = ref(false)
const successMsg = ref('')
const errorMsg = ref('')

const enviarSolicitud = async () => {
  loading.value = true
  successMsg.value = ''
  errorMsg.value = ''
  try {
    const res = await authService.forgotPassword(userName.value)
    if (res.data.hasError) {
      errorMsg.value = res.data.errors?.join(', ') || 'Error al enviar solicitud'
      return
    }
    successMsg.value = 'Si el usuario existe, recibirás un correo para restablecer tu contraseña.'
    userName.value = ''
  } catch (err) {
    const data = err.response?.data
    errorMsg.value = data?.errors?.join(', ') || 'Error al enviar solicitud'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
input:focus {
  background-color: #fff !important;
  box-shadow: 0 0 0 0.25rem rgba(14, 165, 233, 0.25);
}
</style>
