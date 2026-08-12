<template>
  <div>
    <PageHeader
      title="Resultados de análisis"
      subtitle="Análisis de IA generados para tus consultas"
      :icon="IconAi"
    />

    <LoadingState v-if="loading" label="Cargando resultados…" />

    <AppAlert v-else-if="error" variant="danger" :message="error" />

    <BaseCard v-else-if="results.length === 0" flush padding="none">
      <EmptyState
        :icon="IconAi"
        title="Sin resultados todavía"
        message="Cuando un médico genere un análisis de IA en una de tus consultas, aparecerá aquí."
      />
    </BaseCard>

    <div v-else class="d-grid gap-3">
      <BaseCard
        v-for="r in results"
        :key="r.id"
        :title="r.analysisType || 'Análisis clínico'"
        :subtitle="formatDate(r.createdAt)"
        :icon="IconAi"
      >
        <template #header-actions>
          <StatusBadge
            :text="r.isReviewed ? 'Revisado' : 'Pendiente'"
            :variant="r.isReviewed ? 'active' : 'pending'"
            dot
          />
        </template>

        <template v-if="r.aiResponse && !isAiError(r.aiResponse)">
          <div class="d-grid gap-3">
            <p v-if="parsed(r)?.summary" class="result__summary">{{ parsed(r).summary }}</p>

            <div v-if="parsed(r)?.riskLevel">
              <p class="result__label">Nivel de riesgo</p>
              <StatusBadge :text="parsed(r).riskLevel" :variant="riskTone(parsed(r).riskLevel)" />
            </div>

            <div v-if="parsed(r)?.recommendations?.length">
              <p class="result__label">Recomendaciones</p>
              <ul class="result__recommendations">
                <li v-for="(rec, i) in parsed(r).recommendations" :key="i">
                  <p class="result__rec-title">{{ rec.title }}</p>
                  <p class="result__rec-desc">{{ rec.description }}</p>
                </li>
              </ul>
            </div>

            <!-- Respaldo: si la respuesta no es el JSON esperado, se muestra el
                 texto tal cual en lugar de fallar en silencio. -->
            <pre v-if="!parsed(r)" class="result__raw">{{ r.aiResponse }}</pre>
          </div>
        </template>

        <AppAlert v-else-if="isAiError(r.aiResponse)" variant="danger" :message="r.aiResponse" />

        <template #footer>
          <div class="d-flex justify-content-between">
            <small class="text-app-muted">Estado: {{ r.status || 'N/D' }}</small>
          </div>
        </template>
      </BaseCard>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { AppAlert, BaseCard, EmptyState, LoadingState, PageHeader, StatusBadge } from '@/components/ui'
import { IconAi } from '@/lib/icons'
import { isAiError, parseAiResponse, riskTone } from '@/lib/aiAnalysis'
import { portalService } from '@/api/portal'

const loading = ref(true)
const error = ref('')
const results = ref([])

// Cachea el parseo por resultado para no re-parsear el mismo JSON en cada
// referencia dentro de la plantilla (summary, riskLevel, recommendations).
const parsedCache = new WeakMap()
const parsed = (r) => {
  if (!parsedCache.has(r)) parsedCache.set(r, parseAiResponse(r.aiResponse))
  return parsedCache.get(r)
}

const formatDate = (dateStr) => {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('es-ES', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

onMounted(async () => {
  try {
    const res = await portalService.getResults()
    results.value = res.data || []
  } catch (err) {
    error.value =
      err.response?.status === 404
        ? 'No tienes un perfil de paciente registrado. Contacta a un administrador para que vincule tu cuenta.'
        : 'Error al cargar resultados'
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.result__summary {
  margin: 0;
  color: var(--app-text);
}

.result__label {
  margin: 0 0 0.375rem;
  font-size: 0.72rem;
  font-weight: 600;
  letter-spacing: 0.05em;
  text-transform: uppercase;
  color: var(--app-text-muted);
}

.result__recommendations {
  list-style: none;
  margin: 0;
  padding: 0;
  display: grid;
  gap: 0.625rem;
}

.result__recommendations li {
  padding-inline-start: 0.75rem;
  border-inline-start: 2px solid var(--bs-info-border-subtle);
}

.result__rec-title {
  margin: 0;
  font-weight: 600;
  font-size: 0.875rem;
  color: var(--app-text-strong);
}

.result__rec-desc {
  margin: 0.125rem 0 0;
  font-size: 0.825rem;
  color: var(--app-text-muted);
}

.result__raw {
  margin: 0;
  padding: 0.75rem;
  background-color: var(--bs-tertiary-bg);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius-sm);
  font-size: 0.8rem;
  color: var(--app-text);
  white-space: pre-wrap;
  overflow-x: auto;
  max-height: 320px;
}
</style>
