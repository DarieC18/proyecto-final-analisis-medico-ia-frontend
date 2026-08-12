<template>
  <button
    type="button"
    class="btn btn-ghost btn-sm theme-toggle"
    :aria-label="label"
    :title="label"
    @click="cycle"
  >
    <Icon :icon="currentIcon" :size="18" />
    <span v-if="withLabel" class="ms-2">{{ shortLabel }}</span>
  </button>
</template>

<script setup>
import { computed } from 'vue'
import Icon from './Icon.vue'
import { IconDark, IconLight, IconSystem } from '@/lib/icons'
import { useTheme } from '@/composables/useTheme'

defineProps({ withLabel: Boolean })

const { preference, setTheme } = useTheme()

// Ciclo de tres estados: claro -> oscuro -> sistema. Se expone `system` a
// propósito: es la única forma de que el usuario devuelva el control al SO
// una vez ha elegido manualmente.
const ORDER = ['light', 'dark', 'system']
const META = {
  light: { icon: IconLight, label: 'Tema claro', short: 'Claro' },
  dark: { icon: IconDark, label: 'Tema oscuro', short: 'Oscuro' },
  system: { icon: IconSystem, label: 'Tema del sistema', short: 'Sistema' }
}

const currentIcon = computed(() => META[preference.value].icon)
const shortLabel = computed(() => META[preference.value].short)
const label = computed(() => {
  const next = ORDER[(ORDER.indexOf(preference.value) + 1) % ORDER.length]
  return `${META[preference.value].label}. Cambiar a ${META[next].label.toLowerCase()}`
})

function cycle() {
  setTheme(ORDER[(ORDER.indexOf(preference.value) + 1) % ORDER.length])
}
</script>

<style scoped>
.theme-toggle {
  display: inline-flex;
  align-items: center;
  padding-inline: 0.5rem;
}
</style>
