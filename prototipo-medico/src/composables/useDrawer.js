import { ref } from 'vue'

/**
 * Estado del drawer de navegación móvil.
 * Singleton de módulo: la topbar lo abre y el propio drawer lo cierra, sin
 * pasar props ni emitir eventos por media docena de componentes.
 */
const isOpen = ref(false)

export function useDrawer() {
  return {
    isOpen,
    open: () => {
      isOpen.value = true
    },
    close: () => {
      isOpen.value = false
    },
    toggle: () => {
      isOpen.value = !isOpen.value
    }
  }
}
