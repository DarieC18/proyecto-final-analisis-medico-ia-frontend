<template>
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-5 col-lg-4 mt-5">
        <div class="text-center mb-4">
          <h1 class="text-primary fw-bold mb-1">⚕️ Smart-Medical</h1>
          <p class="text-muted">Plataforma de Análisis Clínico</p>
        </div>

        <div class="card shadow-sm p-4">
          <div class="card-body">
            <h4 class="fw-bold mb-4 text-center">Bienvenido de nuevo</h4>
            <div v-if="errorMsg" class="alert alert-danger border-0 rounded-3 py-2 small">{{ errorMsg }}</div>
            <form @submit.prevent="iniciarSesion">
              <div class="mb-3">
                <label class="form-label text-muted small fw-bold text-uppercase">Nombre de Usuario</label>
                <input v-model="userName" type="text" class="form-control form-control-lg bg-light border-0" placeholder="Administrator" required>
              </div>
              <div class="mb-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Contraseña</label>
                <input v-model="password" type="password" class="form-control form-control-lg bg-light border-0" placeholder="••••••••" required>
              </div>
              <button type="submit" class="btn btn-primary btn-lg w-100 shadow-sm mb-3" :disabled="loading">
                <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                {{ loading ? 'Ingresando...' : 'Acceder al sistema' }}
              </button>
            </form>
            <div class="text-center mt-3 d-flex justify-content-center gap-3">
              <RouterLink to="/registro" class="text-decoration-none text-muted small">¿Primera vez? Solicitar acceso</RouterLink>
              <span class="text-muted small">|</span>
              <RouterLink to="/forgot-password" class="text-decoration-none text-muted small">¿Olvidaste tu contraseña?</RouterLink>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { authService } from '@/api/auth'
import { authStore } from '@/stores/auth'

const router = useRouter()
const auth = authStore

const userName = ref('')
const password = ref('')
const loading = ref(false)
const errorMsg = ref('')

const iniciarSesion = async () => {
  loading.value = true
  errorMsg.value = ''
  try {
    const res = await authService.login(userName.value, password.value)
    if (res.data.hasError) {
      errorMsg.value = res.data.errors?.join(', ') || 'Error al iniciar sesión'
      return
    }
    auth.setSession(res.data)
    const role = res.data.roles?.[0]
    if (role === 'Administrator') {
      router.push('/dashboard-admin')
    } else {
      router.push('/dashboard-medico')
    }
  } catch (err) {
    const data = err.response?.data
    if (data?.errors) {
      errorMsg.value = data.errors.join(', ')
    } else {
      errorMsg.value = 'Credenciales inválidas. Intente de nuevo.'
    }
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
