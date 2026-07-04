<template>
  <span class="badge rounded-pill px-3 py-2 fw-bold" :class="badgeClass">{{ text }}</span>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  text: String,
  variant: { type: String, default: 'secondary' }
})

const badgeClass = computed(() => {
  const map = {
    pending: 'bg-warning bg-opacity-10 text-warning',
    inprogress: 'bg-primary bg-opacity-10 text-primary',
    completed: 'bg-success bg-opacity-10 text-success',
    cancelled: 'bg-danger bg-opacity-10 text-danger',
    active: 'bg-success bg-opacity-10 text-success',
    inactive: 'bg-secondary bg-opacity-10 text-secondary',
    info: 'bg-info bg-opacity-10 text-info',
    moderate: 'bg-warning bg-opacity-10 text-warning',
    severe: 'bg-danger bg-opacity-10 text-danger',
    low: 'bg-success bg-opacity-10 text-success'
  }
  return map[props.variant] || `bg-${props.variant} bg-opacity-10 text-${props.variant}`
})
</script>
