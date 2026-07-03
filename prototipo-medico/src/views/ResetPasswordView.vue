<template>
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-5 col-lg-4 mt-5">
        <div class="text-center mb-4">
          <h1 class="text-primary fw-bold mb-1">Restablecer Contraseña</h1>
          <p class="text-muted">Ingresa tu nueva contraseña</p>
        </div>

        <div class="card shadow-sm p-4">
          <div class="card-body">
            <div v-if="successMsg" class="alert alert-success border-0 rounded-3 py-2 small">{{ successMsg }}</div>
            <div v-if="errorMsg" class="alert alert-danger border-0 rounded-3 py-2 small">{{ errorMsg }}</div>
            <form @submit.prevent="restablecer" v-if="!successMsg">
              <div class="mb-3">
                <label class="form-label text-muted small fw-bold text-uppercase">Nueva Contraseña</label>
                <input v-model="password" type="password" class="form-control form-control-lg bg-light border-0" placeholder="••••••••" required>
              </div>
              <div class="mb-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Confirmar Contraseña</label>
                <input v-model="confirmPassword" type="password" class="form-control form-control-lg bg-light border-0" placeholder="••••••••" required>
              </div>
              <button type="submit" class="btn btn-primary btn-lg w-100 shadow-sm mb-3" :disabled="loading">
                <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                {{ loading ? 'Restableciendo...' : 'Restablecer contraseña' }}
              </button>
            </form>
            <div class="text-center mt-3" v-if="successMsg">
              <RouterLink to="/login" class="btn btn-primary px-4 py-2">Ir a iniciar sesión</RouterLink>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { authService } from '@/api/auth'

const route = useRoute()
const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const successMsg = ref('')
const errorMsg = ref('')

const userId = ref('')
const token = ref('')

onMounted(() => {
  userId.value = route.query.userId || ''
  token.value = route.query.token || ''
  if (!userId.value || !token.value) {
    errorMsg.value = 'Enlace inválido o expirado. Solicita un nuevo restablecimiento de contraseña.'
  }
})

const restablecer = async () => {
  if (password.value !== confirmPassword.value) {
    errorMsg.value = 'Las contraseñas no coinciden'
    return
  }
  loading.value = true
  errorMsg.value = ''
  try {
    const res = await authService.resetPassword(userId.value, token.value, password.value)
    if (res.data.hasError) {
      errorMsg.value = res.data.errors?.join(', ') || 'Error al restablecer la contraseña'
      return
    }
    successMsg.value = 'Contraseña restablecida exitosamente. Ahora puedes iniciar sesión con tu nueva contraseña.'
  } catch (err) {
    const data = err.response?.data
    errorMsg.value = data?.errors?.join(', ') || 'Error al restablecer la contraseña'
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
