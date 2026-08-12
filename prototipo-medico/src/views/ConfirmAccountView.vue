<template>
  <AuthCard
    :title="loading ? 'Confirmando tu cuenta' : success ? '¡Cuenta confirmada!' : 'Error de confirmación'"
    :subtitle="subtitle"
    :icon="loading ? null : success ? IconSuccess : IconError"
    :tone="success ? 'success' : 'danger'"
  >
    <LoadingState v-if="loading" label="Verificando el enlace…" />

    <div v-else class="d-grid">
      <AppButton :variant="success ? 'primary' : 'soft'" to="/login" size="lg">
        {{ success ? 'Ir al inicio de sesión' : 'Volver al inicio de sesión' }}
      </AppButton>
    </div>
  </AuthCard>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import AuthCard from '@/layouts/AuthCard.vue'
import AppButton from '@/components/ui/AppButton.vue'
import LoadingState from '@/components/ui/LoadingState.vue'
import { IconError, IconSuccess } from '@/lib/icons'
import { authService } from '@/api/auth'

const route = useRoute()
const loading = ref(true)
const success = ref(false)
const errorMsg = ref('')

const subtitle = computed(() => {
  if (loading.value) return ''
  if (success.value) return 'Tu cuenta ha sido verificada. Ya puedes iniciar sesión.'
  return errorMsg.value || 'El enlace de confirmación es inválido o ha expirado.'
})

onMounted(async () => {
  const { userId, token } = route.query
  if (!userId || !token) {
    loading.value = false
    errorMsg.value = 'Faltan parámetros de confirmación en el enlace.'
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
