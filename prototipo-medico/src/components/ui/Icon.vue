<template>
  <component
    :is="icon"
    :size="size"
    :stroke-width="strokeWidth"
    :class="toneClass"
    class="app-icon"
    aria-hidden="true"
  />
</template>

<script setup>
import { computed } from 'vue'

/**
 * Envoltorio mínimo sobre lucide.
 *
 * Recibe el COMPONENTE, no un string. Un `:name="'file-text'"` obligaría a un
 * mapa estático gigante o a importar el paquete entero, y mataría el
 * tree-shaking.
 *
 * Aporta dos cosas: un stroke-width por defecto de 1.75 (lucide usa 2, que a
 * 16-18px se ve pesado junto a tipografía de UI) y el `aria-hidden` en un solo
 * sitio. Para iconos con significado, el `aria-label` va en el botón que los
 * contiene, no en el SVG.
 */
const props = defineProps({
  icon: { type: [Object, Function], required: true },
  size: { type: [Number, String], default: 18 },
  strokeWidth: { type: [Number, String], default: 1.75 },
  tone: { type: String, default: null }
})

const TONES = {
  brand: 'text-brand',
  muted: 'text-app-muted',
  subtle: 'text-app-subtle',
  strong: 'text-app-strong',
  success: 'text-success-emphasis',
  warning: 'text-warning-emphasis',
  danger: 'text-danger-emphasis',
  info: 'text-info-emphasis'
}

const toneClass = computed(() => (props.tone ? TONES[props.tone] ?? null : null))
</script>

<style scoped>
.app-icon {
  flex: none;
  vertical-align: -0.125em;
}
</style>
