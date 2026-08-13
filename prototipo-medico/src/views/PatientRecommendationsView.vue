<template>
  <div>
    <PageHeader
      title="Recomendaciones"
      subtitle="Recomendaciones generadas por análisis de IA"
      :icon="IconRecommendation"
    />

    <LoadingState v-if="loading" label="Cargando recomendaciones…" />

    <AppAlert v-else-if="error" variant="danger" :message="error" />

    <BaseCard v-else-if="items.length === 0" flush padding="none">
      <EmptyState
        :icon="IconRecommendation"
        title="Sin recomendaciones"
        message="Cuando un análisis genere recomendaciones para ti, aparecerán aquí."
      />
    </BaseCard>

    <div v-else class="row g-3">
      <div v-for="r in items" :key="r.id" class="col-md-6">
        <BaseCard class="h-100">
          <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
            <h3 class="rec__title">{{ r.title }}</h3>
            <StatusBadge v-if="r.riskLevel" :text="r.riskLevel" :variant="riskTone(r.riskLevel)" />
          </div>
          <p class="rec__desc">{{ r.description }}</p>
          <small class="text-app-muted">{{ formatDate(r.createdAt) }}</small>
        </BaseCard>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { portalService } from '@/api/portal'
import { AppAlert, BaseCard, EmptyState, LoadingState, PageHeader, StatusBadge } from '@/components/ui'
import { IconRecommendation } from '@/lib/icons'
import { riskTone } from '@/lib/aiAnalysis'

const loading = ref(true)
const error = ref('')
const items = ref([])

const formatDate = (dateStr) => {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' })
}

onMounted(async () => {
  try {
    const res = await portalService.getRecommendations()
    items.value = res.data || []
  } catch (err) {
    error.value =
      err.response?.status === 404
        ? 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
        : 'Error al cargar recomendaciones'
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.rec__title {
  margin: 0;
  font-size: 0.9375rem;
  font-weight: 600;
  color: var(--app-text-strong);
}

.rec__desc {
  margin: 0 0 0.5rem;
  font-size: 0.875rem;
  color: var(--app-text-muted);
}
</style>
