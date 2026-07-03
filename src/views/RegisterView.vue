<template>
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-8">
        <div class="card shadow">
          <div class="card-body">
            <h3 class="card-title text-center mb-4">Registro de Usuario</h3>
            <form @submit.prevent>
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Nombre completo</label> <input type="text" class="form-control">
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Nombre de usuario</label> <input type="text" class="form-control">
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label">Correo electrónico</label> <input type="email" class="form-control">
              </div>
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Contraseña</label> <input type="password" class="form-control">
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Confirmar contraseña</label> <input type="password" class="form-control">
                </div>
              </div>
              <div class="mb-4">
                <label class="form-label">Tipo de usuario</label> <select class="form-select">
                  <option selected>Seleccione un rol...</option>
                  <option value="medico">Médico</option> <option value="enfermera">Enfermera</option> <option value="usuario">Usuario de consulta</option> </select>
              </div>
              <button class="btn btn-success w-100">Registrar cuenta</button>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
</script>