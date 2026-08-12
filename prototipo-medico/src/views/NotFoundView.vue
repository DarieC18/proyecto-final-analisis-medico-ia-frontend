<template>
  <div class="not-found">
    <IconTile :icon="IconAttention" tone="warning" size="xl" />
    <p class="not-found__code">404</p>
    <h1 class="not-found__title">Esta página no existe</h1>
    <p class="not-found__text">
      La dirección <code>{{ $route.fullPath }}</code> no corresponde a ninguna sección de
      MedAnalyzer.
    </p>
    <RouterLink :to="homeRoute" class="btn btn-primary">
      <Icon :icon="IconBack" :size="16" class="me-2" />
      Volver al inicio
    </RouterLink>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import Icon from '@/components/ui/Icon.vue'
import IconTile from '@/components/ui/IconTile.vue'
import { IconAttention, IconBack } from '@/lib/icons'
import { homeRouteForRoles } from '@/config/navigation'
import { authStore } from '@/stores/auth'

const homeRoute = computed(
  () => homeRouteForRoles(authStore.user?.roles?.filter(Boolean) ?? []) ?? '/'
)
</script>

<style scoped>
.not-found {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  min-height: 70dvh;
  text-align: center;
  padding: 2rem 1rem;
}

.not-found__code {
  margin: 1rem 0 0;
  font-family: var(--app-font-display);
  font-size: 3rem;
  font-weight: 700;
  line-height: 1;
  color: var(--app-text-subtle);
}

.not-found__title {
  margin: 0;
  font-size: 1.4rem;
}

.not-found__text {
  max-width: 44ch;
  color: var(--app-text-muted);
  margin-bottom: 1rem;
}

.not-found__text code {
  color: var(--app-text);
  background-color: var(--bs-tertiary-bg);
  padding: 0.1rem 0.35rem;
  border-radius: var(--app-radius-sm);
}
</style>
