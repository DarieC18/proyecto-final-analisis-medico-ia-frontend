<template>
  <div>
    <LoadingState v-if="loading" label="Cargando perfil…" />

    <AppAlert v-else-if="error" variant="danger" :message="error" />

    <template v-else>
      <div class="profile__identity">
        <AvatarInitials :name="nombre" :last-name="apellido" size="lg" />
        <div>
          <h1 class="profile__name">{{ profile?.fullName }}</h1>
          <p class="profile__role">Paciente</p>
        </div>
      </div>

      <BaseCard title="Mi Perfil" :icon="IconUser">
        <div class="row g-4">
          <div class="col-md-6">
            <DataField label="Nombre completo" :value="profile?.fullName" :icon="IconUser" />
          </div>
          <div class="col-md-6">
            <DataField label="Usuario" :value="profile?.userName" />
          </div>
          <div class="col-md-6">
            <DataField label="Email" :value="profile?.email" :icon="IconEmail" />
          </div>
          <div class="col-md-6">
            <DataField label="Identificación" :value="profile?.numberIdentification" :icon="IconIdentification" />
          </div>
          <div class="col-md-6">
            <DataField label="Teléfono" :value="profile?.phoneNumber" :icon="IconPhone" />
          </div>
          <div class="col-md-6">
            <DataField label="Género" :value="profile?.gender" />
          </div>
          <div class="col-md-6">
            <DataField label="Fecha de nacimiento" :value="formatDate(profile?.birthDate)" :icon="IconBirthdate" />
          </div>
          <div class="col-md-6">
            <DataField label="Tipo de paciente" :value="profile?.patientType" />
          </div>
        </div>
      </BaseCard>
    </template>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { portalService } from '@/api/portal'
import { AppAlert, AvatarInitials, BaseCard, DataField, LoadingState } from '@/components/ui'
import { IconBirthdate, IconEmail, IconIdentification, IconPhone, IconUser } from '@/lib/icons'

const loading = ref(true)
const error = ref('')
const profile = ref(null)

// AvatarInitials compone las iniciales a partir de nombre + apellido, pero el
// portal sólo expone `fullName`: se parte aquí en vez de duplicar la lógica.
const partesNombre = computed(() => (profile.value?.fullName ?? '').trim().split(/\s+/).filter(Boolean))
const nombre = computed(() => partesNombre.value[0] ?? '')
const apellido = computed(() => partesNombre.value[1] ?? '')

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' })
}

onMounted(async () => {
  try {
    const res = await portalService.getProfile()
    profile.value = res.data
  } catch (err) {
    error.value =
      err.response?.status === 404
        ? 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
        : 'Error al cargar perfil'
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
  font-size: 1.375rem;
  font-weight: 600;
  color: var(--app-text-strong);
}

.profile__role {
  margin: 0.125rem 0 0;
  font-size: 0.875rem;
  color: var(--app-text-muted);
}
</style>
