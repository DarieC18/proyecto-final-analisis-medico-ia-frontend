<template>
  <div class="container d-flex justify-content-center align-items-center" style="min-height: 80vh;">
    <div class="card shadow border-0 rounded-4 text-center p-5" style="max-width: 500px;">
      <div v-if="loading" class="py-4">
        <div class="spinner-border text-primary mb-3" role="status"></div>
        <p class="text-muted">Confirmando tu cuenta...</p>
      </div>
      <div v-else-if="success" class="py-4">
        <div class="display-1 text-success mb-3">✅</div>
        <h4 class="fw-bold mb-3">¡Cuenta Confirmada!</h4>
        <p class="text-muted mb-4">Tu cuenta ha sido verificada exitosamente. Ya puedes iniciar sesión.</p>
        <RouterLink to="/login" class="btn btn-primary px-5 py-2 shadow-sm">Ir al Login</RouterLink>
      </div>
      <div v-else class="py-4">
        <div class="display-1 text-danger mb-3">❌</div>
        <h4 class="fw-bold mb-3">Error de Confirmación</h4>
        <p class="text-muted mb-4">{{ errorMsg || 'El enlace de confirmación es inválido o ha expirado.' }}</p>
        <RouterLink to="/login" class="btn btn-light border px-5 py-2">Volver al Login</RouterLink>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { authService } from '@/api/auth'

const route = useRoute()
const loading = ref(true)
const success = ref(false)
const errorMsg = ref('')

onMounted(async () => {
  const userId = route.query.userId
  const token = route.query.token
  if (!userId || !token) {
    loading.value = false
    errorMsg.value = 'Faltan parámetros de confirmación.'
    return
  }
  try {
    await authService.confirmAccount(userId, token)
    success.value = true
  } catch (err) {
    errorMsg.value = err.response?.data?.message || 'Error al confirmar la cuenta.'
  } finally {
    loading.value = false
  }
})
</script>
