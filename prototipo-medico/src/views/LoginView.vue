<template>
  <AuthCard title="Bienvenido de nuevo" subtitle="Accede a la plataforma de análisis clínico">
    <AppAlert v-if="errorMsg" variant="danger" :message="errorMsg" class="mb-3" />

    <form @submit.prevent="iniciarSesion">
      <div class="mb-3">
        <label for="login-user" class="form-label d-inline-flex align-items-center gap-2">
          <Icon :icon="IconUser" :size="15" tone="muted" />
          Nombre de usuario
        </label>
        <input
          id="login-user"
          v-model="userName"
          type="text"
          class="form-control"
          placeholder="Tu usuario"
          autocomplete="username"
          required
        >
      </div>

      <div class="mb-4">
        <label for="login-pass" class="form-label d-inline-flex align-items-center gap-2">
          <Icon :icon="IconLock" :size="15" tone="muted" />
          Contraseña
        </label>
        <input
          id="login-pass"
          v-model="password"
          type="password"
          class="form-control"
          placeholder="••••••••"
          autocomplete="current-password"
          required
        >
      </div>

      <AppButton type="submit" variant="primary" size="lg" block :loading="loading">
        {{ loading ? 'Ingresando…' : 'Acceder al sistema' }}
      </AppButton>
    </form>

    <template #footer>
      <div class="d-flex justify-content-center align-items-center gap-2 flex-wrap">
        <RouterLink to="/registro">¿Primera vez? Solicitar acceso</RouterLink>
        <span class="text-app-subtle">·</span>
        <RouterLink to="/forgot-password">¿Olvidaste tu contraseña?</RouterLink>
      </div>
    </template>
  </AuthCard>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import AuthCard from '@/layouts/AuthCard.vue'
import AppAlert from '@/components/ui/AppAlert.vue'
import AppButton from '@/components/ui/AppButton.vue'
import Icon from '@/components/ui/Icon.vue'
import { IconLock, IconUser } from '@/lib/icons'
import { authService } from '@/api/auth'
import { authStore } from '@/stores/auth'
import { homeRouteForRoles } from '@/config/navigation'

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
    // Antes esto era un if/else propio que mandaba a los pacientes a /perfil,
    // una ruta que su rol no tiene permitida: el guard los rebotaba en el acto.
    router.push(homeRouteForRoles(res.data.roles || []) ?? '/login')
  } catch (err) {
    const data = err.response?.data
    errorMsg.value = data?.errors
      ? data.errors.join(', ')
      : 'Credenciales inválidas. Intente de nuevo.'
  } finally {
    loading.value = false
  }
}
</script>
