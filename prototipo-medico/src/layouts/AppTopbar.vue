<template>
  <header class="app-topbar">
    <button
      type="button"
      class="btn btn-ghost btn-sm d-lg-none"
      aria-label="Abrir navegación"
      aria-controls="app-drawer"
      :aria-expanded="isOpen"
      @click="open"
    >
      <Icon :icon="IconMenu" :size="20" />
    </button>

    <RouterLink :to="homeRoute" class="app-topbar__brand d-lg-none">
      <BrandMark :size="30" compact />
    </RouterLink>

    <p class="app-topbar__title d-none d-lg-block">{{ title }}</p>

    <div class="app-topbar__actions">
      <ThemeToggle class="d-lg-none" />
      <UserMenu />
    </div>
  </header>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import BrandMark from '@/components/ui/BrandMark.vue'
import Icon from '@/components/ui/Icon.vue'
import ThemeToggle from '@/components/ui/ThemeToggle.vue'
import UserMenu from '@/components/UserMenu.vue'
import { IconMenu } from '@/lib/icons'
import { homeRouteForRoles, navForRoles } from '@/config/navigation'
import { authStore } from '@/stores/auth'
import { useDrawer } from '@/composables/useDrawer'

const route = useRoute()
const { isOpen, open } = useDrawer()

const roles = computed(() => authStore.user?.roles?.filter(Boolean) ?? [])
const homeRoute = computed(() => homeRouteForRoles(roles.value) ?? '/login')

// El título sale de la propia definición de navegación, así no hay una segunda
// lista de nombres de pantalla que mantener en sincronía.
const title = computed(() => {
  for (const section of navForRoles(roles.value)) {
    for (const item of section.items) {
      if (item.routeNames?.includes(route.name)) return item.label
    }
  }
  return route.meta?.title ?? ''
})
</script>

<style scoped>
/* Sticky DENTRO de la columna de contenido: el flujo normal ya reserva su
   altura, así que no hace falta compensar nada con padding. */
.app-topbar {
  position: sticky;
  top: 0;
  z-index: 1020;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  height: var(--app-topbar-h);
  padding-inline: 0.875rem;
  background-color: color-mix(in srgb, var(--app-surface) 86%, transparent);
  backdrop-filter: saturate(180%) blur(8px);
  border-bottom: 1px solid var(--app-border);
}

@supports not (backdrop-filter: blur(8px)) {
  .app-topbar {
    background-color: var(--app-surface);
  }
}

.app-topbar__brand {
  display: flex;
  align-items: center;
}

.app-topbar__title {
  margin: 0;
  font-family: var(--app-font-display);
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--app-text-strong);
  padding-inline-start: 0.375rem;
}

.app-topbar__actions {
  margin-inline-start: auto;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}
</style>
