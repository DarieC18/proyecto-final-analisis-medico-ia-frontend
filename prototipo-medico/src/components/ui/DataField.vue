<template>
  <div class="data-field">
    <p class="data-field__label">
      <Icon v-if="icon" :icon="icon" :size="13" tone="subtle" />
      {{ label }}
    </p>
    <p class="data-field__value" :class="{ 'data-field__value--empty': isEmpty }">
      <slot>{{ isEmpty ? '—' : value }}</slot>
    </p>
  </div>
</template>

<script setup>
import { computed, useSlots } from 'vue'
import Icon from './Icon.vue'

/** Par etiqueta/valor de las fichas de detalle. Sustituye al
    `label.text-muted.small.fw-bold.text-uppercase + p.fw-medium` repetido. */
const props = defineProps({
  label: { type: String, required: true },
  value: { type: [String, Number], default: null },
  icon: { type: [Object, Function], default: null }
})

const slots = useSlots()
const isEmpty = computed(
  () => !slots.default && (props.value === null || props.value === undefined || props.value === '')
)
</script>

<style scoped>
.data-field__label {
  display: flex;
  align-items: center;
  gap: 0.3rem;
  margin: 0 0 0.15rem;
  font-size: 0.7rem;
  font-weight: 600;
  letter-spacing: 0.05em;
  text-transform: uppercase;
  color: var(--app-text-muted);
}

.data-field__value {
  margin: 0;
  font-size: 0.9375rem;
  font-weight: 500;
  color: var(--app-text-strong);
}

.data-field__value--empty {
  color: var(--app-text-subtle);
  font-weight: 400;
}
</style>
