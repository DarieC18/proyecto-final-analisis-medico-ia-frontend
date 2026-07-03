<template>
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-5 col-lg-4 mt-5">
        <div class="text-center mb-4">
          <h1 class="text-primary fw-bold mb-1">⚕️ Smart-Medical</h1>
          <p class="text-muted">Plataforma de Análisis Clínico</p>
        </div>
        
        <div class="card shadow-sm p-4">
          <div class="card-body">
            <h4 class="fw-bold mb-4 text-center">Bienvenido de nuevo</h4>
            <form @submit.prevent="iniciarSesion">
              <div class="mb-3">
                <label class="form-label text-muted small fw-bold text-uppercase">Usuario o Correo</label>
                <input type="text" class="form-control form-control-lg bg-light border-0" placeholder="dr.ejemplo@med.com" required>
              </div>
              <div class="mb-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Contraseña</label>
                <input type="password" class="form-control form-control-lg bg-light border-0" placeholder="••••••••" required>
              </div>
              <button type="submit" class="btn btn-primary btn-lg w-100 shadow-sm mb-3">Acceder al sistema</button>
            </form>
            <div class="text-center mt-3">
              <RouterLink to="/registro" class="text-decoration-none text-muted small hover-primary">¿Primera vez? Solicitar acceso</RouterLink>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

const iniciarSesion = () => {
  localStorage.setItem('auth', 'true')
  router.push('/dashboard-medico')
}
</script>

<style scoped>
input:focus {
  background-color: #fff !important;
  box-shadow: 0 0 0 0.25rem rgba(14, 165, 233, 0.25);
}
</style>