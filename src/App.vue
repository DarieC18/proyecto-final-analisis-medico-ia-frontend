<template>
  <nav v-if="!isPublicRoute" class="navbar navbar-expand-lg navbar-light bg-white shadow-sm py-3 custom-navbar">
    <div class="container-fluid px-4">
      <a class="navbar-brand fw-bold text-primary d-flex align-items-center gap-2" href="#">
        <span class="fs-4">⚕️</span> Smart-Medical IA
      </a>
      <button class="navbar-toggler border-0" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav mx-auto gap-3 fw-medium text-secondary">
          <li class="nav-item"><RouterLink class="nav-link rounded-pill px-3" to="/dashboard-medico">Dashboard Médico</RouterLink></li>
          <li class="nav-item"><RouterLink class="nav-link rounded-pill px-3" to="/pacientes">Pacientes</RouterLink></li>
          <li class="nav-item"><RouterLink class="nav-link rounded-pill px-3" to="/citas">Citas Médicas</RouterLink></li>
          <li class="nav-item"><RouterLink class="nav-link rounded-pill px-3" to="/dashboard-admin">Panel Admin</RouterLink></li>
          <li class="nav-item"><RouterLink class="nav-link rounded-pill px-3" to="/usuarios">Usuarios</RouterLink></li>
        </ul>
        <button @click="logout" class="btn btn-light text-danger rounded-pill px-4 fw-medium shadow-sm hover-danger">Cerrar sesión</button>
      </div>
    </div>
  </nav>

  <main class="py-5">
    <RouterView />
  </main>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const isPublicRoute = computed(() => route.meta.public)

const logout = () => {
  localStorage.removeItem('auth') 
  router.push('/login') 
}
</script>

<style>
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap');

body { 
  background-color: #f0f4f8;
  font-family: 'Poppins', sans-serif;
  color: #2b3a4a;
}

:root {
  --bs-primary: #0ea5e9; 
  --bs-success: #10b981; 
  --bs-info: #3b82f6;    
}

.card {
  border-radius: 1rem !important;
  border: none !important;
  transition: transform 0.2s ease-in-out;
}
.btn {
  border-radius: 0.5rem;
  font-weight: 500;
  letter-spacing: 0.3px;
}
.custom-navbar .router-link-active {
  background-color: #e0f2fe;
  color: #0ea5e9 !important;
}
.hover-danger:hover {
  background-color: #fee2e2;
}
</style>