<template>
  <component
    :is="tag"
    :to="to"
    :href="href"
    :type="tag === 'button' ? type : undefined"
    :disabled="tag === 'button' ? disabled || loading : undefined"
    :aria-busy="loading || undefined"
    class="btn d-inline-flex align-items-center justify-content-center gap-2"
    :class="classes"
  >
    <span v-if="loading" class="spinner-border spinner-border-sm" aria-hidden="true" />
    <Icon v-else-if="icon" :icon="icon" :size="iconSize" />
    <slot />
    <Icon v-if="iconRight && !loading" :icon="iconRight" :size="iconSize" />
  </component>
</template>

<script setup>
import { computed } from 'vue'
import { RouterLink } from 'vue-router'
import Icon from './Icon.vue'

/**
 * Botón único de la app.
 *
 * Antes cada llamada retecleaba su combinación de clases: `rounded-pill px-5
 * py-2 shadow` en un sitio, `px-4 shadow-sm` en otro, y el spinner
 * `<span class="spinner-border spinner-border-sm me-2">` copiado a mano en la
 * mayoría de los ~40 puntos de carga. Aquí eso es una prop.
 */
const props = defineProps({
  variant: { type: String, default: 'primary' },
  size: { type: String, default: null },
  loading: Boolean,
  disabled: Boolean,
  icon: { type: [Object, Function], default: null },
  iconRight: { type: [Object, Function], default: null },
  block: Boolean,
  pill: Boolean,
  to: { type: [String, Object], default: null },
  href: { type: String, default: null },
  type: { type: String, default: 'button' }
})

const VARIANTS = {
  primary: 'btn-primary',
  soft: 'btn-soft',
  'soft-primary': 'btn-soft-primary',
  outline: 'btn-outline-secondary',
  'outline-primary': 'btn-outline-primary',
  ghost: 'btn-ghost',
  danger: 'btn-danger',
  'soft-danger': 'btn-soft-danger',
  'outline-danger': 'btn-outline-danger',
  success: 'btn-success',
  warning: 'btn-warning',
  light: 'btn-light',
  'outline-light': 'btn-outline-light',
  // Para usar sobre paneles de color de marca (gradientes), no sobre --app-surface.
  'on-brand': 'btn-on-brand',
  'on-brand-outline': 'btn-on-brand-outline'
}

// RouterLink cuando hay `to`, <a> cuando hay `href`, <button> en el resto.
const tag = computed(() => (props.to ? RouterLink : props.href ? 'a' : 'button'))

const iconSize = computed(() => (props.size === 'sm' ? 15 : props.size === 'lg' ? 19 : 17))

const classes = computed(() => [
  VARIANTS[props.variant] ?? `btn-${props.variant}`,
  props.size ? `btn-${props.size}` : null,
  props.block ? 'w-100' : null,
  props.pill ? 'rounded-pill' : null,
  // Un <a>/RouterLink deshabilitado no responde al atributo `disabled`.
  tag.value !== 'button' && (props.disabled || props.loading) ? 'disabled pe-none' : null
])
</script>
