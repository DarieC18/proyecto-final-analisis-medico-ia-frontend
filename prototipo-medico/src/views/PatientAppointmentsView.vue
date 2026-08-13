<template>
  <div>
    <PageHeader title="Mis Citas" subtitle="Historial de citas médicas" :icon="IconAppointment">
      <template #actions>
        <AppButton
          v-if="vista === 'lista'"
          variant="primary"
          :icon="IconAppointmentAdd"
          @click="abrirCrear"
        >
          Solicitar cita
        </AppButton>
        <AppButton v-else variant="soft" :icon="IconBack" @click="vista = 'lista'">Volver</AppButton>
      </template>
    </PageHeader>

    <AppAlert v-if="error" variant="danger" :message="error" class="mb-3" />

    <LoadingState v-if="loading" label="Cargando citas…" />

    <!-- ============ Solicitar cita ============ -->
    <BaseCard v-else-if="vista === 'crear'" class="u-fade-in">
      <AppAlert v-if="formError" variant="danger" :message="formError" class="mb-3" />

      <form class="d-grid gap-3" @submit.prevent="solicitar">
        <FormSection title="Datos de la solicitud" :icon="IconAppointment">
          <div class="row g-3">
            <div class="col-md-6">
              <label for="p-doctor" class="form-label">
                Médico <span class="text-app-muted fw-normal">(opcional)</span>
              </label>
              <AppSelect id="p-doctor" v-model="form.doctorId" :options="opcionesDoctores" />
            </div>
            <div class="col-md-6">
              <label for="p-fecha" class="form-label">Fecha de la cita</label>
              <AppDatePicker
                id="p-fecha"
                v-model="form.appointmentDate"
                mode="datetime"
                :min="hoyISO"
                placeholder="Elegir fecha y hora…"
              />
            </div>
            <div class="col-12">
              <label for="p-motivo" class="form-label">Motivo de consulta</label>
              <input
                id="p-motivo"
                v-model="form.reason"
                type="text"
                class="form-control"
                placeholder="Razón principal de la consulta"
                required
              />
            </div>
            <div class="col-12">
              <label for="p-notas" class="form-label">
                Notas <span class="text-app-muted fw-normal">(opcional)</span>
              </label>
              <textarea id="p-notas" v-model="form.notes" class="form-control" rows="3" placeholder="Notas adicionales…" />
            </div>
          </div>
        </FormSection>

        <div class="d-flex justify-content-end gap-2">
          <AppButton variant="soft" @click="vista = 'lista'">Cancelar</AppButton>
          <AppButton type="submit" variant="primary" :loading="saving" :icon="IconSave">
            Solicitar cita
          </AppButton>
        </div>
      </form>
    </BaseCard>

    <!-- ============ Listado ============ -->
    <BaseCard v-else-if="citas.length === 0" flush padding="none">
      <EmptyState
        :icon="IconAppointment"
        title="No tienes citas registradas"
        message="Solicita una cita y aparecerá aquí junto a su estado."
        action-label="Solicitar cita"
        :action-icon="IconAppointmentAdd"
        @action="abrirCrear"
      />
    </BaseCard>

    <div v-else class="row g-3">
      <div v-for="c in citas" :key="c.id" class="col-md-6">
        <BaseCard class="h-100">
          <div class="d-flex justify-content-between align-items-start gap-2 mb-3">
            <div class="u-min-w-0">
              <h3 class="cita__motivo">{{ c.reason || 'Sin motivo' }}</h3>
              <small class="text-app-muted">{{ c.doctorName || 'Médico no asignado' }}</small>
            </div>
            <StatusBadge :text="translateStatus(c.status)" :variant="statusVariant(c.status)" />
          </div>
          <div class="d-flex justify-content-between align-items-center gap-2">
            <small class="text-app-muted fw-medium">{{ formatDate(c.appointmentDate) }}</small>
            <AppButton
              v-if="c.status === 'Pending'"
              variant="soft-danger"
              size="sm"
              :loading="cancelling === c.id"
              @click="cancelarCita(c)"
            >
              Cancelar
            </AppButton>
          </div>
        </BaseCard>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, onMounted } from 'vue'
import { portalService } from '@/api/portal'
import {
  AppAlert,
  AppButton,
  AppDatePicker,
  AppSelect,
  BaseCard,
  EmptyState,
  FormSection,
  LoadingState,
  PageHeader,
  StatusBadge
} from '@/components/ui'
import { IconAppointment, IconAppointmentAdd, IconBack, IconSave } from '@/lib/icons'
import { statusVariant, translateStatus } from '@/utils/appointmentStatus'

const loading = ref(true)
const error = ref('')
const citas = ref([])
const cancelling = ref(null)
const vista = ref('lista')
const doctores = ref([])
const saving = ref(false)
const formError = ref('')
const form = ref({ doctorId: '', appointmentDate: '', reason: '', notes: '' })

const opcionesDoctores = computed(() => [
  { value: '', label: 'Sin preferencia' },
  ...doctores.value.map((d) => ({
    value: d.id,
    label: d.specialty ? `${d.fullName} — ${d.specialty}` : d.fullName
  }))
])

// Una cita no puede pedirse para ayer. El input nativo no lo impedía.
const hoyISO = computed(() => {
  const n = new Date()
  return `${n.getFullYear()}-${String(n.getMonth() + 1).padStart(2, '0')}-${String(n.getDate()).padStart(2, '0')}`
})

const formatDate = (dateStr) => {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('es-ES', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const cargarCitas = async () => {
  try {
    const res = await portalService.getAppointments()
    citas.value = res.data || []
  } catch (err) {
    error.value =
      err.response?.status === 404
        ? 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
        : 'Error al cargar citas'
  }
}

const abrirCrear = async () => {
  formError.value = ''
  form.value = { doctorId: '', appointmentDate: '', reason: '', notes: '' }
  vista.value = 'crear'
  if (doctores.value.length === 0) {
    try {
      const res = await portalService.getDoctors()
      doctores.value = res.data || []
    } catch {
      // si falla, el select sólo mostrará "Sin preferencia"
    }
  }
}

const solicitar = async () => {
  // AppDatePicker no es un <input>, así que la validación nativa de `required`
  // ya no cubre este campo: se comprueba a mano antes de enviar.
  if (!form.value.appointmentDate) {
    formError.value = 'Selecciona la fecha y hora de la cita.'
    return
  }

  saving.value = true
  formError.value = ''
  try {
    await portalService.requestAppointment({
      doctorId: form.value.doctorId || null,
      appointmentDate: form.value.appointmentDate,
      reason: form.value.reason,
      notes: form.value.notes || null
    })
    await cargarCitas()
    vista.value = 'lista'
  } catch (err) {
    formError.value = err.response?.data?.message || 'Error al solicitar la cita. Intenta de nuevo.'
  } finally {
    saving.value = false
  }
}

const cancelarCita = async (c) => {
  cancelling.value = c.id
  try {
    await portalService.cancelAppointment(c.id)
    c.status = 'Cancelled'
  } catch {
    error.value = 'Error al cancelar la cita'
  } finally {
    cancelling.value = null
  }
}

onMounted(async () => {
  await cargarCitas()
  loading.value = false
})
</script>

<style scoped>
.cita__motivo {
  margin: 0;
  font-size: 0.9375rem;
  font-weight: 600;
  color: var(--app-text-strong);
}
</style>
