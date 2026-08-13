<template>
  <div>
    <!-- ============ Listado ============ -->
    <div v-if="vistaActual === 'lista'" class="u-fade-in">
      <PageHeader
        title="Gestión de pacientes"
        subtitle="Directorio general de pacientes registrados"
        :icon="IconPatients"
      >
        <template #actions>
          <AppButton
            v-if="!auth.hasRole('Doctor')"
            variant="primary"
            :icon="IconAdd"
            @click="vistaActual = 'crear'"
          >
            Crear paciente
          </AppButton>
        </template>
      </PageHeader>

      <FilterBar>
        <div class="input-group flex-grow-1">
          <span class="input-group-text">
            <Icon :icon="IconSearch" :size="16" />
          </span>
          <input
            v-model="busqueda"
            type="search"
            class="form-control"
            placeholder="Buscar por nombre completo o identificación…"
            aria-label="Buscar pacientes"
            @keyup.enter="buscarApi"
          >
        </div>
        <template #actions>
          <AppButton variant="primary" :loading="buscandoApi" @click="buscarApi">Buscar</AppButton>
          <AppButton v-if="busqueda" variant="soft" @click="limpiarBusqueda">Limpiar</AppButton>
        </template>
      </FilterBar>

      <DataTable
        :columns="columnasPacientes"
        :rows="pacientesFiltrados"
        :loading="loading"
        :error="error"
        loading-label="Cargando pacientes…"
        :empty-icon="IconPatients"
        empty-title="No se encontraron pacientes"
        :empty-message="busqueda ? 'Prueba con otro nombre o número de identificación.' : 'Todavía no hay pacientes registrados.'"
      >
        <template #cell-fullName="{ row }">
          <div class="d-flex align-items-center gap-2">
            <AvatarInitials :name="row.fullName" size="sm" />
            <span class="fw-semibold text-body-emphasis">{{ row.fullName }}</span>
          </div>
        </template>
        <template #cell-birthDate="{ row }">{{ formatDate(row.birthDate) }}</template>
        <template #cell-createdAt="{ row }">{{ formatDate(row.createdAt) }}</template>
        <template #cell-gender="{ row }">
          <StatusBadge :text="row.gender || '—'" variant="secondary" size="sm" />
        </template>

        <template #actions="{ row }">
          <AppButton variant="ghost" size="sm" :icon="IconView" aria-label="Ver expediente" @click="abrirDetalle(row)" />
          <AppButton variant="ghost" size="sm" :icon="IconEdit" aria-label="Editar paciente" @click="editarPaciente(row)" />
          <AppButton variant="ghost" size="sm" :icon="IconDelete" aria-label="Eliminar paciente" @click="confirmarEliminar(row)" />
        </template>
      </DataTable>
    </div>

    <!-- ============ Crear / editar ============ -->
    <div v-else-if="vistaActual === 'crear' || vistaActual === 'editar'" class="u-fade-in">
      <PageHeader
        :title="vistaActual === 'crear' ? 'Nuevo paciente' : 'Editar paciente'"
        :subtitle="vistaActual === 'editar' ? `Modificando datos de ${form.fullName}` : 'Registra un paciente en el sistema'"
        :icon="vistaActual === 'crear' ? IconUserAdd : IconEdit"
      >
        <template #actions>
          <AppButton variant="soft" :icon="IconBack" @click="cancelarForm">Volver</AppButton>
        </template>
      </PageHeader>

      <BaseCard>
        <AppAlert v-if="formError" variant="danger" :message="formError" class="mb-3" />

        <form @submit.prevent="guardarPaciente" class="d-grid gap-3">
          <FormSection title="Identificación" :icon="IconIdentification">
            <div class="row g-3">
              <div class="col-md-4">
                <label for="p-idtype" class="form-label">Tipo de identificación</label>
                <select id="p-idtype" v-model="form.identificationType" class="form-select" required>
                  <option value="Cédula">Cédula</option>
                  <option value="Pasaporte">Pasaporte</option>
                </select>
              </div>
              <div class="col-md-8">
                <label for="p-idnum" class="form-label">Número de identificación</label>
                <input id="p-idnum" v-model="form.identificationNumber" type="text" class="form-control" placeholder="Ej: 001-1234567-8" required>
              </div>
            </div>
          </FormSection>

          <FormSection title="Datos personales" :icon="IconUser" tone="info">
            <div class="row g-3">
              <div class="col-12">
                <label for="p-name" class="form-label">Nombre completo</label>
                <input id="p-name" v-model="form.fullName" type="text" class="form-control" placeholder="Nombre y apellidos del paciente" required>
              </div>
              <div class="col-md-6">
                <label for="p-birth" class="form-label">Fecha de nacimiento</label>
                <input id="p-birth" v-model="form.birthDate" type="date" class="form-control" required>
              </div>
              <div class="col-md-6">
                <label for="p-gender" class="form-label">Género</label>
                <select id="p-gender" v-model="form.gender" class="form-select" required>
                  <option value="Masculino">Masculino</option>
                  <option value="Femenino">Femenino</option>
                  <option value="Otro">Otro</option>
                </select>
              </div>
            </div>
          </FormSection>

          <FormSection title="Contacto y clasificación" :icon="IconPhone" tone="success">
            <div class="row g-3">
              <div class="col-md-4">
                <label for="p-email" class="form-label">Correo electrónico</label>
                <input id="p-email" v-model="form.email" type="email" class="form-control" placeholder="correo@ejemplo.com" required>
              </div>
              <div class="col-md-4">
                <label for="p-phone" class="form-label">Teléfono</label>
                <input id="p-phone" v-model="form.phoneNumber" type="tel" class="form-control" placeholder="(809) 555-1234" required>
              </div>
              <div class="col-md-4">
                <label for="p-ptype" class="form-label">Tipo de paciente</label>
                <select id="p-ptype" v-model="form.patientType" class="form-select" required>
                  <option value="Asegurado">Asegurado</option>
                  <option value="No Asegurado">No Asegurado</option>
                </select>
              </div>
            </div>
          </FormSection>

          <div class="d-flex justify-content-end gap-2">
            <AppButton type="button" variant="soft" @click="cancelarForm">Cancelar</AppButton>
            <AppButton type="submit" variant="primary" :icon="IconSave" :loading="saving">
              {{ saving ? 'Guardando…' : 'Guardar paciente' }}
            </AppButton>
          </div>
        </form>
      </BaseCard>
    </div>

    <!-- ============ Detalle ============ -->
    <div v-else-if="vistaActual === 'detalle'" class="u-fade-in">
      <PageHeader
        :title="detalle?.fullName || 'Expediente'"
        subtitle="Expediente completo del paciente"
        :icon="IconUser"
      >
        <template #actions>
          <AppButton variant="soft" :icon="IconBack" @click="cancelarForm">Volver al listado</AppButton>
        </template>
      </PageHeader>

      <div class="d-grid gap-3">
        <BaseCard title="Datos del paciente" :icon="IconIdentification">
          <div class="row g-4">
            <div class="col-md-4"><DataField label="Identificación" :value="detalle?.identificationNumber" /></div>
            <div class="col-md-4"><DataField label="Teléfono" :value="detalle?.phoneNumber" /></div>
            <div class="col-md-4"><DataField label="Género" :value="detalle?.gender" /></div>
            <div class="col-md-4"><DataField label="Fecha de nacimiento" :value="formatDate(detalle?.birthDate)" /></div>
            <div class="col-md-4"><DataField label="Tipo de paciente" :value="detalle?.patientType" /></div>
            <div class="col-md-4"><DataField label="Registrado" :value="formatDate(detalle?.createdAt)" /></div>
          </div>
        </BaseCard>

        <DataTable
          :columns="columnasCitas"
          :rows="detalle?.appointments ?? []"
          :empty-icon="IconAppointment"
          empty-title="Sin citas registradas"
          empty-message="Este paciente todavía no tiene citas en su historial."
        >
          <template #cell-appointmentDate="{ row }">{{ formatDate(row.appointmentDate) }}</template>
          <template #cell-status="{ row }">
            <StatusBadge :text="row.status" :variant="row.status?.toLowerCase()" />
          </template>
        </DataTable>
      </div>
    </div>

    <ConfirmDialog
      :visible="deleteDialog"
      danger
      title="Eliminar paciente"
      :message="`¿Seguro que deseas eliminar a ${deleteTarget?.fullName ?? 'este paciente'} y todo su historial clínico? Esta acción no se puede deshacer.`"
      confirm-text="Eliminar"
      @confirm="eliminarPaciente"
      @cancel="deleteDialog = false"
    />
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import {
  AppAlert,
  AppButton,
  AvatarInitials,
  BaseCard,
  ConfirmDialog,
  DataField,
  DataTable,
  FilterBar,
  FormSection,
  Icon,
  PageHeader,
  StatusBadge
} from '@/components/ui'
import {
  IconAdd,
  IconAppointment,
  IconBack,
  IconDelete,
  IconEdit,
  IconIdentification,
  IconPatients,
  IconPhone,
  IconSave,
  IconSearch,
  IconUser,
  IconUserAdd,
  IconView
} from '@/lib/icons'
import { patientService } from '@/api/patients'
import { authStore } from '@/stores/auth'

const auth = authStore
const busqueda = ref('')
const vistaActual = ref('lista')
const loading = ref(true)
const error = ref('')
const saving = ref(false)
const formError = ref('')
const deleteDialog = ref(false)
const deleteTarget = ref(null)
const pacientes = ref([])
const detalle = ref(null)
const editId = ref(null)
const editUserId = ref(null)
const buscandoApi = ref(false)

const EMPTY_FORM = {
  fullName: '',
  identificationNumber: '',
  identificationType: 'Cédula',
  birthDate: '',
  gender: 'Masculino',
  phoneNumber: '',
  email: '',
  patientType: 'Asegurado'
}

const form = reactive({ ...EMPTY_FORM })

const columnasPacientes = [
  { key: 'fullName', label: 'Nombre completo' },
  { key: 'identificationNumber', label: 'Identificación' },
  { key: 'birthDate', label: 'F. nacimiento' },
  { key: 'phoneNumber', label: 'Teléfono' },
  { key: 'gender', label: 'Género' },
  { key: 'createdAt', label: 'Registro' }
]

const columnasCitas = [
  { key: 'appointmentDate', label: 'Fecha' },
  { key: 'doctorName', label: 'Médico' },
  { key: 'reason', label: 'Motivo' },
  { key: 'status', label: 'Estado' }
]

const pacientesFiltrados = computed(() => {
  const list = pacientes.value.filter((p) => p.fullName)
  if (!busqueda.value) return list
  const q = busqueda.value.toLowerCase()
  return list.filter(
    (p) => p.fullName.toLowerCase().includes(q) || p.identificationNumber?.includes(q)
  )
})

const formatDate = (dateStr) => {
  if (!dateStr || dateStr.startsWith('0001-01-01')) return '—'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    })
  } catch {
    return dateStr
  }
}

const cargarPacientes = async () => {
  loading.value = true
  error.value = ''
  try {
    const res = await patientService.getAll()
    pacientes.value = res.data || []
  } catch (err) {
    if (err.response?.status === 404 || err.response?.status === 204) {
      pacientes.value = []
    } else {
      error.value = 'Error al cargar pacientes'
    }
  } finally {
    loading.value = false
  }
}

const abrirDetalle = async (p) => {
  vistaActual.value = 'detalle'
  try {
    const res = await patientService.getDetails(p.id)
    detalle.value = { ...p, ...res.data }
  } catch {
    detalle.value = { ...p }
  }
}

const editarPaciente = async (p) => {
  editId.value = p.id
  try {
    const res = await patientService.getById(p.id)
    editUserId.value = res.data?.userId || p.userId
  } catch {
    editUserId.value = p.userId
  }
  Object.assign(form, {
    fullName: p.fullName || '',
    identificationNumber: p.identificationNumber || '',
    identificationType: p.identificationType || 'Cédula',
    birthDate: p.birthDate ? p.birthDate.substring(0, 10) : '',
    gender: p.gender || 'Masculino',
    phoneNumber: p.phoneNumber || '',
    email: p.email || '',
    patientType: p.patientType || 'Asegurado'
  })
  vistaActual.value = 'editar'
  formError.value = ''
}

const guardarPaciente = async () => {
  saving.value = true
  formError.value = ''
  try {
    if (vistaActual.value === 'crear') {
      await patientService.create({ ...form })
    } else {
      await patientService.update(editId.value, { ...form, userId: editUserId.value })
    }
    vistaActual.value = 'lista'
    await cargarPacientes()
  } catch (err) {
    const data = err.response?.data
    if (data?.errors && Array.isArray(data.errors)) {
      formError.value = data.errors.join(', ')
    } else if (data?.errors && typeof data.errors === 'object') {
      formError.value = Object.values(data.errors).flat().join(', ')
    } else {
      formError.value = data?.message || data?.detail || 'Error al guardar paciente'
    }
  } finally {
    saving.value = false
  }
}

const cancelarForm = () => {
  vistaActual.value = 'lista'
  detalle.value = null
  editId.value = null
  editUserId.value = null
  Object.assign(form, EMPTY_FORM)
}

const confirmarEliminar = (p) => {
  deleteTarget.value = p
  deleteDialog.value = true
}

const buscarApi = async () => {
  if (!busqueda.value) {
    await cargarPacientes()
    return
  }
  buscandoApi.value = true
  try {
    const res = await patientService.search(busqueda.value)
    pacientes.value = res.data || []
  } catch {
    error.value = 'Error en la búsqueda'
  } finally {
    buscandoApi.value = false
  }
}

const limpiarBusqueda = () => {
  busqueda.value = ''
  cargarPacientes()
}

const eliminarPaciente = async () => {
  deleteDialog.value = false
  try {
    await patientService.remove(deleteTarget.value.id)
    await cargarPacientes()
  } catch {
    error.value = 'Error al eliminar paciente'
  }
}

onMounted(cargarPacientes)
</script>
