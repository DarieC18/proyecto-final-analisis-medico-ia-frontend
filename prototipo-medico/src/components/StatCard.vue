<template>
  <div class="card shadow-sm h-100" :class="borderClass">
    <div class="card-body">
      <h6 class="text-muted fw-bold text-uppercase mb-2">{{ label }}</h6>
      <h2 class="fw-bold mb-0" :class="valueClass">{{ value }}</h2>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  label: String,
  value: [String, Number],
  color: { type: String, default: 'info' }
})

const borderClass = computed(() => `border-start border-4 border-${props.color}`)
const valueClass = computed(() => props.color === 'danger' ? 'text-danger' : 'text-dark')
</script>
