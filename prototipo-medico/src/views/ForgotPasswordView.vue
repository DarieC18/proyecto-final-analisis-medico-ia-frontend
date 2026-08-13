<template>
  <AuthCard
    title="Recuperar contraseña"
    subtitle="Te enviaremos un enlace para restablecerla"
    :icon="IconPassword"
  >
    <AppAlert v-if="successMsg" variant="success" :message="successMsg" class="mb-3" />
    <AppAlert v-if="errorMsg" variant="danger" :message="errorMsg" class="mb-3" />

    <form @submit.prevent="enviarSolicitud">
      <div class="mb-4">
        <label for="fp-email" class="form-label d-inline-flex align-items-center gap-2">
          <Icon :icon="IconEmail" :size="15" tone="muted" />
          Correo electrónico
        </label>
        <input
          id="fp-email"
          v-model="email"
          type="email"
          class="form-control"
          placeholder="correo@ejemplo.com"
          autocomplete="email"
          required
        >
      </div>

      <AppButton type="submit" variant="primary" size="lg" block :loading="loading">
        {{ loading ? 'Enviando…' : 'Enviar enlace' }}
      </AppButton>
    </form>

    <template #footer>
      <RouterLink to="/login">Volver a inicio de sesión</RouterLink>
    </template>
  </AuthCard>
</template>

<script setup>
import { ref } from 'vue'
import AuthCard from '@/layouts/AuthCard.vue'
import AppAlert from '@/components/ui/AppAlert.vue'
import AppButton from '@/components/ui/AppButton.vue'
import Icon from '@/components/ui/Icon.vue'
import { IconEmail, IconPassword } from '@/lib/icons'
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
    successMsg.value =
      'Si el correo está registrado, recibirás un enlace para restablecer tu contraseña.'
    email.value = ''
  } catch (err) {
    errorMsg.value = err.response?.data?.errors?.join(', ') || 'Error al enviar solicitud'
  } finally {
    loading.value = false
  }
}
</script>
