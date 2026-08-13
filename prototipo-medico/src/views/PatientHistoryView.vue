<template>
  <div>
    <PageHeader
      title="Historial Clínico"
      subtitle="Registro de tus consultas médicas"
      :icon="IconHistory"
    />

    <AppAlert v-if="error" variant="danger" :message="error" />

    <DataTable
      v-else
      :columns="columnas"
      :rows="records"
      :loading="loading"
      loading-label="Cargando historial…"
      :empty-icon="IconHistory"
      empty-title="Sin registros clínicos"
      empty-message="Aquí aparecerán tus consultas una vez que hayan sido registradas."
    >
      <template #cell-appointmentDate="{ value }">{{ formatDate(value) }}</template>
      <template #cell-appointmentStatus="{ value }">
        <StatusBadge :text="translateStatus(value)" :variant="statusVariant(value)" />
      </template>
    </DataTable>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { portalService } from '@/api/portal'
import { AppAlert, DataTable, PageHeader, StatusBadge } from '@/components/ui'
import { IconHistory } from '@/lib/icons'
import { statusVariant, translateStatus } from '@/utils/appointmentStatus'

const loading = ref(true)
const error = ref('')
const records = ref([])

const columnas = [
  { key: 'appointmentDate', label: 'Fecha' },
  { key: 'doctorName', label: 'Médico' },
  { key: 'reason', label: 'Motivo' },
  { key: 'appointmentStatus', label: 'Estado' }
]

const formatDate = (dateStr) => {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' })
}

onMounted(async () => {
  try {
    const res = await portalService.getMedicalRecords()
    records.value = res.data || []
  } catch (err) {
    error.value =
      err.response?.status === 404
        ? 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
        : 'Error al cargar historial'
  } finally {
    loading.value = false
  }
})
</script>
