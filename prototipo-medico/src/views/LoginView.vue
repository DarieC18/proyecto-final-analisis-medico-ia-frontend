<template>
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-5 col-lg-4 mt-5">
        <div class="text-center mb-4">
          <h1 class="text-primary fw-bold mb-1">⚕️ MedAnalyzer</h1>
          <p class="text-muted">Plataforma de Análisis Clínico</p>
        </div>

        <div class="card shadow-sm p-4">
          <div class="card-body">
            <h4 class="fw-bold mb-4 text-center">Bienvenido de nuevo</h4>
            <div v-if="errorMsg" class="alert alert-danger border-0 rounded-3 py-2 small">{{ errorMsg }}</div>
            <form @submit.prevent="iniciarSesion" class="form-card">
              <div class="mb-3">
                <label class="form-label fw-medium">👤 Usuario o Correo</label>
                <input v-model="userName" type="text" class="form-control form-control-lg" placeholder="Tu usuario o correo electrónico" required>
              </div>
              <div class="mb-4">
                <label class="form-label fw-medium">🔒 Contraseña</label>
                <input v-model="password" type="password" class="form-control form-control-lg" placeholder="••••••••" required>
              </div>
              <button type="submit" class="btn btn-primary btn-lg w-100 shadow-sm rounded-pill mb-3" :disabled="loading">
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
    const roles = res.data.roles || []
    if (roles.includes('Administrator')) {
      router.push('/dashboard-admin')
    } else if (roles.includes('Doctor') || roles.includes('Nurse')) {
      router.push('/dashboard-medico')
    } else {
      router.push('/perfil')
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
.form-card .form-control-lg,
.form-card .form-control {
  border: 1px solid #d1d5db;
  background: #fff;
  padding: 12px 16px;
  border-radius: 10px;
  transition: all 0.2s ease;
}
.form-card .form-control-lg:focus,
.form-card .form-control:focus {
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
