<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>Gestión de Usuarios</h2>  
      <button class="btn btn-dark">+ Crear Usuario</button>  
    </div>

    <div class="card shadow-sm">
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-dark">
            <tr>
              <th>Nombre Completo</th>  
              <th>Correo</th> 
              <th>Rol</th>  
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Administrador Principal</td>
              <td>admin@medapp.com</td>
              <td>Administrador</td> 
              <td><span class="badge bg-success">Activo</span></td> 
              <td>
                <button class="btn btn-sm btn-secondary" disabled>Propia Cuenta</button>
              </td>
            </tr>
            <tr>
              <td>Dra. Ana López</td>
              <td>alopez@medapp.com</td>
              <td>Médico</td> 
              <td><span class="badge bg-success">Activo</span></td>
              <td>
                <button class="btn btn-sm btn-warning me-2">Editar</button> 
                <button class="btn btn-sm btn-danger">Inactivar</button> 
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
</script>