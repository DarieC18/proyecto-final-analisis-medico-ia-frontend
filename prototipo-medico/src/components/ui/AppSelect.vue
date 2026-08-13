<template>
  <div ref="root" class="app-select" :class="{ 'app-select--open': open }">
    <button
      :id="id"
      ref="trigger"
      type="button"
      class="app-select__trigger"
      role="combobox"
      aria-haspopup="listbox"
      :aria-expanded="open"
      :aria-controls="listId"
      :aria-activedescendant="open && activeIndex >= 0 ? `${listId}-${activeIndex}` : undefined"
      :disabled="disabled"
      @click="toggle"
      @keydown="onKeydown"
    >
      <span class="app-select__value" :class="{ 'app-select__value--empty': !selected }">
        {{ selected ? selected.label : placeholder }}
      </span>
      <Icon :icon="IconChevronDown" :size="16" class="app-select__chevron" />
    </button>

    <Teleport to="body">
      <Transition name="app-select-pop">
        <ul
          v-if="open"
          :id="listId"
          ref="list"
          class="app-select__list"
          role="listbox"
          :aria-label="placeholder"
          :style="popStyle"
        >
          <li
            v-for="(opt, i) in options"
            :id="`${listId}-${i}`"
            :key="opt.value"
            class="app-select__option"
            :class="{
              'app-select__option--active': i === activeIndex,
              'app-select__option--selected': opt.value === modelValue
            }"
            role="option"
            :aria-selected="opt.value === modelValue"
            @click="pick(opt)"
            @mousemove="activeIndex = i"
          >
            <span class="app-select__option-label">{{ opt.label }}</span>
            <Icon v-if="opt.value === modelValue" :icon="IconCheck" :size="15" />
          </li>

          <li v-if="!options.length" class="app-select__empty">Sin opciones</li>
        </ul>
      </Transition>
    </Teleport>
  </div>
</template>

<script setup>
import { computed, nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import Icon from './Icon.vue'
import { IconCheck, IconChevronDown } from '@/lib/icons'
import { usePopover } from '@/composables/usePopover'

/**
 * Select propio.
 *
 * El `<select>` nativo despliega su lista como UI DEL NAVEGADOR, pintada fuera
 * del documento: ni `border-radius`, ni la tipografía, ni el cián de marca la
 * alcanzan. `color-scheme` sólo decide si esa lista es clara u oscura — nada
 * más. Por eso aquí la lista es DOM real, con los tokens del tema.
 *
 * Sigue el patrón ARIA de combobox: el foco no se mueve a las opciones, se
 * queda en el trigger y la opción activa se anuncia con `aria-activedescendant`.
 *
 * `options`: [{ value, label }]. El llamante mapea sus datos.
 */
const props = defineProps({
  modelValue: { type: [String, Number, null], default: '' },
  options: { type: Array, default: () => [] },
  placeholder: { type: String, default: 'Seleccionar…' },
  id: { type: String, default: null },
  disabled: Boolean
})

const emit = defineEmits(['update:modelValue'])

const root = ref(null)
const trigger = ref(null)
const list = ref(null)
const open = ref(false)
const activeIndex = ref(-1)

const listId = `app-select-${Math.random().toString(36).slice(2, 9)}`

// La lista se teletransporta a <body>: si no, FormSection (que lleva
// `overflow: hidden`) la recortaría. `matchWidth` la mantiene alineada al ancho
// del campo, como haría el desplegable nativo.
const { style: popStyle, update: updatePop } = usePopover(trigger, list, open, { matchWidth: true })

const selected = computed(() => props.options.find((o) => o.value === props.modelValue) ?? null)

function openList() {
  if (props.disabled) return
  open.value = true
  const i = props.options.findIndex((o) => o.value === props.modelValue)
  activeIndex.value = i >= 0 ? i : 0
  scrollActiveIntoView()
}

function closeList() {
  open.value = false
  activeIndex.value = -1
}

function toggle() {
  open.value ? closeList() : openList()
}

function pick(opt) {
  emit('update:modelValue', opt.value)
  closeList()
  trigger.value?.focus()
}

async function scrollActiveIntoView() {
  await nextTick()
  list.value?.children[activeIndex.value]?.scrollIntoView({ block: 'nearest' })
}

function move(delta) {
  if (!props.options.length) return
  const n = props.options.length
  activeIndex.value = (activeIndex.value + delta + n) % n
  scrollActiveIntoView()
}

function onKeydown(e) {
  switch (e.key) {
    case 'ArrowDown':
      e.preventDefault()
      open.value ? move(1) : openList()
      break
    case 'ArrowUp':
      e.preventDefault()
      open.value ? move(-1) : openList()
      break
    case 'Home':
      if (open.value) {
        e.preventDefault()
        activeIndex.value = 0
        scrollActiveIntoView()
      }
      break
    case 'End':
      if (open.value) {
        e.preventDefault()
        activeIndex.value = props.options.length - 1
        scrollActiveIntoView()
      }
      break
    case 'Enter':
    case ' ':
      e.preventDefault()
      if (open.value && activeIndex.value >= 0) pick(props.options[activeIndex.value])
      else openList()
      break
    case 'Escape':
      if (open.value) {
        e.preventDefault()
        closeList()
      }
      break
    case 'Tab':
      // Tab confirma lo ya seleccionado y sale: cerrar sin robar el foco.
      if (open.value) closeList()
      break
  }
}

// La lista vive en <body> por el Teleport, así que ya NO está dentro de `root`:
// comprobar sólo `root` cerraría el desplegable al pulsar una opción.
function onDocPointerDown(e) {
  if (!open.value) return
  if (root.value?.contains(e.target) || list.value?.contains(e.target)) return
  closeList()
}

// Si la lista cambia bajo los pies (carga asíncrona), un índice viejo apuntaría
// a otra opción distinta de la resaltada.
watch(
  () => props.options,
  () => {
    if (!open.value) return
    activeIndex.value = Math.min(activeIndex.value, props.options.length - 1)
    // Cambia el número de opciones -> cambia el alto -> hay que recolocar (y
    // reevaluar si sigue cabiendo hacia abajo).
    updatePop()
  }
)

onMounted(() => document.addEventListener('pointerdown', onDocPointerDown))
onBeforeUnmount(() => document.removeEventListener('pointerdown', onDocPointerDown))
</script>

<style scoped>
.app-select {
  position: relative;
}

.app-select__trigger {
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

.app-select__trigger:hover:not(:disabled) {
  border-color: var(--c-brand-500);
}

.app-select__trigger:focus-visible,
.app-select--open .app-select__trigger {
  outline: none;
  border-color: var(--c-brand-500);
  box-shadow: var(--app-ring);
}

.app-select__trigger:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.app-select__value {
  flex: 1;
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.app-select__value--empty {
  color: var(--app-text-subtle);
}

.app-select__chevron {
  flex: none;
  color: var(--app-text-muted);
  transition: transform var(--app-transition);
}

.app-select--open .app-select__chevron {
  transform: rotate(180deg);
}

/* La posición (fixed + top/left/width) la inyecta usePopover como estilo en
   línea; aquí sólo queda el aspecto. z-index por encima del panel del
   calendario (1060), porque los select de hora viven dentro de él. */
.app-select__list {
  z-index: 1070;
  margin: 0;
  padding: 0.25rem;
  list-style: none;
  overflow-y: auto;
  background-color: var(--app-surface-raised);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius);
  box-shadow: var(--app-shadow-lg);
}

.app-select__option {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.4rem 0.6rem;
  border-radius: var(--app-radius-sm);
  font-size: 0.9rem;
  color: var(--app-text);
  cursor: pointer;
}

.app-select__option-label {
  flex: 1;
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* El resaltado sigue al teclado Y al ratón, por eso es `--active` y no :hover:
   con dos resaltados a la vez no se sabe cuál confirmaría Enter. */
.app-select__option--active {
  background-color: var(--bs-primary-bg-subtle);
  color: var(--bs-primary-text-emphasis);
}

.app-select__option--selected {
  font-weight: 600;
  color: var(--bs-primary-text-emphasis);
}

.app-select__empty {
  padding: 0.5rem 0.6rem;
  font-size: 0.85rem;
  color: var(--app-text-subtle);
}

.app-select-pop-enter-active,
.app-select-pop-leave-active {
  transition: opacity 0.14s ease, transform 0.14s ease;
}
.app-select-pop-enter-from,
.app-select-pop-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}
</style>
