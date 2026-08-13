<template>
  <div ref="root" class="user-menu">
    <button
      type="button"
      class="user-menu__trigger"
      :aria-expanded="open"
      aria-haspopup="menu"
      @click="open = !open"
    >
      <AvatarInitials :name="user?.name" :last-name="user?.lastName" size="sm" />
      <span class="user-menu__text d-none d-md-flex">
        <span class="user-menu__name">{{ fullName }}</span>
        <span class="user-menu__role">{{ roleLabel }}</span>
      </span>
      <Icon :icon="IconChevronDown" :size="16" class="text-app-subtle" />
    </button>

    <Transition name="user-menu-pop">
      <div v-if="open" class="user-menu__panel" role="menu">
        <div class="user-menu__header">
          <p class="user-menu__header-name">{{ fullName }}</p>
          <p class="user-menu__header-email">{{ user?.email }}</p>
        </div>
        <RouterLink :to="profileRoute" class="user-menu__item" role="menuitem" @click="open = false">
          <Icon :icon="IconUser" :size="16" />
          Mi perfil
        </RouterLink>
        <button type="button" class="user-menu__item user-menu__item--danger" role="menuitem" @click="logout">
          <Icon :icon="IconLogout" :size="16" />
          Cerrar sesión
        </button>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import AvatarInitials from '@/components/ui/AvatarInitials.vue'
import Icon from '@/components/ui/Icon.vue'
import { IconChevronDown, IconLogout, IconUser } from '@/lib/icons'
import { authStore } from '@/stores/auth'
import { translateRole } from '@/utils/roles'

const router = useRouter()
const root = ref(null)
const open = ref(false)

const user = computed(() => authStore.user)
const fullName = computed(() =>
  [user.value?.name, user.value?.lastName].filter(Boolean).join(' ') || 'Usuario'
)
const roleLabel = computed(() => translateRole(user.value?.roles?.filter(Boolean)[0] ?? ''))
const profileRoute = computed(() => (authStore.hasRole('Patient') ? '/portal/perfil' : '/perfil'))

// Dropdown propio: el JS de Bootstrap nunca se importa en este proyecto, así
// que los data-bs-toggle del navbar anterior no hacían nada.
function onDocumentClick(e) {
  if (open.value && root.value && !root.value.contains(e.target)) open.value = false
}
function onKeydown(e) {
  if (e.key === 'Escape' && open.value) open.value = false
}

onMounted(() => {
  document.addEventListener('click', onDocumentClick)
  document.addEventListener('keydown', onKeydown)
})
onBeforeUnmount(() => {
  document.removeEventListener('click', onDocumentClick)
  document.removeEventListener('keydown', onKeydown)
})

watch(() => router.currentRoute.value.fullPath, () => {
  open.value = false
})

function logout() {
  open.value = false
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.user-menu {
  position: relative;
}

.user-menu__trigger {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.25rem 0.5rem 0.25rem 0.25rem;
  background: transparent;
  border: 1px solid transparent;
  border-radius: var(--app-radius-pill);
  cursor: pointer;
  transition: background-color var(--app-transition), border-color var(--app-transition);
}
.user-menu__trigger:hover {
  background-color: var(--bs-tertiary-bg);
  border-color: var(--app-border);
}

.user-menu__text {
  flex-direction: column;
  align-items: flex-start;
  line-height: 1.2;
}
.user-menu__name {
  font-size: 0.8125rem;
  font-weight: 600;
  color: var(--app-text-strong);
}
.user-menu__role {
  font-size: 0.7rem;
  color: var(--app-text-muted);
}

.user-menu__panel {
  position: absolute;
  right: 0;
  top: calc(100% + 0.5rem);
  z-index: 1030;
  min-width: 232px;
  padding: 0.375rem;
  background-color: var(--app-surface-raised);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius-lg);
  box-shadow: var(--app-shadow-lg);
}

.user-menu__header {
  padding: 0.5rem 0.75rem 0.625rem;
  border-bottom: 1px solid var(--app-border);
  margin-bottom: 0.375rem;
}
.user-menu__header-name {
  margin: 0;
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--app-text-strong);
}
.user-menu__header-email {
  margin: 0;
  font-size: 0.75rem;
  color: var(--app-text-muted);
  overflow: hidden;
  text-overflow: ellipsis;
}

.user-menu__item {
  display: flex;
  width: 100%;
  align-items: center;
  gap: 0.625rem;
  padding: 0.5rem 0.75rem;
  border: 0;
  background: transparent;
  border-radius: var(--app-radius-sm);
  font-size: 0.8125rem;
  font-weight: 500;
  color: var(--app-text);
  text-align: left;
  cursor: pointer;
}
.user-menu__item:hover {
  background-color: var(--bs-tertiary-bg);
  color: var(--app-text-strong);
}
.user-menu__item--danger {
  color: var(--bs-danger-text-emphasis);
}
.user-menu__item--danger:hover {
  background-color: var(--bs-danger-bg-subtle);
  color: var(--bs-danger-text-emphasis);
}

.user-menu-pop-enter-active,
.user-menu-pop-leave-active {
  transition: opacity 0.14s ease, transform 0.14s ease;
}
.user-menu-pop-enter-from,
.user-menu-pop-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}
</style>
