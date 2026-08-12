<template>
  <div class="landing">
    <!-- ============ Header ============ -->
    <header class="l-header">
      <div class="l-header__inner">
        <RouterLink to="/" class="l-header__brand">
          <BrandMark :size="34" />
        </RouterLink>
        <div class="l-header__actions">
          <ThemeToggle />
          <AppButton variant="soft" to="/login">Iniciar sesión</AppButton>
          <AppButton variant="primary" to="/registro" class="d-none d-sm-inline-flex">Crear cuenta</AppButton>
        </div>
      </div>
    </header>

    <!-- ============ Hero ============ -->
    <section class="l-hero">
      <div class="l-container l-hero__grid">
        <div class="l-hero__copy">
          <span class="l-eyebrow">
            <span class="l-pulse" aria-hidden="true" />
            Análisis clínico con inteligencia artificial
          </span>

          <h1 class="l-hero__title">
            Diagnosticar con IA,
            <span class="l-hero__title-accent">sin demoras</span>
          </h1>

          <p class="l-hero__sub">
            Plataforma clínica que conecta pacientes, médicos y administración en un solo lugar:
            citas inteligentes, signos vitales, síntomas y diagnóstico asistido por IA.
          </p>

          <div class="l-hero__cta">
            <AppButton variant="primary" size="lg" to="/registro" :icon-right="IconForward">
              Empezar ahora
            </AppButton>
            <AppButton variant="outline" size="lg" to="/login">Ya tengo cuenta</AppButton>
          </div>

          <ul class="l-checklist">
            <li v-for="item in trustItems" :key="item">
              <Icon :icon="IconSuccess" :size="16" tone="success" />
              {{ item }}
            </li>
          </ul>
        </div>

        <div class="l-hero__visual">
          <div class="l-mockup">
            <div class="l-mockup__head">
              <span class="l-mockup__head-title">Dashboard clínico</span>
              <StatusBadge text="Vista en vivo" variant="active" size="sm" dot />
            </div>

            <div class="l-mockup__stats">
              <div v-for="s in mockStats" :key="s.label" class="l-stat">
                <p class="l-stat__label">{{ s.label }}</p>
                <p class="l-stat__value">{{ s.value }}</p>
                <p class="l-stat__hint" :class="{ 'l-stat__hint--up': s.up }">{{ s.hint }}</p>
              </div>
            </div>

            <div class="l-mockup__chart">
              <p class="l-mockup__chart-label">Diagnósticos asistidos por IA (últimos 7 días)</p>
              <div class="l-bars">
                <span v-for="(h, i) in chartBars" :key="i" class="l-bar" :style="{ height: h + '%' }" />
              </div>
            </div>

            <div class="l-mockup__foot">
              <div>
                <p class="l-mockup__foot-label">Recomendación IA</p>
                <p class="l-mockup__foot-value">Seguimiento en 7 días</p>
              </div>
              <StatusBadge text="On track" variant="active" size="sm" dot />
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ============ Features ============ -->
    <section class="l-section l-section--sunken">
      <div class="l-container">
        <div class="l-section-head">
          <span class="l-eyebrow l-eyebrow--static">Todo en una plataforma</span>
          <h2 class="l-section-title">Pensada para la atención moderna</h2>
          <p class="l-section-sub">Herramientas integradas que ahorran horas a tu equipo médico cada día.</p>
        </div>

        <div class="l-features">
          <BaseCard v-for="f in features" :key="f.title" hoverable padding="md">
            <IconTile :icon="f.icon" :tone="f.tone" size="lg" class="mb-3" />
            <h3 class="l-feature__title">{{ f.title }}</h3>
            <p class="l-feature__desc">{{ f.desc }}</p>
          </BaseCard>
        </div>
      </div>
    </section>

    <!-- ============ Prueba ============ -->
    <section class="l-section">
      <div class="l-container l-proof__grid">
        <div class="l-proof__visual">
          <BaseCard padding="md">
            <div class="l-proof__header">
              <span class="fw-semibold">Expediente del paciente</span>
              <StatusBadge text="ID #00258" variant="secondary" size="sm" />
            </div>

            <div v-for="v in vitalsSample" :key="v.label" class="l-proof__row">
              <div class="l-proof__row-top">
                <span class="text-app-muted">{{ v.label }}</span>
                <strong>
                  {{ v.value }}
                  <small class="text-success-emphasis fw-semibold">{{ v.tag }}</small>
                </strong>
              </div>
              <div class="progress">
                <div class="progress-bar" :style="{ width: v.pct + '%' }" />
              </div>
            </div>
          </BaseCard>
        </div>

        <div class="l-proof__copy">
          <span class="l-eyebrow l-eyebrow--static">Asistencia real</span>
          <h2 class="l-section-title mt-3">
            Datos clínicos, <span class="l-hero__title-accent">interpretados por IA</span>
          </h2>
          <p class="l-proof__sub">
            La IA cruza síntomas, signos vitales e historial para sugerir diagnósticos y alertar
            al equipo médico sobre riesgos en el momento exacto.
          </p>

          <div class="l-proof__list">
            <div v-for="item in aiHighlights" :key="item.title" class="l-proof__item">
              <IconTile :icon="item.icon" :tone="item.tone" size="md" />
              <div>
                <strong class="d-block">{{ item.title }}</strong>
                <p class="mb-0 text-app-muted">{{ item.desc }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ============ CTA final ============ -->
    <section class="l-container">
      <div class="l-cta">
        <h2 class="l-cta__title">El futuro de tu consulta empieza hoy</h2>
        <p class="l-cta__sub">
          Únete a los centros que ya diagnostican más rápido con Smart-Medical IA.
        </p>
        <div class="l-cta__actions">
          <AppButton variant="on-brand" size="lg" to="/registro">Crear cuenta gratis</AppButton>
          <AppButton variant="on-brand-outline" size="lg" to="/login">Iniciar sesión</AppButton>
        </div>
      </div>
    </section>

    <!-- ============ Footer ============ -->
    <footer class="l-footer">
      <div class="l-container l-footer__inner">
        <BrandMark :size="26" compact />
        <small class="text-app-muted">© {{ year }} Smart-Medical IA. Proyecto académico.</small>
        <div class="l-footer__links">
          <RouterLink to="/login">Acceso</RouterLink>
          <RouterLink to="/registro">Registro</RouterLink>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { markRaw } from 'vue'
import AppButton from '@/components/ui/AppButton.vue'
import BaseCard from '@/components/ui/BaseCard.vue'
import BrandMark from '@/components/ui/BrandMark.vue'
import Icon from '@/components/ui/Icon.vue'
import IconTile from '@/components/ui/IconTile.vue'
import StatusBadge from '@/components/ui/StatusBadge.vue'
import ThemeToggle from '@/components/ui/ThemeToggle.vue'
import {
  IconAi,
  IconAlert,
  IconChart,
  IconClinical,
  IconFast,
  IconForward,
  IconReport,
  IconSuccess,
  IconVitals
} from '@/lib/icons'

const year = new Date().getFullYear()

const chartBars = [42, 58, 46, 72, 64, 88, 96]

const trustItems = ['Sin instalación', 'Datos privados', 'Gratis para pacientes']

const mockStats = [
  { label: 'Pacientes', value: '1,248', hint: '+12%', up: true },
  { label: 'Citas agendadas', value: '86', hint: 'hoy', up: false },
  { label: 'Análisis IA', value: '532', hint: 'esta semana', up: true }
]

// markRaw: son componentes de icono en un array de datos; sin esto Vue los
// envolvería en proxies reactivos sin necesidad.
const features = [
  {
    icon: markRaw(IconClinical),
    tone: 'brand',
    title: 'Citas inteligentes',
    desc: 'Agenda, síntomas y signos vitales en un solo flujo por cita.'
  },
  {
    icon: markRaw(IconAi),
    tone: 'info',
    title: 'Diagnóstico con IA',
    desc: 'Recomendaciones basadas en el historial completo del paciente.'
  },
  {
    icon: markRaw(IconVitals),
    tone: 'success',
    title: 'Signos vitales',
    desc: 'Registro y seguimiento de presión, frecuencia y saturación.'
  },
  {
    icon: markRaw(IconChart),
    tone: 'warning',
    title: 'Reportes en PDF',
    desc: 'Resúmenes clínicos listos para imprimir o compartir.'
  }
]

const vitalsSample = [
  { label: 'Presión arterial', value: '120/80', tag: 'Normal', pct: 72 },
  { label: 'Frecuencia cardíaca', value: '68 bpm', tag: 'Normal', pct: 85 },
  { label: 'Saturación de oxígeno', value: '98%', tag: 'Óptimo', pct: 96 }
]

const aiHighlights = [
  {
    icon: markRaw(IconFast),
    tone: 'brand',
    title: 'Sugerencias de diagnóstico',
    desc: 'Recomendaciones basadas en el expediente completo del paciente.'
  },
  {
    icon: markRaw(IconAlert),
    tone: 'danger',
    title: 'Alertas tempranas',
    desc: 'Detecta valores críticos y notifica al equipo al instante.'
  },
  {
    icon: markRaw(IconReport),
    tone: 'info',
    title: 'Reportes listos',
    desc: 'Genera resúmenes y reportes en PDF con un clic.'
  }
]

</script>

<style scoped>
.landing {
  background-color: var(--app-canvas);
}

.l-container {
  width: 100%;
  max-width: 1180px;
  margin-inline: auto;
  padding-inline: 1.25rem;
}

@media (min-width: 768px) {
  .l-container {
    padding-inline: 1.5rem;
  }
}

/* ============ Header ============ */
.l-header {
  position: sticky;
  top: 0;
  z-index: 50;
  background-color: color-mix(in srgb, var(--app-surface) 86%, transparent);
  backdrop-filter: saturate(180%) blur(8px);
  border-bottom: 1px solid var(--app-border);
}

.l-header__inner {
  max-width: 1180px;
  margin-inline: auto;
  padding: 0.875rem 1.25rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

.l-header__brand {
  display: inline-flex;
}

.l-header__actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

/* ============ Eyebrow / pulse ============ */
.l-eyebrow {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.375rem 0.875rem;
  border-radius: var(--app-radius-pill);
  background-color: var(--bs-primary-bg-subtle);
  color: var(--bs-primary-text-emphasis);
  font-size: 0.8rem;
  font-weight: 600;
}

.l-eyebrow--static {
  margin-bottom: 0.75rem;
}

.l-pulse {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: var(--c-success-500);
  box-shadow: 0 0 0 3px var(--c-success-subtle);
}

/* ============ Hero ============ */
.l-hero {
  padding-block: clamp(2.5rem, 6vw, 5rem);
  background:
    radial-gradient(
      50rem 28rem at 15% -8rem,
      color-mix(in srgb, var(--c-brand-500) 16%, transparent),
      transparent 70%
    ),
    var(--app-canvas);
}

.l-hero__grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 3rem;
  align-items: center;
}

@media (min-width: 992px) {
  .l-hero__grid {
    grid-template-columns: 1fr 1fr;
  }
}

.l-hero__title {
  margin: 1.25rem 0 1rem;
  font-size: clamp(2rem, 1.5rem + 2vw, 2.75rem);
  line-height: 1.12;
  font-weight: 700;
}

.l-hero__title-accent {
  color: var(--bs-primary-text-emphasis);
}

.l-hero__sub {
  max-width: 34rem;
  color: var(--app-text-muted);
  font-size: 1.05rem;
}

.l-hero__cta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  margin: 1.5rem 0;
}

.l-checklist {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem 1.5rem;
}

.l-checklist li {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--app-text);
}

@media (max-width: 991.98px) {
  .l-hero__copy {
    text-align: center;
  }
  .l-hero__sub {
    margin-inline: auto;
  }
  .l-hero__cta,
  .l-checklist {
    justify-content: center;
  }
}

/* ============ Mockup del dashboard ============ */
.l-hero__visual {
  position: relative;
}

.l-mockup {
  position: relative;
  padding: 1.25rem;
  background-color: var(--app-surface);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius-xl);
  box-shadow: var(--app-shadow-lg);
}

/* Marco de acento detrás de la tarjeta: color puro de marca, no depende del
   tema (a diferencia del resto de la superficie). */
.l-mockup::before {
  content: '';
  position: absolute;
  inset: -14px 10px 10px -14px;
  background: linear-gradient(135deg, var(--c-brand-500), var(--c-brand-700));
  border-radius: calc(var(--app-radius-xl) + 6px);
  transform: rotate(1.5deg);
  z-index: -1;
  opacity: 0.9;
}

.l-mockup__head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-bottom: 0.875rem;
  margin-bottom: 0.875rem;
  border-bottom: 1px solid var(--app-border);
}

.l-mockup__head-title {
  font-weight: 600;
}

.l-mockup__stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.625rem;
}

.l-stat {
  padding: 0.75rem;
  background-color: var(--app-surface-sunken);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius);
}

.l-stat__label {
  margin: 0;
  font-size: 0.7rem;
  color: var(--app-text-muted);
}

.l-stat__value {
  margin: 0.125rem 0;
  font-size: 1.15rem;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
}

.l-stat__hint {
  margin: 0;
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--app-text-muted);
}

.l-stat__hint--up {
  color: var(--c-success-emphasis);
}

.l-mockup__chart {
  margin-top: 0.875rem;
  padding: 0.875rem;
  background-color: var(--app-surface-sunken);
  border: 1px solid var(--app-border);
  border-radius: var(--app-radius);
}

.l-mockup__chart-label {
  margin: 0 0 0.5rem;
  font-size: 0.75rem;
  color: var(--app-text-muted);
}

.l-bars {
  height: 90px;
  display: flex;
  align-items: flex-end;
  gap: 0.375rem;
}

.l-bar {
  flex: 1;
  background: linear-gradient(180deg, var(--c-brand-400), var(--c-brand-600));
  border-radius: 6px 6px 2px 2px;
}

.l-mockup__foot {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: 0.875rem;
  padding-top: 0.875rem;
  border-top: 1px solid var(--app-border);
}

.l-mockup__foot-label {
  margin: 0;
  font-size: 0.72rem;
  color: var(--app-text-muted);
}

.l-mockup__foot-value {
  margin: 0;
  font-weight: 600;
}

/* ============ Secciones ============ */
.l-section {
  padding-block: clamp(3rem, 6vw, 5rem);
}

.l-section--sunken {
  background-color: var(--app-surface-sunken);
  border-block: 1px solid var(--app-border);
}

.l-section-head {
  max-width: 40rem;
  margin: 0 auto 2.5rem;
  text-align: center;
}

.l-section-title {
  margin: 0 0 0.625rem;
  font-size: clamp(1.5rem, 1.2rem + 1vw, 1.9rem);
  font-weight: 700;
}

.l-section-sub {
  margin: 0;
  color: var(--app-text-muted);
  font-size: 1.02rem;
}

/* ============ Features ============ */
.l-features {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(230px, 1fr));
  gap: 1.25rem;
}

.l-feature__title {
  font-size: 1rem;
  font-weight: 600;
  margin: 0 0 0.375rem;
}

.l-feature__desc {
  margin: 0;
  font-size: 0.875rem;
  color: var(--app-text-muted);
}

/* ============ Prueba ============ */
.l-proof__grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 3rem;
  align-items: center;
}

@media (min-width: 992px) {
  .l-proof__grid {
    grid-template-columns: 1fr 1fr;
  }
}

.l-proof__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.l-proof__row {
  padding-block: 0.5rem;
}

.l-proof__row-top {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  font-size: 0.9rem;
  margin-bottom: 0.375rem;
}

.l-proof__sub {
  max-width: 34rem;
  color: var(--app-text-muted);
  margin: 0 0 1.5rem;
}

.l-proof__list {
  display: grid;
  gap: 1.125rem;
}

.l-proof__item {
  display: flex;
  align-items: flex-start;
  gap: 0.875rem;
}

/* ============ CTA final ============ */
.l-cta {
  position: relative;
  overflow: hidden;
  text-align: center;
  margin-block: 1rem 4rem;
  padding: clamp(2.5rem, 6vw, 4rem) 1.5rem;
  border-radius: var(--app-radius-xl);
  background: linear-gradient(135deg, var(--c-brand-600), var(--c-brand-800));
  box-shadow: 0 25px 50px -12px color-mix(in srgb, var(--c-brand-700) 45%, transparent);
}

.l-cta__title {
  margin: 0 0 0.75rem;
  color: #fff;
  font-size: clamp(1.5rem, 1.2rem + 1.2vw, 2rem);
  font-weight: 700;
}

.l-cta__sub {
  max-width: 32rem;
  margin: 0 auto 1.75rem;
  color: rgba(255, 255, 255, 0.85);
}

.l-cta__actions {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 0.75rem;
}

/* ============ Footer ============ */
.l-footer {
  border-top: 1px solid var(--app-border);
  padding-block: 1.5rem;
}

.l-footer__inner {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  text-align: center;
}

@media (min-width: 768px) {
  .l-footer__inner {
    flex-direction: row;
    justify-content: space-between;
    text-align: left;
  }
}

.l-footer__links {
  display: flex;
  gap: 1.25rem;
  font-size: 0.875rem;
}

.l-footer__links a {
  color: var(--app-text-muted);
}

.l-footer__links a:hover {
  color: var(--bs-primary-text-emphasis);
}
</style>
