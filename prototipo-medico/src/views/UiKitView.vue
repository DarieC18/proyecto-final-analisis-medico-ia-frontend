<template>
  <div class="ui-kit">
    <PageHeader
      title="Kit de interfaz"
      subtitle="Catálogo de componentes del sistema de diseño. Sólo disponible en desarrollo."
      :icon="IconSparkles"
    >
      <template #actions>
        <ThemeToggle with-label />
      </template>
    </PageHeader>

    <section class="ui-kit__section">
      <h2 class="ui-kit__heading">Paleta</h2>
      <div class="swatches">
        <div v-for="s in swatches" :key="s.name" class="swatch">
          <span class="swatch__chip" :style="{ background: `var(${s.token})` }" />
          <span class="swatch__name">{{ s.name }}</span>
          <code class="swatch__token">{{ s.token }}</code>
        </div>
      </div>
    </section>

    <section class="ui-kit__section">
      <h2 class="ui-kit__heading">Botones</h2>
      <div class="row-wrap">
        <AppButton v-for="v in variants" :key="v" :variant="v">{{ v }}</AppButton>
      </div>
      <div class="row-wrap">
        <AppButton variant="primary" size="sm" :icon="IconAdd">Pequeño</AppButton>
        <AppButton variant="primary" :icon="IconSave">Con icono</AppButton>
        <AppButton variant="primary" size="lg" :icon-right="IconForward">Grande</AppButton>
        <AppButton variant="primary" loading>Cargando</AppButton>
        <AppButton variant="primary" disabled>Deshabilitado</AppButton>
        <AppButton variant="soft" pill :icon="IconSearch">Píldora</AppButton>
      </div>
    </section>

    <section class="ui-kit__section">
      <h2 class="ui-kit__heading">Métricas</h2>
      <div class="grid-4">
        <StatTile label="Pacientes" :value="128" :icon="IconPatients" tone="brand" hint="+12 este mes" />
        <StatTile label="Citas hoy" :value="9" :icon="IconAppointment" tone="info" variant="outline" />
        <StatTile label="Alertas activas" :value="3" :icon="IconAlert" tone="danger" variant="solid" />
        <StatTile label="Completadas" :value="87" :icon="IconSuccess" tone="success" variant="solid" />
      </div>
    </section>

    <section class="ui-kit__section">
      <h2 class="ui-kit__heading">Distintivos de estado</h2>
      <div class="row-wrap">
        <StatusBadge v-for="s in statuses" :key="s.v" :variant="s.v" :text="s.t" dot />
      </div>
    </section>

    <section class="ui-kit__section">
      <h2 class="ui-kit__heading">Avisos</h2>
      <div class="stack">
        <AppAlert variant="danger" title="No se pudo guardar" message="Revisa los campos marcados e inténtalo de nuevo." />
        <AppAlert variant="warning" message="Los signos vitales están fuera del rango habitual." />
        <AppAlert variant="success" message="La cita se registró correctamente." />
        <AppAlert variant="info" message="El análisis de IA tarda unos segundos en completarse." dismissible />
      </div>
    </section>

    <section class="ui-kit__section">
      <h2 class="ui-kit__heading">Tarjetas y formularios</h2>
      <div class="grid-2">
        <BaseCard title="Datos del paciente" subtitle="Información básica" :icon="IconUser">
          <FormSection title="Identificación" :icon="IconIdentification">
            <div class="row g-3">
              <div class="col-sm-6">
                <label class="form-label">Nombre</label>
                <input class="form-control" placeholder="Ana">
              </div>
              <div class="col-sm-6">
                <label class="form-label">Tipo</label>
                <select class="form-select"><option>Cédula</option></select>
              </div>
              <div class="col-12">
                <div class="form-check">
                  <input class="form-check-input" type="checkbox" id="k1" checked>
                  <label class="form-check-label" for="k1">Paciente activo</label>
                </div>
              </div>
            </div>
          </FormSection>
        </BaseCard>

        <div class="stack">
          <BaseCard title="Estados" :icon="IconClinical" hoverable>
            <LoadingState variant="skeleton" :rows="3" />
          </BaseCard>
          <BaseCard flush padding="none">
            <EmptyState
              size="sm"
              :icon="IconDocument"
              tone="neutral"
              title="Sin documentos"
              message="Este paciente todavía no tiene documentos cargados."
              action-label="Subir documento"
              :action-icon="IconUpload"
            />
          </BaseCard>
        </div>
      </div>
    </section>

    <section class="ui-kit__section">
      <h2 class="ui-kit__heading">Tabla</h2>
      <DataTable :columns="columns" :rows="rows" row-key="id">
        <template #cell-estado="{ value }">
          <StatusBadge :variant="value" :text="value" />
        </template>
        <template #actions>
          <AppButton variant="ghost" size="sm" :icon="IconView" aria-label="Ver" />
          <AppButton variant="ghost" size="sm" :icon="IconEdit" aria-label="Editar" />
          <AppButton variant="ghost" size="sm" :icon="IconDelete" aria-label="Eliminar" />
        </template>
      </DataTable>
    </section>

    <section class="ui-kit__section">
      <h2 class="ui-kit__heading">Superposiciones</h2>
      <div class="row-wrap">
        <AppButton variant="soft" @click="showModal = true">Abrir modal</AppButton>
        <AppButton variant="soft-danger" @click="showConfirm = true">Abrir confirmación</AppButton>
      </div>
      <BaseModal v-model="showModal" title="Modal de ejemplo">
        <p class="mb-0 text-app-muted">
          Cierra con Escape, con clic en el fondo o con el botón. El foco queda atrapado dentro
          y el scroll de la página se bloquea.
        </p>
        <template #footer>
          <AppButton variant="soft" @click="showModal = false">Cancelar</AppButton>
          <AppButton variant="primary" @click="showModal = false">Aceptar</AppButton>
        </template>
      </BaseModal>
      <ConfirmDialog
        :visible="showConfirm"
        danger
        title="Eliminar registro"
        message="Esta acción no se puede deshacer."
        confirm-text="Eliminar"
        @confirm="showConfirm = false"
        @cancel="showConfirm = false"
      />
    </section>

    <section class="ui-kit__section">
      <h2 class="ui-kit__heading">Iconos</h2>
      <div class="row-wrap">
        <IconTile v-for="(t, i) in tones" :key="t" :icon="tileIcons[i]" :tone="t" size="lg" />
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import {
  AppAlert,
  AppButton,
  BaseCard,
  BaseModal,
  ConfirmDialog,
  DataTable,
  EmptyState,
  FormSection,
  IconTile,
  LoadingState,
  PageHeader,
  StatTile,
  StatusBadge,
  ThemeToggle
} from '@/components/ui'
import {
  IconAdd,
  IconAi,
  IconAlert,
  IconAppointment,
  IconClinical,
  IconDelete,
  IconDocument,
  IconEdit,
  IconForward,
  IconIdentification,
  IconPatients,
  IconSave,
  IconSearch,
  IconSparkles,
  IconSuccess,
  IconUpload,
  IconUser,
  IconView
} from '@/lib/icons'

const showModal = ref(false)
const showConfirm = ref(false)

const swatches = [
  { name: 'Marca 500', token: '--c-brand-500' },
  { name: 'Marca 600', token: '--c-brand-600' },
  { name: 'Marca 700', token: '--c-brand-700' },
  { name: 'Canvas', token: '--app-canvas' },
  { name: 'Superficie', token: '--app-surface' },
  { name: 'Borde', token: '--app-border' },
  { name: 'Éxito', token: '--c-success-600' },
  { name: 'Aviso', token: '--c-warning-500' },
  { name: 'Peligro', token: '--c-danger-600' },
  { name: 'Info / IA', token: '--c-info-600' }
]

const variants = [
  'primary',
  'soft',
  'soft-primary',
  'outline',
  'outline-primary',
  'ghost',
  'danger',
  'soft-danger',
  'success',
  'warning'
]

const statuses = [
  { v: 'pending', t: 'Pendiente' },
  { v: 'inprogress', t: 'En curso' },
  { v: 'completed', t: 'Completada' },
  { v: 'cancelled', t: 'Cancelada' },
  { v: 'active', t: 'Activo' },
  { v: 'inactive', t: 'Inactivo' },
  { v: 'low', t: 'Leve' },
  { v: 'moderate', t: 'Moderado' },
  { v: 'severe', t: 'Severo' }
]

const tones = ['brand', 'success', 'warning', 'danger', 'info', 'neutral']
const tileIcons = [IconAi, IconSuccess, IconAlert, IconDelete, IconAppointment, IconDocument]

const columns = [
  { key: 'paciente', label: 'Paciente' },
  { key: 'fecha', label: 'Fecha' },
  { key: 'motivo', label: 'Motivo' },
  { key: 'estado', label: 'Estado', align: 'center' }
]

const rows = [
  { id: 1, paciente: 'Ana Ruiz', fecha: '12/08/2026', motivo: 'Control de tensión', estado: 'completed' },
  { id: 2, paciente: 'Luis Peña', fecha: '12/08/2026', motivo: 'Dolor torácico', estado: 'inprogress' },
  { id: 3, paciente: 'Marta Gil', fecha: '13/08/2026', motivo: 'Revisión anual', estado: 'pending' }
]
</script>

<style scoped>
.ui-kit__section {
  margin-bottom: 2.5rem;
}

.ui-kit__heading {
  font-size: 0.75rem;
  font-weight: 600;
  letter-spacing: 0.07em;
  text-transform: uppercase;
  color: var(--app-text-subtle);
  margin-bottom: 0.875rem;
  padding-bottom: 0.5rem;
  border-bottom: 1px solid var(--app-border);
}

.row-wrap {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 0.625rem;
  margin-bottom: 0.75rem;
}

.stack {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.grid-2,
.grid-4 {
  display: grid;
  gap: 1rem;
}
.grid-2 {
  grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
}
.grid-4 {
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
}

.swatches {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
  gap: 0.75rem;
}

.swatch {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.swatch__chip {
  height: 44px;
  border-radius: var(--app-radius);
  border: 1px solid var(--app-border);
}

.swatch__name {
  font-size: 0.8rem;
  font-weight: 500;
}

.swatch__token {
  font-size: 0.7rem;
  color: var(--app-text-subtle);
}
</style>
