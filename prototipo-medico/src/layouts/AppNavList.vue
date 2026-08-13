<template>
  <nav class="nav-list" :aria-label="ariaLabel">
    <div v-for="section in sections" :key="section.id" class="nav-list__section">
      <p class="nav-list__heading">{{ section.label }}</p>
      <ul class="nav-list__items">
        <li v-for="item in section.items" :key="item.to">
          <RouterLink
            :to="item.to"
            class="nav-list__link"
            :class="{ 'is-active': isActive(item) }"
            :aria-current="isActive(item) ? 'page' : undefined"
            active-class=""
            exact-active-class=""
          >
            <Icon :icon="item.icon" :size="18" />
            <span class="nav-list__label">{{ item.label }}</span>
          </RouterLink>
        </li>
      </ul>
    </div>
  </nav>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import Icon from '@/components/ui/Icon.vue'
import { navForRoles } from '@/config/navigation'
import { authStore } from '@/stores/auth'

defineProps({
  ariaLabel: { type: String, default: 'Navegación principal' }
})

const route = useRoute()

const sections = computed(() => navForRoles(authStore.user?.roles?.filter(Boolean) ?? []))

/**
 * No se usa `router-link-active`: coincide por prefijo y da falsos positivos
 * (con /portal/citas activo, /portal también se encendería). Se compara contra
 * `routeNames`, para que /citas/:id ilumine "Citas".
 */
function isActive(item) {
  if (item.routeNames?.length) return item.routeNames.includes(route.name)
  return route.path === item.to
}
</script>

<style scoped>
.nav-list {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.nav-list__heading {
  margin: 0 0 0.375rem;
  padding-inline: 0.75rem;
  font-size: 0.68rem;
  font-weight: 600;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: var(--app-text-subtle);
}

.nav-list__items {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
}

.nav-list__link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem 0.75rem;
  border-radius: var(--app-radius);
  color: var(--app-text-muted);
  font-size: 0.875rem;
  font-weight: 500;
  transition: background-color var(--app-transition), color var(--app-transition);
}

.nav-list__link:hover {
  background-color: var(--bs-tertiary-bg);
  color: var(--app-text-strong);
}

.nav-list__link.is-active {
  background-color: var(--bs-primary-bg-subtle);
  color: var(--bs-primary-text-emphasis);
  font-weight: 600;
}

.nav-list__label {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
