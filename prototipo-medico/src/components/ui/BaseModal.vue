<template>
  <Teleport to="body">
    <Transition name="modal-fade">
      <div v-if="modelValue" class="modal-backdrop-app" @click="onBackdrop" />
    </Transition>

    <Transition name="modal-pop">
      <div
        v-if="modelValue"
        ref="panel"
        class="modal-app"
        :class="`modal-app--${size}`"
        role="dialog"
        aria-modal="true"
        :aria-label="title || 'Diálogo'"
      >
        <header v-if="title || $slots.header" class="modal-app__header">
          <slot name="header">
            <h2 class="modal-app__title">{{ title }}</h2>
          </slot>
          <button type="button" class="btn btn-ghost btn-sm" aria-label="Cerrar" @click="close">
            <Icon :icon="IconClose" :size="18" />
          </button>
        </header>

        <div class="modal-app__body" :class="{ 'modal-app__body--scrollable': scrollable }">
          <slot />
        </div>

        <footer v-if="$slots.footer" class="modal-app__footer">
          <slot name="footer" />
        </footer>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import Icon from './Icon.vue'
import { IconClose } from '@/lib/icons'

/**
 * Modal propio. La app tenía dos modales hechos a mano con
 * `class="modal d-block"` más un `.modal-backdrop` suelto, sin Teleport, sin
 * Escape y sin bloqueo de scroll — porque el JS de Bootstrap nunca se importa.
 */
const props = defineProps({
  modelValue: Boolean,
  title: { type: String, default: '' },
  size: { type: String, default: 'md' },
  scrollable: { type: Boolean, default: true },
  closeOnBackdrop: { type: Boolean, default: true },
  persistent: Boolean
})

const emit = defineEmits(['update:modelValue', 'close'])

const panel = ref(null)
let lastFocused = null

const FOCUSABLE =
  'a[href], button:not([disabled]), input:not([disabled]), select, textarea, [tabindex]:not([tabindex="-1"])'

function close() {
  if (props.persistent) return
  emit('update:modelValue', false)
  emit('close')
}

function onBackdrop() {
  if (props.closeOnBackdrop) close()
}

function onKeydown(e) {
  if (!props.modelValue) return

  if (e.key === 'Escape') {
    close()
    return
  }

  if (e.key !== 'Tab' || !panel.value) return

  const items = [...panel.value.querySelectorAll(FOCUSABLE)]
  if (!items.length) return
  const first = items[0]
  const last = items[items.length - 1]

  if (e.shiftKey && document.activeElement === first) {
    e.preventDefault()
    last.focus()
  } else if (!e.shiftKey && document.activeElement === last) {
    e.preventDefault()
    first.focus()
  }
}

watch(
  () => props.modelValue,
  async (open) => {
    if (open) {
      lastFocused = document.activeElement
      const gap = window.innerWidth - document.documentElement.clientWidth
      document.body.style.overflow = 'hidden'
      if (gap > 0) document.body.style.paddingRight = `${gap}px`
      await nextTick()
      panel.value?.querySelector(FOCUSABLE)?.focus()
    } else {
      document.body.style.overflow = ''
      document.body.style.paddingRight = ''
      lastFocused?.focus?.()
      lastFocused = null
    }
  }
)

onMounted(() => document.addEventListener('keydown', onKeydown))
onBeforeUnmount(() => {
  document.removeEventListener('keydown', onKeydown)
  document.body.style.overflow = ''
  document.body.style.paddingRight = ''
})
</script>

<style scoped>
.modal-backdrop-app {
  position: fixed;
  inset: 0;
  z-index: 1050;
  background-color: rgba(2, 6, 23, 0.55);
  backdrop-filter: blur(2px);
}

.modal-app {
  position: fixed;
  z-index: 1055;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: calc(100vw - 2rem);
  max-height: calc(100dvh - 3rem);
  display: flex;
  flex-direction: column;
  background-color: var(--app-surface-raised);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius-xl);
  box-shadow: var(--app-shadow-lg);
}

.modal-app--sm {
  max-width: 420px;
}
.modal-app--md {
  max-width: 560px;
}
.modal-app--lg {
  max-width: 760px;
}
.modal-app--xl {
  max-width: 1100px;
}

.modal-app__header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem 1.25rem;
  border-bottom: 1px solid var(--app-border);
  flex: none;
}

.modal-app__title {
  margin: 0;
  font-size: 1rem;
  font-weight: 600;
  flex: 1;
  min-width: 0;
}

.modal-app__body {
  padding: 1.25rem;
  min-height: 0;
}
.modal-app__body--scrollable {
  overflow-y: auto;
}

.modal-app__footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
  padding: 0.875rem 1.25rem;
  border-top: 1px solid var(--app-border);
  flex: none;
}

.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.18s ease;
}
.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

.modal-pop-enter-active,
.modal-pop-leave-active {
  transition: opacity 0.18s ease, transform 0.18s ease;
}
.modal-pop-enter-from,
.modal-pop-leave-to {
  opacity: 0;
  transform: translate(-50%, -48%) scale(0.97);
}
</style>
