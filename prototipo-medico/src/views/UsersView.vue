<template>
  <div>
    <PageHeader
      title="Gestión de usuarios"
      subtitle="Administración de cuentas del sistema"
      :icon="IconRoles"
    >
      <template #actions>
        <AppButton v-if="vista === 'lista'" variant="primary" :icon="IconUserAdd" @click="abrirCrear">
          Crear usuario
        </AppButton>
        <AppButton v-else variant="soft" :icon="IconBack" @click="cancelarForm">Volver</AppButton>
      </template>
    </PageHeader>

    <AppAlert v-if="error" variant="danger" :message="error" class="mb-3" />

    <!-- ============ Crear / editar ============ -->
    <BaseCard v-if="vista === 'crear' || vista === 'editar'" class="u-fade-in">
      <AppAlert v-if="formError" variant="danger" :message="formError" class="mb-3" />

      <form @submit.prevent="guardarUsuario" class="d-grid gap-3">
        <FormSection title="Datos del usuario" :icon="IconUser">
          <div class="row g-3">
            <div class="col-md-6">
              <label for="u-name" class="form-label">Nombre</label>
              <input id="u-name" v-model="form.name" type="text" class="form-control" placeholder="Nombre del usuario" required>
            </div>
            <div class="col-md-6">
              <label for="u-lastname" class="form-label">Apellido</label>
              <input id="u-lastname" v-model="form.lastName" type="text" class="form-control" placeholder="Apellido del usuario" required>
            </div>
            <div class="col-md-6">
              <label for="u-user" class="form-label">Nombre de usuario</label>
              <input id="u-user" v-model="form.userName" type="text" class="form-control" placeholder="Nombre de usuario" required>
            </div>
            <div class="col-md-6">
              <label for="u-id" class="form-label">Identificación</label>
              <input
                id="u-id"
                v-model="form.numberIdentification"
                type="text"
                class="form-control"
                maxlength="20"
                placeholder="Ej: 001-1234567-8"
                required
              >
              <small v-if="form.numberIdentification.length === 20" class="text-danger">
                Has alcanzado el máximo de 20 caracteres.
              </small>
            </div>
            <div class="col-md-6">
              <label for="u-email" class="form-label">Correo electrónico</label>
              <input id="u-email" v-model="form.email" type="email" class="form-control" placeholder="correo@ejemplo.com" required>
            </div>
            <div class="col-md-6">
              <label for="u-role" class="form-label">Rol</label>
              <select
                id="u-role"
                v-model="form.role"
                class="form-select"
                :disabled="vista === 'editar' && form.role === 'Patient'"
                required
              >
                <option v-if="form.role === 'Patient'" value="Patient">Paciente</option>
                <template v-else>
                  <option value="Doctor">Médico</option>
                  <option value="Nurse">Enfermero</option>
                  <option value="Administrator">Administrador</option>
                </template>
              </select>
            </div>
            <div v-if="form.role === 'Doctor'" class="col-md-6">
              <label for="u-specialty" class="form-label">Especialidad</label>
              <select id="u-specialty" v-model="form.specialty" class="form-select">
                <option value="">Sin especificar</option>
                <option value="Cardiología">Cardiología</option>
                <option value="Dermatología">Dermatología</option>
                <option value="Endocrinología">Endocrinología</option>
                <option value="Gastroenterología">Gastroenterología</option>
                <option value="Ginecología">Ginecología</option>
                <option value="Medicina Interna">Medicina Interna</option>
                <option value="Neurología">Neurología</option>
                <option value="Oftalmología">Oftalmología</option>
                <option value="Oncología">Oncología</option>
                <option value="Ortopedia">Ortopedia</option>
                <option value="Pediatría">Pediatría</option>
                <option value="Psiquiatría">Psiquiatría</option>
                <option value="Neumología">Neumología</option>
                <option value="Reumatología">Reumatología</option>
                <option value="Urología">Urología</option>
              </select>
            </div>
          </div>
        </FormSection>

        <AppAlert
          v-if="vista === 'crear'"
          variant="info"
          :icon="IconPassword"
          message="Se generará una contraseña automática y se enviará por correo al nuevo usuario."
        />

        <div class="d-flex justify-content-end gap-2">
          <AppButton type="button" variant="soft" @click="cancelarForm">Cancelar</AppButton>
          <AppButton type="submit" variant="primary" :icon="IconSave" :loading="saving">
            {{ saving ? 'Guardando…' : vista === 'editar' ? 'Actualizar usuario' : 'Crear usuario' }}
          </AppButton>
        </div>
      </form>
    </BaseCard>

    <!-- ============ Listado ============ -->
    <DataTable
      v-else
      :columns="columnas"
      :rows="users"
      :loading="loading"
      loading-label="Cargando usuarios…"
      :empty-icon="IconRoles"
      empty-title="No hay usuarios registrados"
      empty-message="Crea la primera cuenta del sistema para empezar."
    >
      <template #cell-nombre="{ row }">
        <div class="d-flex align-items-center gap-2">
          <AvatarInitials :name="row.name" :last-name="row.lastName" size="sm" />
          <span class="fw-semibold text-body-emphasis">{{ row.name }} {{ row.lastName }}</span>
        </div>
      </template>
      <template #cell-role="{ row }">
        <StatusBadge :text="translateRole(row.role)" variant="info" size="sm" />
      </template>
      <template #cell-status="{ row }">
        <StatusBadge
          :text="row.status ? 'Activo' : 'Inactivo'"
          :variant="row.status ? 'active' : 'inactive'"
          dot
        />
      </template>

      <template #actions="{ row }">
        <StatusBadge v-if="row.id === auth.user?.id" text="Mi cuenta" variant="secondary" size="sm" />
        <template v-else>
          <AppButton variant="ghost" size="sm" :icon="IconEdit" aria-label="Editar usuario" @click="abrirEditar(row)" />
          <AppButton
            :variant="row.status ? 'soft-danger' : 'soft-primary'"
            size="sm"
            @click="toggleStatus(row)"
          >
            {{ row.status ? 'Inactivar' : 'Activar' }}
          </AppButton>
        </template>
      </template>
    </DataTable>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import {
  AppAlert,
  AppButton,
  AvatarInitials,
  BaseCard,
  DataTable,
  FormSection,
  PageHeader,
  StatusBadge
} from '@/components/ui'
import {
  IconBack,
  IconEdit,
  IconPassword,
  IconRoles,
  IconSave,
  IconUser,
  IconUserAdd
} from '@/lib/icons'
import { accountService } from '@/api/account'
import { authStore } from '@/stores/auth'
import { translateRole } from '@/utils/roles'

const auth = authStore
const loading = ref(true)
const error = ref('')
const saving = ref(false)
const formError = ref('')
const users = ref([])
const vista = ref('lista')

const EMPTY_FORM = {
  id: null,
  name: '',
  lastName: '',
  userName: '',
  email: '',
  numberIdentification: '',
  role: 'Doctor',
  specialty: ''
}

const form = reactive({ ...EMPTY_FORM })

const columnas = [
  { key: 'nombre', label: 'Nombre completo' },
  { key: 'userName', label: 'Usuario' },
  { key: 'email', label: 'Correo' },
  { key: 'role', label: 'Rol' },
  { key: 'status', label: 'Estado' }
]

const cargarUsuarios = async () => {
  loading.value = true
  error.value = ''
  try {
    const res = await accountService.getAll()
    users.value = res.data || []
  } catch (err) {
    if (err.response?.status === 404 || err.response?.status === 204) {
      users.value = []
    } else {
      error.value = 'Error al cargar usuarios'
    }
  } finally {
    loading.value = false
  }
}

const abrirCrear = () => {
  vista.value = 'crear'
  Object.assign(form, EMPTY_FORM)
  formError.value = ''
}

const abrirEditar = (u) => {
  vista.value = 'editar'
  Object.assign(form, {
    id: u.id,
    name: u.name,
    lastName: u.lastName,
    userName: u.userName,
    email: u.email,
    numberIdentification: u.numberIdentification,
    role: u.role,
    specialty: u.specialty || ''
  })
  formError.value = ''
}

const cancelarForm = () => {
  vista.value = 'lista'
}

const guardarUsuario = async () => {
  saving.value = true
  formError.value = ''
  try {
    const res =
      vista.value === 'editar'
        ? await accountService.update(form.id, { ...form })
        : await accountService.create({ ...form })

    if (res.data.hasError) {
      formError.value = res.data.errors?.join(', ') || 'Error al guardar usuario'
      return
    }
    vista.value = 'lista'
    await cargarUsuarios()
  } catch (err) {
    formError.value = err.response?.data?.errors?.join(', ') || 'Error al guardar usuario'
  } finally {
    saving.value = false
  }
}

const toggleStatus = async (u) => {
  try {
    const res = await accountService.toggleStatus(u.id)
    if (res.data.hasError) {
      error.value = res.data.errors?.join(', ')
      return
    }
    await cargarUsuarios()
  } catch (err) {
    error.value = err.response?.data?.message || 'Error al cambiar estado'
  }
}

onMounted(cargarUsuarios)
</script>
