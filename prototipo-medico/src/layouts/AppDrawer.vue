<template>
  <Teleport to="body">
    <Transition name="drawer-fade">
      <div v-if="isOpen" class="drawer-backdrop" @click="close" />
    </Transition>

    <Transition name="drawer-slide">
      <div
        v-if="isOpen"
        id="app-drawer"
        ref="panel"
        class="drawer"
        role="dialog"
        aria-modal="true"
        aria-label="Navegación"
      >
        <div class="drawer__header">
          <BrandMark :size="32" />
          <button type="button" class="btn btn-ghost btn-sm" aria-label="Cerrar navegación" @click="close">
            <Icon :icon="IconClose" :size="18" />
          </button>
        </div>

        <div class="drawer__scroll">
          <AppNavList aria-label="Navegación principal" />
        </div>

        <div class="drawer__footer">
          <ThemeToggle with-label />
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import AppNavList from './AppNavList.vue'
import BrandMark from '@/components/ui/BrandMark.vue'
import Icon from '@/components/ui/Icon.vue'
import ThemeToggle from '@/components/ui/ThemeToggle.vue'
import { IconClose } from '@/lib/icons'
import { useDrawer } from '@/composables/useDrawer'

/**
 * Drawer de navegación móvil construido a mano.
 * El JS de Bootstrap nunca se importa en este proyecto, así que ni el
 * `navbar-toggler` con data-bs-toggle="collapse" ni el offcanvas funcionaban:
 * por debajo de 992px la app se quedaba literalmente sin navegación.
 */
const { isOpen, close } = useDrawer()
const router = useRouter()
const panel = ref(null)

let lastFocused = null

const FOCUSABLE = 'a[href], button:not([disabled]), input, select, textarea, [tabindex]:not([tabindex="-1"])'

function onKeydown(e) {
  if (!isOpen.value) return

  if (e.key === 'Escape') {
    close()
    return
  }

  if (e.key !== 'Tab' || !panel.value) return

  // Trap simple: el foco no debe escaparse del panel mientras está abierto.
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

watch(isOpen, async (open) => {
  if (open) {
    lastFocused = document.activeElement
    // Compensar el ancho de la barra de scroll evita el salto lateral del layout.
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
})

// Sin esto, navegar desde el drawer lo deja abierto encima de la vista nueva.
// `afterEach` devuelve su propia función de baja: hay que llamarla al
// desmontar, o cada remontaje (logout -> login) apila un guard más.
let stopAfterEach = null

onMounted(() => {
  document.addEventListener('keydown', onKeydown)
  stopAfterEach = router.afterEach(() => close())
})

onBeforeUnmount(() => {
  document.removeEventListener('keydown', onKeydown)
  stopAfterEach?.()
  document.body.style.overflow = ''
  document.body.style.paddingRight = ''
})
</script>

<style scoped>
.drawer-backdrop {
  position: fixed;
  inset: 0;
  z-index: 1040;
  background-color: rgba(2, 6, 23, 0.55);
  backdrop-filter: blur(2px);
}

.drawer {
  position: fixed;
  inset-block: 0;
  inset-inline-start: 0;
  z-index: 1045;
  width: min(84vw, 300px);
  display: flex;
  flex-direction: column;
  background-color: var(--app-surface);
  border-right: 1px solid var(--app-border);
  box-shadow: var(--app-shadow-lg);
}

.drawer__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
  height: var(--app-topbar-h);
  padding-inline: 0.875rem;
  flex: none;
  border-bottom: 1px solid var(--app-border);
}

.drawer__scroll {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  padding: 1rem 0.75rem;
}

.drawer__footer {
  flex: none;
  padding: 0.75rem;
  border-top: 1px solid var(--app-border);
}

.drawer-fade-enter-active,
.drawer-fade-leave-active {
  transition: opacity 0.2s ease;
}
.drawer-fade-enter-from,
.drawer-fade-leave-to {
  opacity: 0;
}

.drawer-slide-enter-active,
.drawer-slide-leave-active {
  transition: transform 0.24s cubic-bezier(0.32, 0.72, 0, 1);
}
.drawer-slide-enter-from,
.drawer-slide-leave-to {
  transform: translateX(-100%);
}

@media (prefers-reduced-motion: reduce) {
  .drawer-slide-enter-active,
  .drawer-slide-leave-active {
    transition: none;
  }
}
</style>
