<template>
  <div class="public-layout">
    <!-- El toggle también aquí: si no, login y registro se salen del tema.
         La Landing trae su propio header con su propio toggle (`bareLayout`),
         así que no se le superpone este botón flotante. -->
    <div v-if="!route.meta.bareLayout" class="public-layout__theme">
      <ThemeToggle />
    </div>
    <slot />
  </div>
</template>

<script setup>
import { useRoute } from 'vue-router'
import ThemeToggle from '@/components/ui/ThemeToggle.vue'

const route = useRoute()
</script>

<style scoped>
.public-layout {
  position: relative;
  min-height: 100dvh;
  background-color: var(--app-canvas);
}

.public-layout__theme {
  position: absolute;
  top: 1rem;
  right: 1rem;
  z-index: 10;
}
</style>
