<template>
  <PublicLayout v-if="isPublicRoute">
    <RouterView />
  </PublicLayout>

  <AppShell v-else>
    <RouterView />
  </AppShell>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import AppShell from '@/layouts/AppShell.vue'
import PublicLayout from '@/layouts/PublicLayout.vue'

const route = useRoute()
const isPublicRoute = computed(() => route.meta.public)
</script>

<!-- Sin <style> global: toda la capa de diseño vive en src/assets/styles/,
     importada desde main.js. -->
