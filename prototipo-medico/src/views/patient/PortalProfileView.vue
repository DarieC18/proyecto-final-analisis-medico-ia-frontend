<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Mi Perfil</h3>
        <p class="text-muted">Información de tu cuenta y expediente clínico</p>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <template v-else-if="perfil">
      <div class="d-flex align-items-center gap-3 mb-4">
        <div class="bg-primary text-white rounded-circle d-flex align-items-center justify-content-center" style="width: 64px; height: 64px; font-size: 1.5rem;">
          {{ iniciales }}
        </div>
        <div>
          <h4 class="fw-bold mb-0">{{ perfil.fullName }}</h4>
          <p class="text-muted mb-0">Paciente</p>
        </div>
      </div>

      <div class="card shadow-sm border-0 rounded-4">
        <div class="card-body p-5">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h5 class="fw-bold mb-0">Datos de la Cuenta</h5>
            <button @click="editando = !editando" class="btn btn-sm" :class="editando ? 'btn-light border' : 'btn-primary'">
              {{ editando ? 'Cancelar' : 'Editar Perfil' }}
            </button>
          </div>

          <div v-if="formError" class="alert alert-danger border-0 rounded-3 py-2 small mb-4">{{ formError }}</div>

          <div v-if="!editando" class="row g-4">
            <div class="col-md-6">
              <label class="form-label text-muted small fw-bold text-uppercase">Nombre Completo</label>
              <p class="fw-medium">{{ perfil.fullName }}</p>
            </div>
            <div class="col-md-6">
              <label class="form-label text-muted small fw-bold text-uppercase">Email</label>
              <p class="fw-medium">{{ perfil.email }}</p>
            </div>
            <div class="col-md-6">
              <label class="form-label text-muted small fw-bold text-uppercase">Usuario</label>
              <p class="fw-medium">{{ perfil.userName }}</p>
            </div>
            <div class="col-md-6">
              <label class="form-label text-muted small fw-bold text-uppercase">Identificación</label>
              <p class="fw-medium">{{ perfil.numberIdentification }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Fecha de Nacimiento</label>
              <p class="fw-medium">{{ perfil.birthDate ? new Date(perfil.birthDate).toLocaleDateString('es-ES') : '-' }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Género</label>
              <p class="fw-medium">{{ perfil.gender || '-' }}</p>
            </div>
            <div class="col-md-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Teléfono</label>
              <p class="fw-medium">{{ perfil.phoneNumber || '-' }}</p>
            </div>
            <div class="col-md-6">
              <label class="form-label text-muted small fw-bold text-uppercase">Tipo Identificación</label>
              <p class="fw-medium">{{ perfil.identificationType || '-' }}</p>
            </div>
            <div class="col-md-6">
              <label class="form-label text-muted small fw-bold text-uppercase">Tipo Paciente</label>
              <p class="fw-medium">{{ perfil.patientType || '-' }}</p>
            </div>
          </div>

          <form v-else @submit.prevent="guardarPerfil">
            <div class="row g-4">
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Fecha de Nacimiento</label>
                <input v-model="form.birthDate" type="date" class="form-control bg-light border-0 py-2">
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Género</label>
                <select v-model="form.gender" class="form-select bg-light border-0 py-2">
                  <option value="">Seleccionar...</option>
                  <option value="Masculino">Masculino</option>
                  <option value="Femenino">Femenino</option>
                  <option value="Otro">Otro</option>
                </select>
              </div>
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Teléfono</label>
                <input v-model="form.phoneNumber" type="text" class="form-control bg-light border-0 py-2">
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Tipo Identificación</label>
                <select v-model="form.identificationType" class="form-select bg-light border-0 py-2">
                  <option value="">Seleccionar...</option>
                  <option value="Cédula">Cédula</option>
                  <option value="Pasaporte">Pasaporte</option>
                </select>
              </div>
              <div class="col-md-6">
                <label class="form-label text-muted small fw-bold text-uppercase">Tipo Paciente</label>
                <select v-model="form.patientType" class="form-select bg-light border-0 py-2">
                  <option value="">Seleccionar...</option>
                  <option value="General">General</option>
                  <option value="Crónico">Crónico</option>
                  <option value="Emergencia">Emergencia</option>
                </select>
              </div>
            </div>
            <div class="d-flex justify-content-end gap-3 mt-5">
              <button type="button" @click="editando = false" class="btn btn-light border px-4 py-2">Cancelar</button>
              <button type="submit" class="btn btn-primary px-4 py-2 shadow-sm" :disabled="guardando">
                <span v-if="guardando" class="spinner-border spinner-border-sm me-2"></span>
                Guardar Cambios
              </button>
            </div>
          </form>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { portalService } from '@/api/portal'

const loading = ref(true)
const error = ref('')
const editando = ref(false)
const guardando = ref(false)
const formError = ref('')
const perfil = ref(null)

const form = ref({
  birthDate: '', gender: '', phoneNumber: '', identificationType: '', patientType: ''
})

const iniciales = computed(() => {
  if (!perfil.value) return '?'
  return perfil.value.fullName?.split(' ').map(w => w[0]).join('').toUpperCase().slice(0, 2) || '?'
})

const cargarPerfil = async () => {
  loading.value = true
  error.value = ''
  try {
    const res = await portalService.getProfile()
    perfil.value = res.data
    form.value = {
      birthDate: res.data.birthDate ? res.data.birthDate.substring(0, 10) : '',
      gender: res.data.gender || '',
      phoneNumber: res.data.phoneNumber || '',
      identificationType: res.data.identificationType || '',
      patientType: res.data.patientType || ''
    }
  } catch (err) {
    error.value = 'Error al cargar perfil'
  } finally {
    loading.value = false
  }
}

const guardarPerfil = async () => {
  guardando.value = true
  formError.value = ''
  try {
    await portalService.updateProfile(form.value)
    editando.value = false
    await cargarPerfil()
  } catch (err) {
    formError.value = err.response?.data?.message || 'Error al actualizar perfil'
  } finally {
    guardando.value = false
  }
}

onMounted(cargarPerfil)
</script>
