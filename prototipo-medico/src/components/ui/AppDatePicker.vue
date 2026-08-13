<template>
  <div ref="root" class="app-date" :class="{ 'app-date--open': open }">
    <button
      :id="id"
      ref="trigger"
      type="button"
      class="app-date__trigger"
      aria-haspopup="dialog"
      :aria-expanded="open"
      :disabled="disabled"
      @click="toggle"
      @keydown.esc="close"
    >
      <span class="app-date__value" :class="{ 'app-date__value--empty': !selected }">
        {{ selected ? etiqueta : placeholder }}
      </span>
      <Icon :icon="IconAppointment" :size="16" class="app-date__icon" />
    </button>

    <Teleport to="body">
      <Transition name="app-date-pop">
        <div v-if="open" ref="panel" class="app-date__panel" role="dialog" aria-label="Seleccionar fecha" :style="popStyle">
        <div class="app-date__body">
          <!-- ---- Calendario ---- -->
          <div class="app-date__cal">
            <header class="app-date__nav">
              <button type="button" class="app-date__nav-btn" aria-label="Mes anterior" @click="cambiarMes(-1)">
                <Icon :icon="IconChevronLeft" :size="16" />
              </button>
              <span class="app-date__month">{{ etiquetaMes }}</span>
              <button type="button" class="app-date__nav-btn" aria-label="Mes siguiente" @click="cambiarMes(1)">
                <Icon :icon="IconChevronRight" :size="16" />
              </button>
            </header>

            <div class="app-date__grid app-date__grid--head" aria-hidden="true">
              <span v-for="d in DIAS" :key="d">{{ d }}</span>
            </div>

            <div class="app-date__grid">
              <button
                v-for="cell in celdas"
                :key="cell.key"
                type="button"
                class="app-date__day"
                :class="{
                  'app-date__day--out': !cell.mesActual,
                  'app-date__day--today': cell.hoy,
                  'app-date__day--sel': cell.seleccionado,
                  'app-date__day--off': cell.deshabilitado
                }"
                :disabled="cell.deshabilitado"
                :aria-current="cell.hoy ? 'date' : undefined"
                @click="elegirDia(cell.fecha)"
              >
                {{ cell.fecha.getDate() }}
              </button>
            </div>
          </div>

          <!-- ---- Hora ---- -->
          <div v-if="mode === 'datetime'" class="app-date__time">
            <p class="label-eyebrow mb-1">Hora</p>
            <div class="d-flex align-items-center gap-2">
              <AppSelect v-model="hora" :options="horas" placeholder="--" />
              <span class="app-date__sep">:</span>
              <AppSelect v-model="minuto" :options="minutos" placeholder="--" />
            </div>
          </div>
        </div>

          <footer class="app-date__footer">
            <AppButton variant="ghost" size="sm" @click="limpiar">Limpiar</AppButton>
            <AppButton variant="soft-primary" size="sm" @click="elegirHoy">Hoy</AppButton>
          </footer>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<script setup>
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import AppButton from './AppButton.vue'
import AppSelect from './AppSelect.vue'
import Icon from './Icon.vue'
import { IconAppointment, IconChevronLeft, IconChevronRight } from '@/lib/icons'
import { usePopover } from '@/composables/usePopover'

/**
 * Selector de fecha (y hora) propio.
 *
 * Sustituye a `<input type="date|datetime-local">`, cuyo desplegable es un
 * popup del NAVEGADOR: no admite ni un color ni un radio de la marca, sólo
 * claro/oscuro vía `color-scheme`.
 *
 * El `modelValue` conserva el MISMO formato que el input nativo
 * ('YYYY-MM-DD' o 'YYYY-MM-DDTHH:mm'), así que sustituirlo en una vista no
 * obliga a tocar el payload que se manda al backend.
 */
const props = defineProps({
  modelValue: { type: String, default: '' },
  mode: { type: String, default: 'date' }, // 'date' | 'datetime'
  placeholder: { type: String, default: 'Seleccionar fecha…' },
  id: { type: String, default: null },
  disabled: Boolean,
  /** 'YYYY-MM-DD'. Los días anteriores quedan deshabilitados. */
  min: { type: String, default: '' },
  minuteStep: { type: Number, default: 5 }
})

const emit = defineEmits(['update:modelValue'])

const DIAS = ['Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá', 'Do']
const pad = (n) => String(n).padStart(2, '0')

/**
 * OJO: `new Date('2026-08-12')` se interpreta como UTC y en husos negativos
 * cae en el día 11. Por eso la cadena se descompone a mano y se construye una
 * fecha LOCAL.
 */
function parseValue(v) {
  if (!v) return null
  const m = /^(\d{4})-(\d{2})-(\d{2})(?:[T ](\d{2}):(\d{2}))?/.exec(v)
  if (!m) return null
  return new Date(+m[1], +m[2] - 1, +m[3], +(m[4] ?? 0), +(m[5] ?? 0))
}

function toValue(d) {
  const base = `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`
  return props.mode === 'datetime' ? `${base}T${pad(d.getHours())}:${pad(d.getMinutes())}` : base
}

const sameDay = (a, b) =>
  a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate()

const root = ref(null)
const trigger = ref(null)
const panel = ref(null)
const open = ref(false)

// El panel se teletransporta a <body>: dentro de un FormSection (que lleva
// `overflow: hidden`) se recortaba por abajo, comiéndose la última semana y el
// pie con "Limpiar"/"Hoy".
const { style: popStyle } = usePopover(trigger, panel, open, { minWidth: 288 })

const selected = computed(() => parseValue(props.modelValue))
const cursor = ref(selected.value ?? new Date())

// Al reabrir, el calendario debe mostrar el mes del valor actual, no el último
// mes que el usuario estuviese hojeando la vez anterior.
watch(open, (o) => {
  if (o) cursor.value = selected.value ?? new Date()
})

const minDate = computed(() => parseValue(props.min))

const etiquetaMes = computed(() =>
  cursor.value.toLocaleDateString('es-ES', { month: 'long', year: 'numeric' })
)

const etiqueta = computed(() => {
  const d = selected.value
  if (!d) return ''
  const opts = { day: 'numeric', month: 'long', year: 'numeric' }
  if (props.mode === 'datetime') Object.assign(opts, { hour: '2-digit', minute: '2-digit' })
  return d.toLocaleDateString('es-ES', opts)
})

const celdas = computed(() => {
  const y = cursor.value.getFullYear()
  const m = cursor.value.getMonth()
  const hoy = new Date()
  // getDay() es 0=domingo; el calendario en es-ES empieza en lunes.
  const offset = (new Date(y, m, 1).getDay() + 6) % 7

  // 42 celdas (6 semanas) para que el panel no cambie de alto al pasar de mes.
  return Array.from({ length: 42 }, (_, i) => {
    const fecha = new Date(y, m, 1 - offset + i)
    return {
      key: fecha.toISOString(),
      fecha,
      mesActual: fecha.getMonth() === m,
      hoy: sameDay(fecha, hoy),
      seleccionado: selected.value ? sameDay(fecha, selected.value) : false,
      deshabilitado: minDate.value ? fecha < minDate.value && !sameDay(fecha, minDate.value) : false
    }
  })
})

const horas = Array.from({ length: 24 }, (_, h) => ({ value: pad(h), label: pad(h) }))
const minutos = computed(() =>
  Array.from({ length: Math.ceil(60 / props.minuteStep) }, (_, i) => {
    const v = pad(i * props.minuteStep)
    return { value: v, label: v }
  })
)

const hora = computed({
  get: () => (selected.value ? pad(selected.value.getHours()) : ''),
  set: (v) => aplicarHora(+v, selected.value?.getMinutes() ?? 0)
})

const minuto = computed({
  get: () => (selected.value ? pad(selected.value.getMinutes()) : ''),
  set: (v) => aplicarHora(selected.value?.getHours() ?? 0, +v)
})

function aplicarHora(h, mi) {
  const base = selected.value ?? new Date()
  emit('update:modelValue', toValue(new Date(base.getFullYear(), base.getMonth(), base.getDate(), h, mi)))
}

function elegirDia(fecha) {
  const prev = selected.value
  const d = new Date(
    fecha.getFullYear(),
    fecha.getMonth(),
    fecha.getDate(),
    prev?.getHours() ?? 0,
    prev?.getMinutes() ?? 0
  )
  emit('update:modelValue', toValue(d))
  // En modo fecha ya está todo elegido; en datetime el panel sigue abierto
  // para poder ajustar la hora sin reabrirlo.
  if (props.mode === 'date') close()
}

function elegirHoy() {
  const now = new Date()
  cursor.value = now
  elegirDia(now)
}

function limpiar() {
  emit('update:modelValue', '')
  close()
}

function cambiarMes(delta) {
  cursor.value = new Date(cursor.value.getFullYear(), cursor.value.getMonth() + delta, 1)
}

function close() {
  open.value = false
}

function toggle() {
  if (props.disabled) return
  open.value = !open.value
}

function onDocPointerDown(e) {
  if (!open.value) return
  if (root.value?.contains(e.target) || panel.value?.contains(e.target)) return
  // Los AppSelect de hora/minuto viven DENTRO de este panel, pero su lista
  // también se teletransporta a <body>, así que no cuenta como descendiente:
  // sin esta salvedad, elegir una hora cerraría el calendario entero.
  if (e.target.closest?.('.app-select__list')) return
  close()
}

function onKeydown(e) {
  if (e.key === 'Escape' && open.value) {
    close()
    trigger.value?.focus()
  }
}

onMounted(() => {
  document.addEventListener('pointerdown', onDocPointerDown)
  document.addEventListener('keydown', onKeydown)
})
onBeforeUnmount(() => {
  document.removeEventListener('pointerdown', onDocPointerDown)
  document.removeEventListener('keydown', onKeydown)
})
</script>

<style scoped>
.app-date {
  position: relative;
}

.app-date__trigger {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  width: 100%;
  padding: 0.5rem 0.75rem;
  background-color: var(--bs-body-bg);
  border: 1px solid var(--app-border-strong);
  border-radius: var(--app-radius);
  color: var(--app-text);
  font-size: 0.9375rem;
  text-align: start;
  cursor: pointer;
  transition: border-color var(--app-transition), box-shadow var(--app-transition);
}

.app-date__trigger:hover:not(:disabled) {
  border-color: var(--c-brand-500);
}

.app-date__trigger:focus-visible,
.app-date--open .app-date__trigger {
  outline: none;
  border-color: var(--c-brand-500);
  box-shadow: var(--app-ring);
}

.app-date__trigger:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.app-date__value {
  flex: 1;
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.app-date__value--empty {
  color: var(--app-text-subtle);
}

.app-date__icon {
  flex: none;
  color: var(--app-text-muted);
}

/* La posición (fixed + top/left) la inyecta usePopover como estilo en línea. */
.app-date__panel {
  z-index: 1060;
  min-width: 18rem;
  overflow-y: auto;
  background-color: var(--app-surface-raised);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius-lg);
  box-shadow: var(--app-shadow-lg);
}

.app-date__body {
  display: flex;
  gap: 0.75rem;
  padding: 0.75rem;
}

.app-date__cal {
  min-width: 0;
}

.app-date__nav {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.app-date__nav-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  border: 1px solid transparent;
  border-radius: var(--app-radius-sm);
  background: transparent;
  color: var(--app-text-muted);
  cursor: pointer;
}

.app-date__nav-btn:hover {
  background-color: var(--app-surface-sunken);
  color: var(--app-text);
}

.app-date__nav-btn:focus-visible {
  outline: none;
  box-shadow: var(--app-ring);
}

.app-date__month {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--app-text-strong);
  text-transform: capitalize;
}

.app-date__grid {
  display: grid;
  grid-template-columns: repeat(7, 2rem);
  gap: 0.125rem;
}

.app-date__grid--head {
  margin-bottom: 0.25rem;
}

.app-date__grid--head span {
  display: grid;
  place-items: center;
  height: 1.5rem;
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--app-text-muted);
}

.app-date__day {
  height: 2rem;
  border: 1px solid transparent;
  border-radius: var(--app-radius-sm);
  background: transparent;
  color: var(--app-text);
  font-size: 0.8125rem;
  font-variant-numeric: tabular-nums;
  cursor: pointer;
  transition: background-color var(--app-transition), color var(--app-transition);
}

.app-date__day:hover:not(:disabled) {
  background-color: var(--app-surface-sunken);
}

.app-date__day:focus-visible {
  outline: none;
  box-shadow: var(--app-ring);
}

/* Los días de relleno del mes anterior/siguiente siguen siendo pulsables (como
   en el nativo), sólo bajan de jerarquía. */
.app-date__day--out {
  color: var(--app-text-subtle);
}

.app-date__day--today {
  border-color: var(--c-brand-500);
  font-weight: 600;
}

.app-date__day--sel,
.app-date__day--sel:hover {
  background-color: var(--c-brand-600);
  border-color: var(--c-brand-600);
  color: #fff;
  font-weight: 600;
}

.app-date__day--off {
  opacity: 0.35;
  cursor: not-allowed;
}

.app-date__time {
  flex: none;
  width: 9.5rem;
  padding-inline-start: 0.75rem;
  border-inline-start: 1px solid var(--app-border);
}

.app-date__sep {
  color: var(--app-text-muted);
}

.app-date__footer {
  display: flex;
  justify-content: space-between;
  gap: 0.5rem;
  padding: 0.5rem 0.75rem;
  border-top: 1px solid var(--app-border);
}

.app-date-pop-enter-active,
.app-date-pop-leave-active {
  transition: opacity 0.14s ease, transform 0.14s ease;
}
.app-date-pop-enter-from,
.app-date-pop-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}

@media (max-width: 480px) {
  .app-date__body {
    flex-direction: column;
  }
  .app-date__time {
    width: auto;
    padding-inline-start: 0;
    border-inline-start: 0;
    border-top: 1px solid var(--app-border);
    padding-top: 0.75rem;
  }
}
</style>
