<template>
  <div class="container d-flex justify-content-center align-items-center" style="min-height: 70vh;">
    <div class="text-center px-4" style="max-width: 480px;">
      <div class="mb-4">
        <span style="font-size: 5rem; line-height: 1;">🔒</span>
      </div>
      <h2 class="fw-bold mb-2">Acceso Denegado</h2>
      <p class="text-muted mb-4">No tienes permisos para acceder a esta sección. Si crees que esto es un error, contacta al administrador del sistema.</p>
      <button @click="irAlInicio" class="btn btn-primary rounded-pill px-5 py-2 shadow-sm">
        ← Volver a mi panel
      </button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { authStore } from '@/stores/auth'

const router = useRouter()
const auth = authStore

const irAlInicio = () => {
  if (auth.isAdmin()) router.push('/dashboard-admin')
  else if (auth.hasRole('Doctor') || auth.hasRole('Nurse')) router.push('/dashboard-medico')
  else router.push('/portal/perfil')
}
</script>
