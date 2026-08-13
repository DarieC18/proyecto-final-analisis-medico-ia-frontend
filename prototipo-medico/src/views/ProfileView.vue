<template>
  <div>
    <PageHeader title="Mi Perfil" subtitle="Información de tu cuenta" :icon="IconUser" :back-to="rutaVolver" />

    <div class="profile__identity">
      <AvatarInitials :name="user?.name" :last-name="user?.lastName" size="lg" />
      <div>
        <h2 class="profile__name">{{ user?.name }} {{ user?.lastName }}</h2>
        <p class="profile__role">{{ translateRole(rolActual) }}</p>
      </div>
    </div>

    <LoadingState v-if="loading" label="Cargando perfil…" />

    <BaseCard v-else title="Información de la cuenta" :icon="IconSecurity">
      <div class="row g-4">
        <div class="col-md-6">
          <DataField label="Nombre" :value="user?.name" />
        </div>
        <div class="col-md-6">
          <DataField label="Apellido" :value="user?.lastName" />
        </div>
        <div class="col-md-6">
          <DataField label="Usuario" :value="user?.userName" />
        </div>
        <div class="col-md-6">
          <DataField label="Email" :value="user?.email" :icon="IconEmail" />
        </div>
        <div class="col-md-6">
          <DataField label="Identificación" :value="user?.numberIdentification" :icon="IconIdentification" />
        </div>
        <div class="col-md-6">
          <DataField label="Rol" :icon="IconRoles">
            <StatusBadge :text="translateRole(rolActual)" variant="info" />
          </DataField>
        </div>
        <div v-if="rolActual === 'Doctor'" class="col-md-6">
          <DataField label="Especialidad" :value="user?.specialty || 'No especificada'" />
        </div>
        <div class="col-md-6">
          <DataField label="Estado">
            <StatusBadge
              :text="user?.status ? 'Activo' : 'Inactivo'"
              :variant="user?.status ? 'active' : 'inactive'"
              dot
            />
          </DataField>
        </div>
        <div class="col-md-6">
          <DataField label="Verificado">
            <StatusBadge
              :text="user?.isVerified ? 'Sí' : 'No'"
              :variant="user?.isVerified ? 'active' : 'inactive'"
              dot
            />
          </DataField>
        </div>
        <div class="col-12">
          <DataField label="Fecha de registro" :value="formatFecha(user?.createdAt)" :icon="IconBirthdate" />
        </div>
      </div>
    </BaseCard>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { authService } from '@/api/auth'
import { authStore } from '@/stores/auth'
import { AvatarInitials, BaseCard, DataField, LoadingState, PageHeader, StatusBadge } from '@/components/ui'
import { IconBirthdate, IconEmail, IconIdentification, IconRoles, IconSecurity, IconUser } from '@/lib/icons'
import { translateRole } from '@/utils/roles'
import { homeRouteForRoles } from '@/config/navigation'

const auth = authStore
const loading = ref(true)
const user = ref(null)

const rolActual = computed(() => user.value?.role || user.value?.roles?.[0] || '')

// El botón de volver reutiliza la misma fuente que el guard del router y la
// marca del sidebar; antes esta vista tenía su propia cadena de if/else que
// podía discrepar de `homeRouteForRoles`.
const rutaVolver = computed(() => homeRouteForRoles(auth.user?.roles ?? []) ?? '/login')

const formatFecha = (dateStr) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString('es-ES', {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  })
}

onMounted(async () => {
  loading.value = true
  try {
    const res = await authService.me()
    user.value = res.data
    auth.setUserFromMe(res.data)
  } catch {
    user.value = auth.user
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.profile__identity {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.profile__name {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--app-text-strong);
}

.profile__role {
  margin: 0.125rem 0 0;
  font-size: 0.875rem;
  color: var(--app-text-muted);
}
</style>
