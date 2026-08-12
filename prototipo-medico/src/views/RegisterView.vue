<template>
  <AuthCard
    wide
    title="Solicitar acceso como paciente"
    subtitle="Completa tus datos para crear una cuenta en el portal"
  >
    <AppAlert v-if="successMsg" variant="success" :message="successMsg" class="mb-3" />
    <AppAlert v-if="errorMsg" variant="danger" :message="errorMsg" class="mb-3" />

    <form @submit.prevent="registrar" class="d-grid gap-3">
      <FormSection title="Datos personales" :icon="IconUser">
        <div class="row g-3">
          <div class="col-md-6">
            <label for="r-name" class="form-label">Nombre</label>
            <input
              id="r-name"
              v-model="form.name"
              type="text"
              class="form-control"
              placeholder="Tu nombre"
              required
              :pattern="LETTERS_PATTERN"
              title="El nombre solo puede contener letras"
            >
          </div>
          <div class="col-md-6">
            <label for="r-lastname" class="form-label">Apellido</label>
            <input
              id="r-lastname"
              v-model="form.lastName"
              type="text"
              class="form-control"
              placeholder="Tu apellido"
              required
              :pattern="LETTERS_PATTERN"
              title="El apellido solo puede contener letras"
            >
          </div>
          <div class="col-md-6">
            <label for="r-user" class="form-label">Nombre de usuario</label>
            <input
              id="r-user"
              v-model="form.userName"
              type="text"
              class="form-control"
              placeholder="Elige un nombre de usuario"
              autocomplete="username"
              required
            >
          </div>
          <div class="col-md-6">
            <label for="r-id" class="form-label">Identificación</label>
            <input
              id="r-id"
              v-model="form.numberIdentification"
              type="text"
              class="form-control"
              placeholder="Ej: 001-1234567-8"
              required
            >
          </div>
          <div class="col-12">
            <label for="r-email" class="form-label">Correo electrónico</label>
            <input
              id="r-email"
              v-model="form.email"
              type="email"
              class="form-control"
              placeholder="correo@ejemplo.com"
              autocomplete="email"
              required
            >
          </div>
        </div>
      </FormSection>

      <FormSection title="Datos clínicos" :icon="IconClinical" tone="info">
        <div class="row g-3">
          <div class="col-md-6">
            <label for="r-phone" class="form-label">Teléfono</label>
            <input
              id="r-phone"
              v-model="form.phoneNumber"
              type="tel"
              class="form-control"
              placeholder="Ej: 809-555-0000"
            >
          </div>
          <div class="col-md-6">
            <label for="r-gender" class="form-label">Género</label>
            <select id="r-gender" v-model="form.gender" class="form-select">
              <option value="">Seleccione…</option>
              <option value="Masculino">Masculino</option>
              <option value="Femenino">Femenino</option>
              <option value="Otro">Otro</option>
            </select>
          </div>
          <div class="col-md-6">
            <label for="r-birth" class="form-label">Fecha de nacimiento</label>
            <input id="r-birth" v-model="form.birthDate" type="date" class="form-control">
          </div>
          <div class="col-md-6">
            <label for="r-idtype" class="form-label">Tipo de identificación</label>
            <select id="r-idtype" v-model="form.identificationType" class="form-select">
              <option value="">Seleccione…</option>
              <option value="Cédula">Cédula</option>
              <option value="Pasaporte">Pasaporte</option>
              <option value="Otro">Otro</option>
            </select>
          </div>
          <div class="col-md-6">
            <label for="r-ptype" class="form-label">Tipo de paciente</label>
            <select id="r-ptype" v-model="form.patientType" class="form-select">
              <option value="">Seleccione…</option>
              <option value="Asegurado">Asegurado</option>
              <option value="No Asegurado">No Asegurado</option>
            </select>
          </div>
        </div>
      </FormSection>

      <FormSection title="Credenciales" :icon="IconSecurity" tone="success">
        <div class="row g-3">
          <div class="col-md-6">
            <label for="r-pass" class="form-label">Contraseña</label>
            <input
              id="r-pass"
              v-model="form.password"
              type="password"
              class="form-control"
              placeholder="••••••••"
              autocomplete="new-password"
              required
            >
          </div>
          <div class="col-md-6">
            <label for="r-pass2" class="form-label">Confirmar contraseña</label>
            <input
              id="r-pass2"
              v-model="form.confirmPassword"
              type="password"
              class="form-control"
              :class="{ 'is-invalid': mismatch }"
              placeholder="••••••••"
              autocomplete="new-password"
              required
            >
            <div v-if="mismatch" class="invalid-feedback d-block">
              Las contraseñas no coinciden.
            </div>
          </div>
        </div>
      </FormSection>

      <AppButton type="submit" variant="primary" size="lg" block :loading="loading">
        {{ loading ? 'Registrando…' : 'Registrar cuenta' }}
      </AppButton>
    </form>

    <template #footer>
      <RouterLink to="/login">¿Ya tienes cuenta? Inicia sesión</RouterLink>
    </template>
  </AuthCard>
</template>

<script setup>
import { computed, reactive, ref } from 'vue'
import AuthCard from '@/layouts/AuthCard.vue'
import AppAlert from '@/components/ui/AppAlert.vue'
import AppButton from '@/components/ui/AppButton.vue'
import FormSection from '@/components/ui/FormSection.vue'
import { IconClinical, IconSecurity, IconUser } from '@/lib/icons'
import { authService } from '@/api/auth'

const LETTERS_PATTERN = "[A-Za-záéíóúÁÉÍÓÚüÜñÑ\\s\\-']+"
const LETTERS_RE = /^[A-Za-záéíóúÁÉÍÓÚüÜñÑ\s\-']+$/

const EMPTY_FORM = {
  name: '',
  lastName: '',
  userName: '',
  email: '',
  numberIdentification: '',
  password: '',
  confirmPassword: '',
  phoneNumber: '',
  gender: '',
  birthDate: '',
  identificationType: '',
  patientType: ''
}

const loading = ref(false)
const successMsg = ref('')
const errorMsg = ref('')
const form = reactive({ ...EMPTY_FORM })

const mismatch = computed(
  () => form.confirmPassword.length > 0 && form.password !== form.confirmPassword
)

const registrar = async () => {
  loading.value = true
  successMsg.value = ''
  errorMsg.value = ''

  try {
    if (!LETTERS_RE.test(form.name)) {
      errorMsg.value = 'El nombre solo puede contener letras.'
      return
    }
    if (!LETTERS_RE.test(form.lastName)) {
      errorMsg.value = 'El apellido solo puede contener letras.'
      return
    }
    if (form.password !== form.confirmPassword) {
      errorMsg.value = 'Las contraseñas no coinciden'
      return
    }

    const res = await authService.registerPatient({ ...form })
    if (res.data.hasError) {
      errorMsg.value = res.data.errors?.join(', ') || 'Error al registrar'
      return
    }
    successMsg.value = 'Cuenta registrada exitosamente. Revisa tu correo para confirmar.'
    Object.assign(form, EMPTY_FORM)
  } catch (err) {
    errorMsg.value = err.response?.data?.errors?.join(', ') || 'Error al registrar el usuario'
  } finally {
    // En un `finally`: antes cada retorno de validación tenía que acordarse de
    // apagar el spinner a mano, y era fácil olvidarse en la siguiente rama.
    loading.value = false
  }
}
</script>
