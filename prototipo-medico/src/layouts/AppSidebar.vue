<template>
  <aside class="app-sidebar d-none d-lg-flex">
    <RouterLink :to="homeRoute" class="app-sidebar__brand">
      <BrandMark :size="34" />
    </RouterLink>

    <div class="app-sidebar__scroll">
      <AppNavList aria-label="Navegación principal" />
    </div>

    <div class="app-sidebar__footer">
      <ThemeToggle with-label />
    </div>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import AppNavList from './AppNavList.vue'
import BrandMark from '@/components/ui/BrandMark.vue'
import ThemeToggle from '@/components/ui/ThemeToggle.vue'
import { homeRouteForRoles } from '@/config/navigation'
import { authStore } from '@/stores/auth'

const homeRoute = computed(
  () => homeRouteForRoles(authStore.user?.roles?.filter(Boolean) ?? []) ?? '/login'
)
</script>

<style scoped>
/* Sticky dentro del grid, con altura de viewport propia. Nada de `fixed` +
   padding-top compensatorio: así desaparece el número mágico de 76px que estaba
   copiado en dos archivos como altura del navbar. */
.app-sidebar {
  position: sticky;
  top: 0;
  align-self: start;
  height: 100dvh;
  flex-direction: column;
  background-color: var(--app-surface);
  border-right: 1px solid var(--app-border);
}

.app-sidebar__brand {
  display: flex;
  align-items: center;
  height: var(--app-topbar-h);
  padding-inline: 1rem;
  flex: none;
  border-bottom: 1px solid var(--app-border);
}

.app-sidebar__scroll {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  padding: 1rem 0.75rem;
}

.app-sidebar__footer {
  flex: none;
  padding: 0.75rem;
  border-top: 1px solid var(--app-border);
}
</style>
