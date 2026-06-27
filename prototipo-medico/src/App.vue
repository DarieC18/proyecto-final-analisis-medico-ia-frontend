<template>
  <Navbar v-if="!isPublicRoute" />
  <main :class="{ 'py-5': !isPublicRoute, 'py-0': isPublicRoute }">
    <RouterView />
  </main>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import Navbar from '@/components/Navbar.vue'

const route = useRoute()
const isPublicRoute = computed(() => route.meta.public)
</script>

<style>
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap');

body {
  background-color: #f0f4f8;
  font-family: 'Poppins', sans-serif;
  color: #2b3a4a;
}

:root {
  --bs-primary: #0ea5e9;
  --bs-success: #10b981;
  --bs-info: #3b82f6;
}

.card {
  border-radius: 1rem !important;
  border: none !important;
  transition: transform 0.2s ease-in-out;
}
.btn {
  border-radius: 0.5rem;
  font-weight: 500;
  letter-spacing: 0.3px;
}
</style>
