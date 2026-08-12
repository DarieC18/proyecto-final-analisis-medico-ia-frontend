<template>
  <div class="auth">
    <div class="auth__panel" :class="{ 'auth__panel--wide': wide }">
      <RouterLink to="/" class="auth__brand">
        <BrandMark :size="40" />
      </RouterLink>

      <div class="auth__card">
        <header class="auth__head">
          <IconTile v-if="icon" :icon="icon" :tone="tone" size="lg" class="mb-3" />
          <h1 class="auth__title">{{ title }}</h1>
          <p v-if="subtitle" class="auth__subtitle">{{ subtitle }}</p>
        </header>

        <slot />
      </div>

      <div v-if="$slots.footer" class="auth__footer">
        <slot name="footer" />
      </div>
    </div>
  </div>
</template>

<script setup>
import BrandMark from '@/components/ui/BrandMark.vue'
import IconTile from '@/components/ui/IconTile.vue'

/** Envoltorio común de las cinco pantallas de acceso (login, registro,
    recuperación, restablecimiento y confirmación de cuenta). */
defineProps({
  title: { type: String, required: true },
  subtitle: { type: String, default: '' },
  icon: { type: [Object, Function], default: null },
  tone: { type: String, default: 'brand' },
  wide: Boolean
})
</script>

<style scoped>
.auth {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100dvh;
  padding: 2rem 1rem 3rem;
  /* Halo de marca muy tenue: da temperatura al fondo sin competir con el
     formulario. Se atenúa solo en oscuro porque el token cambia. */
  background:
    radial-gradient(
      60rem 34rem at 50% -12rem,
      color-mix(in srgb, var(--c-brand-500) 14%, transparent),
      transparent 70%
    ),
    var(--app-canvas);
}

.auth__panel {
  width: 100%;
  max-width: 420px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.25rem;
}

.auth__panel--wide {
  max-width: 760px;
}

.auth__brand {
  display: inline-flex;
}

.auth__card {
  width: 100%;
  padding: 1.75rem;
  background-color: var(--app-surface);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius-xl);
  box-shadow: var(--app-shadow-lg);
}

.auth__head {
  text-align: center;
  margin-bottom: 1.5rem;
}

.auth__title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
}

.auth__subtitle {
  margin: 0.25rem 0 0;
  font-size: 0.875rem;
  color: var(--app-text-muted);
}

.auth__footer {
  text-align: center;
  font-size: 0.825rem;
  color: var(--app-text-muted);
}

@media (max-width: 575.98px) {
  .auth__card {
    padding: 1.25rem;
  }
}
</style>
