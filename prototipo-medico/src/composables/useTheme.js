import { computed, ref, watch } from 'vue'

/**
 * Tema claro/oscuro.
 *
 * Singleton a nivel de módulo, mismo patrón que stores/auth.js (el proyecto no
 * usa Pinia). El estado guardado es la PREFERENCIA ('light' | 'dark' | 'system'),
 * no el tema resuelto: así, si el usuario deja 'system', la app sigue al sistema
 * operativo aunque cambie a mitad de sesión.
 *
 * El primer pintado lo resuelve un script bloqueante en index.html; initTheme()
 * sólo toma el relevo una vez arranca Vue.
 */

const STORAGE_KEY = 'sm-theme'
const VALID = ['light', 'dark', 'system']

const stored = typeof localStorage !== 'undefined' ? localStorage.getItem(STORAGE_KEY) : null

const preference = ref(VALID.includes(stored) ? stored : 'system')
const systemPrefersDark = ref(false)

const resolved = computed(() =>
  preference.value === 'system' ? (systemPrefersDark.value ? 'dark' : 'light') : preference.value
)

function applyTheme(theme) {
  document.documentElement.setAttribute('data-bs-theme', theme)
  document
    .querySelector('meta[name="theme-color"]')
    ?.setAttribute('content', theme === 'dark' ? '#0b1220' : '#f1f5f9')
}

/** Se llama una vez desde main.js, antes de montar la app. */
export function initTheme() {
  const mq = window.matchMedia('(prefers-color-scheme: dark)')
  systemPrefersDark.value = mq.matches
  mq.addEventListener('change', (e) => {
    systemPrefersDark.value = e.matches
  })

  watch(resolved, applyTheme, { immediate: true })
}

export function useTheme() {
  function setTheme(value) {
    if (!VALID.includes(value)) return
    preference.value = value
    localStorage.setItem(STORAGE_KEY, value)
  }

  return {
    preference,
    resolved,
    isDark: computed(() => resolved.value === 'dark'),
    setTheme,
    toggle: () => setTheme(resolved.value === 'dark' ? 'light' : 'dark')
  }
}
