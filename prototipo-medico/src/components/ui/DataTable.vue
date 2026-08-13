<template>
  <BaseCard flush padding="none">
    <div v-if="$slots.toolbar" class="data-table__toolbar">
      <slot name="toolbar" />
    </div>

    <AppAlert v-if="error" variant="danger" class="m-3" :message="error" />

    <LoadingState v-else-if="loading" :label="loadingLabel" />

    <div v-else-if="!rows.length" class="data-table__empty">
      <slot name="empty">
        <EmptyState
          size="sm"
          :icon="emptyIcon"
          :title="emptyTitle"
          :message="emptyMessage"
        />
      </slot>
    </div>

    <div v-else class="table-responsive">
      <table class="table table-hover align-middle mb-0" :class="{ 'table-sm': dense }">
        <thead class="table-head">
          <tr>
            <th
              v-for="col in columns"
              :key="col.key"
              scope="col"
              :class="[alignClass(col), col.thClass]"
              :style="col.width ? { width: col.width } : null"
            >
              {{ col.label }}
            </th>
            <th v-if="$slots.actions" scope="col" class="text-end pe-4">Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(row, index) in rows" :key="keyFor(row, index)">
            <td
              v-for="col in columns"
              :key="col.key"
              :class="[alignClass(col), col.tdClass]"
            >
              <slot :name="`cell-${col.key}`" :row="row" :value="row[col.key]" :index="index">
                {{ format(row[col.key]) }}
              </slot>
            </td>
            <td v-if="$slots.actions" class="text-end pe-4">
              <div class="data-table__actions">
                <slot name="actions" :row="row" :index="index" />
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="$slots.footer" class="data-table__footer">
      <slot name="footer" />
    </div>
  </BaseCard>
</template>

<script setup>
import AppAlert from './AppAlert.vue'
import BaseCard from './BaseCard.vue'
import EmptyState from './EmptyState.vue'
import LoadingState from './LoadingState.vue'

/**
 * Tabla de datos. Absorbe las 11 tablas de la app, que repetían la misma receta
 * (`table table-hover align-middle mb-0` dentro de `.card > .table-responsive`,
 * con `<thead class="bg-light text-muted">`) y además reimplementaban a mano
 * sus estados de carga, error y vacío alrededor de cada una.
 *
 * Nota: la cabecera usa la clase propia `.table-head` y NO `.table-light`,
 * porque las variantes contextuales de tabla de Bootstrap llevan el hex
 * compilado y no reaccionan a data-bs-theme.
 */
const props = defineProps({
  columns: { type: Array, required: true },
  rows: { type: Array, default: () => [] },
  rowKey: { type: [String, Function], default: 'id' },
  loading: Boolean,
  error: { type: String, default: '' },
  loadingLabel: { type: String, default: 'Cargando datos…' },
  emptyTitle: { type: String, default: 'Sin resultados' },
  emptyMessage: { type: String, default: '' },
  emptyIcon: { type: [Object, Function], default: null },
  dense: Boolean
})

const alignClass = (col) =>
  col.align === 'end' ? 'text-end' : col.align === 'center' ? 'text-center' : null

const keyFor = (row, index) =>
  typeof props.rowKey === 'function' ? props.rowKey(row) : row?.[props.rowKey] ?? index

const format = (v) => (v === null || v === undefined || v === '' ? '—' : v)
</script>

<style scoped>
.data-table__toolbar {
  padding: 0.875rem 1rem;
  border-bottom: 1px solid var(--app-border);
}

.data-table__footer {
  padding: 0.75rem 1rem;
  border-top: 1px solid var(--app-border);
}

.data-table__actions {
  display: inline-flex;
  gap: 0.375rem;
  justify-content: flex-end;
}

.table :deep(th),
.table :deep(td) {
  padding-block: 0.75rem;
}
.table :deep(th:first-child),
.table :deep(td:first-child) {
  padding-inline-start: 1.25rem;
}
</style>
