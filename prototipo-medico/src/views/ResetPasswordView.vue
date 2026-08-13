<template>
  <AuthCard
    :title="successMsg ? 'Contraseña restablecida' : 'Restablecer contraseña'"
    :subtitle="successMsg ? 'Ya puedes acceder con tu nueva contraseña.' : 'Elige una contraseña nueva'"
    :icon="successMsg ? IconSuccess : IconPassword"
    :tone="successMsg ? 'success' : 'brand'"
  >
    <AppAlert v-if="errorMsg" variant="danger" :message="errorMsg" class="mb-3" />

    <div v-if="successMsg" class="d-grid">
      <AppButton variant="primary" size="lg" to="/login">Ir a iniciar sesión</AppButton>
    </div>

    <form v-else @submit.prevent="restablecer">
      <div class="mb-3">
        <label for="rp-pass" class="form-label d-inline-flex align-items-center gap-2">
          <Icon :icon="IconPassword" :size="15" tone="muted" />
          Nueva contraseña
        </label>
        <input
          id="rp-pass"
          v-model="password"
          type="password"
          class="form-control"
          placeholder="••••••••"
          autocomplete="new-password"
          required
        >
      </div>

      <div class="mb-4">
        <label for="rp-pass2" class="form-label d-inline-flex align-items-center gap-2">
          <Icon :icon="IconLock" :size="15" tone="muted" />
          Confirmar contraseña
        </label>
        <input
          id="rp-pass2"
          v-model="confirmPassword"
          type="password"
          class="form-control"
          :class="{ 'is-invalid': mismatch }"
          placeholder="••••••••"
          autocomplete="new-password"
          required
        >
        <div v-if="mismatch" class="invalid-feedback d-block">Las contraseñas no coinciden.</div>
      </div>

      <AppButton
        type="submit"
        variant="primary"
        size="lg"
        block
        :loading="loading"
        :disabled="!canSubmit"
      >
        {{ loading ? 'Restableciendo…' : 'Restablecer contraseña' }}
      </AppButton>
    </form>

    <template #footer>
      <RouterLink to="/login">Volver a inicio de sesión</RouterLink>
    </template>
  </AuthCard>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import AuthCard from '@/layouts/AuthCard.vue'
import AppAlert from '@/components/ui/AppAlert.vue'
import AppButton from '@/components/ui/AppButton.vue'
import Icon from '@/components/ui/Icon.vue'
import { IconLock, IconPassword, IconSuccess } from '@/lib/icons'
import { authService } from '@/api/auth'

const route = useRoute()
const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const successMsg = ref('')
const errorMsg = ref('')

const userId = ref('')
const token = ref('')

// Se avisa del desajuste mientras se escribe, no al enviar.
const mismatch = computed(
  () => confirmPassword.value.length > 0 && password.value !== confirmPassword.value
)
const canSubmit = computed(
  () => !!password.value && password.value === confirmPassword.value && !!userId.value
)

onMounted(() => {
  userId.value = route.query.userId || ''
  token.value = route.query.token || ''
  if (!userId.value || !token.value) {
    errorMsg.value =
      'Enlace inválido o expirado. Solicita un nuevo restablecimiento de contraseña.'
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
    successMsg.value = 'Contraseña restablecida exitosamente.'
  } catch (err) {
    errorMsg.value =
      err.response?.data?.errors?.join(', ') || 'Error al restablecer la contraseña'
  } finally {
    loading.value = false
  }
}
</script>
