<template>
  <div class="container mt-4">
    <div class="row justify-content-center">
      <div class="col-md-8">
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-border text-primary" role="status"></div>
        </div>
        <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>
        <template v-else>
          <div class="d-flex align-items-center gap-3 mb-4">
            <div class="bg-primary text-white rounded-circle d-flex align-items-center justify-content-center" style="width: 64px; height: 64px; font-size: 1.5rem;">
              {{ iniciales }}
            </div>
            <div>
              <h3 class="fw-bold mb-0">{{ profile?.fullName }}</h3>
              <p class="text-muted mb-0">Paciente</p>
            </div>
          </div>
          <div class="card shadow-sm border-0 rounded-4">
            <div class="card-body p-5">
              <h5 class="fw-bold mb-4">Mi Perfil</h5>
              <div class="row g-4">
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Nombre Completo</label>
                  <p class="fw-medium">{{ profile?.fullName || '-' }}</p>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Usuario</label>
                  <p class="fw-medium">{{ profile?.userName || '-' }}</p>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Email</label>
                  <p class="fw-medium">{{ profile?.email || '-' }}</p>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Identificación</label>
                  <p class="fw-medium">{{ profile?.numberIdentification || '-' }}</p>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Teléfono</label>
                  <p class="fw-medium">{{ profile?.phoneNumber || '-' }}</p>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Género</label>
                  <p class="fw-medium">{{ profile?.gender || '-' }}</p>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Fecha de Nacimiento</label>
                  <p class="fw-medium">{{ formatDate(profile?.birthDate) }}</p>
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Tipo de Paciente</label>
                  <p class="fw-medium">{{ profile?.patientType || '-' }}</p>
                </div>
              </div>
            </div>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { portalService } from '@/api/portal'

const loading = ref(true)
const error = ref('')
const profile = ref(null)

const iniciales = computed(() => {
  const p = profile.value
  if (!p?.fullName) return '?'
  return p.fullName.split(' ').map(w => w[0]).join('').toUpperCase().slice(0, 2)
})

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' })
}

onMounted(async () => {
  try {
    const res = await portalService.getProfile()
    profile.value = res.data
  } catch (err) {
    if (err.response?.status === 404) {
      error.value = 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
    } else {
      error.value = 'Error al cargar perfil'
    }
  } finally {
    loading.value = false
  }
})
</script>
