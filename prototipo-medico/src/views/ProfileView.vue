<template>
  <div class="container mt-4">
    <div class="row justify-content-center">
      <div class="col-md-8">
        <div class="d-flex align-items-center gap-3 mb-4">
          <div class="bg-primary text-white rounded-circle d-flex align-items-center justify-content-center" style="width: 64px; height: 64px; font-size: 1.5rem;">
            {{ iniciales }}
          </div>
          <div>
            <h3 class="fw-bold mb-0">{{ user?.name }} {{ user?.lastName }}</h3>
            <p class="text-muted mb-0">{{ user?.role || user?.roles?.[0] }}</p>
          </div>
        </div>

        <div v-if="loading" class="text-center py-5">
          <div class="spinner-border text-primary" role="status"></div>
        </div>

        <div v-else class="card shadow-sm border-0 rounded-4">
          <div class="card-body p-5">
            <h5 class="fw-bold mb-4">Información de la Cuenta</h5>
            <div class="row g-4">
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Nombre</label>
                <p class="fw-medium">{{ user?.name || '-' }}</p>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Apellido</label>
                <p class="fw-medium">{{ user?.lastName || '-' }}</p>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Usuario</label>
                <p class="fw-medium">{{ user?.userName || '-' }}</p>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Email</label>
                <p class="fw-medium">{{ user?.email || '-' }}</p>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Identificación</label>
                <p class="fw-medium">{{ user?.numberIdentification || '-' }}</p>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Rol</label>
                <p><span class="badge bg-info bg-opacity-10 text-info rounded-pill px-3 py-2">{{ user?.role || user?.roles?.[0] || '-' }}</span></p>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Estado</label>
                <p><StatusBadge :text="user?.status ? 'Activo' : 'Inactivo'" :variant="user?.status ? 'active' : 'inactive'" /></p>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Verificado</label>
                <p><StatusBadge :text="user?.isVerified ? 'Sí' : 'No'" :variant="user?.isVerified ? 'active' : 'inactive'" /></p>
              </div>
              <div class="col-12">
                <label class="form-label text-muted small fw-bold text-uppercase">Fecha de Registro</label>
                <p class="fw-medium">{{ formatFecha(user?.createdAt) }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { authService } from '@/api/auth'
import { authStore } from '@/stores/auth'
import StatusBadge from '@/components/StatusBadge.vue'

const auth = authStore
const loading = ref(true)
const user = ref(null)

const iniciales = computed(() => {
  const u = user.value || auth.user
  if (!u) return '?'
  return ((u.name?.[0] || '') + (u.lastName?.[0] || '')).toUpperCase()
})

const formatFecha = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('es-ES', {
    day: 'numeric', month: 'long', year: 'numeric'
  })
}

onMounted(async () => {
  loading.value = true
  try {
    const res = await authService.me()
    user.value = res.data
    auth.setUserFromMe(res.data)
  } catch {
    user.value = auth.user
  } finally {
    loading.value = false
  }
})
</script>
