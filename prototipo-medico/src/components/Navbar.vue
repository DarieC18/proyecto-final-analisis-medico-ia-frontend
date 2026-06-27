<template>
  <nav class="navbar navbar-expand-lg navbar-light bg-white shadow-sm py-3 custom-navbar">
    <div class="container-fluid px-4">
      <RouterLink class="navbar-brand fw-bold text-primary d-flex align-items-center gap-2" to="/dashboard-medico">
        <span class="fs-4">⚕️</span> Smart-Medical IA
      </RouterLink>
      <button class="navbar-toggler border-0" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav mx-auto gap-3 fw-medium text-secondary">

          <!-- Clinical dropdown -->
          <li class="nav-item dropdown" :class="{ show: showClinical }">
            <button @click="showClinical = !showClinical" class="btn btn-sm btn-light border rounded-pill px-3 py-1 d-flex align-items-center gap-1 dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
              <span>🩺</span>
              <span>Clínico</span>
            </button>
            <ul class="dropdown-menu shadow-sm border-0 rounded-4 p-2 mt-2" :class="{ show: showClinical }" style="min-width: 200px;">
              <li><RouterLink @click="showClinical = false" class="dropdown-item rounded-pill px-3 py-2" to="/dashboard-medico">📊 Dashboard Médico</RouterLink></li>
              <li><RouterLink @click="showClinical = false" class="dropdown-item rounded-pill px-3 py-2" to="/pacientes">👥 Pacientes</RouterLink></li>
              <li><RouterLink @click="showClinical = false" class="dropdown-item rounded-pill px-3 py-2" to="/citas">📅 Citas Médicas</RouterLink></li>
            </ul>
          </li>

          <!-- Divider -->
          <li class="nav-item d-none d-lg-flex align-items-center"><span class="text-muted" style="opacity: 0.3;">│</span></li>

          <!-- Admin section (always visible for admins) -->
          <template v-if="auth.isAdmin()">
            <li class="nav-item"><RouterLink class="nav-link rounded-pill px-3" to="/dashboard-admin">Panel Admin</RouterLink></li>
            <li class="nav-item"><RouterLink class="nav-link rounded-pill px-3" to="/usuarios">Usuarios</RouterLink></li>
            <li class="nav-item"><RouterLink class="nav-link rounded-pill px-3" to="/auditoria">Auditoría</RouterLink></li>
          </template>

          <li class="nav-item"><RouterLink class="nav-link rounded-pill px-3" to="/perfil">Mi Perfil</RouterLink></li>
        </ul>
        <div class="d-flex align-items-center gap-3">
          <small class="text-muted d-none d-lg-block">{{ auth.user?.name }} {{ auth.user?.lastName }}</small>
          <button @click="cerrarSesion" class="btn btn-light text-danger rounded-pill px-4 fw-medium shadow-sm hover-danger">Cerrar sesión</button>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { authStore } from '@/stores/auth'

const router = useRouter()
const auth = authStore
const showClinical = ref(false)

const cerrarSesion = () => {
  auth.logout()
  router.push('/login')
}
</script>

<style scoped>
.custom-navbar .router-link-active {
  background-color: #e0f2fe;
  color: #0ea5e9 !important;
}
.hover-danger:hover {
  background-color: #fee2e2;
}
.dropdown-menu .dropdown-item:hover,
.dropdown-menu .dropdown-item.router-link-active {
  background-color: #e0f2fe;
  color: #0ea5e9 !important;
}
.dropdown-menu .dropdown-item {
  transition: background-color 0.15s ease;
}
</style>
