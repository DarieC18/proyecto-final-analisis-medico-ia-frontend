<template>
  <span class="brand" :class="{ 'brand--compact': compact }">
    <span class="brand__mark" :style="{ width: `${size}px`, height: `${size}px` }">
      <!-- Cruz médica con trazo de pulso. Sustituye al emoji ⚕️ que estaba
           repetido como marca en cinco archivos distintos. -->
      <svg viewBox="0 0 32 32" fill="none" aria-hidden="true">
        <path
          d="M13 5h6v8h8v6h-8v8h-6v-8H5v-6h8V5z"
          fill="currentColor"
          fill-opacity="0.16"
        />
        <path
          d="M4 16h5l2.5-5 4 10 3-6 2 3h7.5"
          stroke="currentColor"
          stroke-width="2.2"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
      </svg>
    </span>
    <span v-if="!compact" class="brand__name">
      Med<span class="brand__name-accent">Analyzer</span>
    </span>
  </span>
</template>

<script setup>
defineProps({
  size: { type: Number, default: 32 },
  compact: Boolean
})
</script>

<style scoped>
.brand {
  display: inline-flex;
  align-items: center;
  gap: 0.625rem;
  color: inherit;
}

.brand__mark {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex: none;
  border-radius: var(--app-radius);
  background: linear-gradient(135deg, var(--c-brand-500), var(--c-brand-700));
  color: #fff;
  box-shadow: var(--app-shadow-sm);
}

.brand__mark svg {
  width: 68%;
  height: 68%;
}

.brand__name {
  font-family: var(--app-font-display);
  font-weight: 600;
  font-size: 1.0625rem;
  letter-spacing: -0.015em;
  color: var(--app-text-strong);
  white-space: nowrap;
}

.brand__name-accent {
  color: var(--bs-primary-text-emphasis);
}
</style>
