<template>
  <div>
    <PageHeader
      title="Agenda de citas"
      subtitle="Gestiona las consultas médicas programadas"
      :icon="IconAppointment"
    >
      <template #actions>
        <AppButton
          v-if="auth.hasRole('Nurse') && vista === 'lista'"
          variant="primary"
          :icon="IconAppointmentAdd"
          @click="abrirCrear"
        >
          Agendar cita
        </AppButton>
        <AppButton v-if="vista === 'crear'" variant="soft" :icon="IconBack" @click="cancelarForm">
          Volver
        </AppButton>
      </template>
    </PageHeader>

    <AppAlert v-if="error" variant="danger" :message="error" class="mb-3" />

    <!-- ============ Nueva cita ============ -->
    <BaseCard v-if="vista === 'crear'" class="u-fade-in">
      <AppAlert v-if="formError" variant="danger" :message="formError" class="mb-3" />

      <form @submit.prevent="crearCita" class="d-grid gap-3">
        <FormSection title="Información de la cita" :icon="IconRecord">
          <div class="row g-3">
            <div class="col-md-6">
              <label for="a-patient" class="form-label">Paciente</label>
              <AppSelect
                id="a-patient"
                v-model="form.patientId"
                :options="opcionesPacientes"
                placeholder="Seleccione un paciente…"
              />
            </div>
            <div class="col-md-6">
              <label for="a-doctor" class="form-label">Médico</label>
              <AppSelect
                id="a-doctor"
                v-model="form.doctorId"
                :options="opcionesDoctores"
                placeholder="Seleccione un médico…"
              />
            </div>
            <div class="col-md-6">
              <label for="a-date" class="form-label">Fecha y hora</label>
              <AppDatePicker
                id="a-date"
                v-model="form.appointmentDate"
                mode="datetime"
                placeholder="Elegir fecha y hora…"
              />
            </div>
          </div>
        </FormSection>

        <FormSection title="Detalle de la consulta" :icon="IconNotes" tone="info">
          <div class="row g-3">
            <div class="col-12">
              <label for="a-reason" class="form-label">Motivo de consulta</label>
              <input id="a-reason" v-model="form.reason" type="text" class="form-control" placeholder="Razón principal de la consulta" required>
            </div>
            <div class="col-12">
              <label for="a-notes" class="form-label">
                Notas <span class="text-app-subtle fw-normal">(opcional)</span>
              </label>
              <textarea id="a-notes" v-model="form.notes" class="form-control" rows="3" placeholder="Notas adicionales…" />
            </div>
          </div>
        </FormSection>

        <div class="d-flex justify-content-end gap-2">
          <AppButton type="button" variant="soft" @click="cancelarForm">Cancelar</AppButton>
          <AppButton type="submit" variant="primary" :icon="IconAppointmentAdd" :loading="saving">
            {{ saving ? 'Guardando…' : 'Crear cita' }}
          </AppButton>
        </div>
      </form>
    </BaseCard>

    <!-- ============ Listado ============ -->
    <template v-else>
      <FilterBar>
        <div class="input-group flex-grow-1">
          <span class="input-group-text"><Icon :icon="IconSearch" :size="16" /></span>
          <input
            v-model="filtro"
            type="search"
            class="form-control"
            placeholder="Buscar por paciente o médico…"
            aria-label="Buscar citas"
          >
        </div>
      </FilterBar>

      <DataTable
        :columns="columnas"
        :rows="citasFiltradas"
        :loading="loading"
        loading-label="Cargando citas…"
        :empty-icon="IconAppointment"
        empty-title="No hay citas registradas"
        :empty-message="filtro ? 'Ninguna cita coincide con la búsqueda.' : 'Agenda la primera consulta para verla aquí.'"
      >
        <template #cell-paciente="{ row }">
          <span class="fw-semibold text-body-emphasis">
            {{ row.patientName || getPatientName(row.patientId) }}
          </span>
        </template>
        <template #cell-medico="{ row }">{{ row.doctorName || getDoctorName(row.doctorId) }}</template>
        <template #cell-appointmentDate="{ row }">{{ formatDateTime(row.appointmentDate) }}</template>
        <template #cell-status="{ row }">
          <StatusBadge :text="translateStatus(row.status)" :variant="statusVariant(row.status)" dot />
        </template>

        <template #actions="{ row }">
          <template v-if="row.status === 'Pending'">
            <AppButton variant="soft-primary" size="sm" @click="iniciarCita(row)">Iniciar</AppButton>
            <AppButton variant="soft" size="sm" @click="completarCita(row)">Completar</AppButton>
          </template>
          <AppButton
            variant="ghost"
            size="sm"
            :icon="IconForward"
            :to="`/citas/${row.id}`"
            aria-label="Ir al detalle de la cita"
          />
        </template>
      </DataTable>
    </template>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  AppAlert,
  AppButton,
  AppDatePicker,
  AppSelect,
  BaseCard,
  DataTable,
  FilterBar,
  FormSection,
  Icon,
  PageHeader,
  StatusBadge
} from '@/components/ui'
import {
  IconAppointment,
  IconAppointmentAdd,
  IconBack,
  IconForward,
  IconNotes,
  IconRecord,
  IconSearch
} from '@/lib/icons'
import { appointmentService } from '@/api/appointments'
import { patientService } from '@/api/patients'
import { userService } from '@/api/users'
import { authStore } from '@/stores/auth'
import { statusVariant, translateStatus } from '@/utils/appointmentStatus'

const router = useRouter()
const auth = authStore
const loading = ref(true)
const error = ref('')
const saving = ref(false)
const formError = ref('')
const citas = ref([])
const pacientes = ref([])
const doctores = ref([])
const vista = ref('lista')
const filtro = ref('')

const EMPTY_FORM = { patientId: '', doctorId: '', appointmentDate: '', reason: '', notes: '' }
const form = reactive({ ...EMPTY_FORM })

const opcionesPacientes = computed(() =>
  pacientes.value.map((p) => ({ value: p.id, label: `${p.fullName} — ${p.identificationNumber}` }))
)

const opcionesDoctores = computed(() =>
  doctores.value.map((d) => ({ value: d.id, label: `${d.name} ${d.lastName}` }))
)

const columnas = [
  { key: 'paciente', label: 'Paciente' },
  { key: 'medico', label: 'Médico' },
  { key: 'appointmentDate', label: 'Fecha de cita' },
  { key: 'reason', label: 'Motivo' },
  { key: 'status', label: 'Estado' }
]

const citasFiltradas = computed(() => {
  const q = filtro.value.trim().toLowerCase()
  if (!q) return citas.value
  return citas.value.filter(
    (c) => c.patientName?.toLowerCase().includes(q) || c.doctorName?.toLowerCase().includes(q)
  )
})

const formatDateTime = (dateStr) => {
  if (!dateStr) return '—'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch {
    return dateStr
  }
}

const getPatientName = (id) =>
  pacientes.value.find((p) => p.id === id)?.fullName ?? `Paciente #${id}`

const getDoctorName = (id) => {
  const d = doctores.value.find((x) => x.id === id)
  return d ? `${d.name} ${d.lastName}` : `Médico #${id}`
}

const cargarCitas = async () => {
  loading.value = true
  error.value = ''
  try {
    const [citasRes, pacientesRes] = await Promise.all([
      appointmentService.getAll(),
      patientService.getAll()
    ])
    citas.value = citasRes.data || []
    pacientes.value = pacientesRes.data || []

    if (auth.hasRole('Nurse')) {
      try {
        const doctoresRes = await userService.getDoctors()
        doctores.value = doctoresRes.data || []
      } catch {
        doctores.value = []
      }
    }
  } catch (err) {
    if (err.response?.status === 404 || err.response?.status === 204) {
      citas.value = []
    } else {
      error.value = 'Error al cargar citas'
    }
  } finally {
    loading.value = false
  }
}

const abrirCrear = () => {
  vista.value = 'crear'
  formError.value = ''
  Object.assign(form, EMPTY_FORM)
}

const cancelarForm = () => {
  vista.value = 'lista'
}

const crearCita = async () => {
  // AppSelect y AppDatePicker no son controles nativos: `required` ya no los
  // valida, así que los campos obligatorios se comprueban aquí.
  if (!form.patientId || !form.doctorId || !form.appointmentDate) {
    formError.value = 'Paciente, médico y fecha son obligatorios.'
    return
  }

  saving.value = true
  formError.value = ''
  try {
    await appointmentService.create({
      patientId: Number(form.patientId),
      doctorId: form.doctorId,
      appointmentDate: form.appointmentDate,
      reason: form.reason,
      notes: form.notes || null
    })
    vista.value = 'lista'
    await cargarCitas()
  } catch (err) {
    const data = err.response?.data
    if (data?.errors && Array.isArray(data.errors)) {
      formError.value = data.errors.join(', ')
    } else if (data?.errors && typeof data.errors === 'object') {
      formError.value = Object.values(data.errors).flat().join(', ')
    } else {
      formError.value = data?.message || data?.detail || 'Error al crear la cita'
    }
  } finally {
    saving.value = false
  }
}

const iniciarCita = async (c) => {
  try {
    await appointmentService.changeStatus(c.id, 'InProgress')
    router.push(`/citas/${c.id}`)
  } catch {
    error.value = 'Error al iniciar la cita'
  }
}

const completarCita = async (c) => {
  try {
    await appointmentService.changeStatus(c.id, 'Completed')
    await cargarCitas()
  } catch {
    error.value = 'Error al completar la cita'
  }
}

onMounted(cargarCitas)
</script>
