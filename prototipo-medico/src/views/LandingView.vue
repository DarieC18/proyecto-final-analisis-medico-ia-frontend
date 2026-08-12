<template>
  <div class="landing bg-white">
    <!-- Header -->
    <header class="header d-flex align-items-center justify-content-between">
      <RouterLink to="/" class="brand d-flex align-items-center gap-2">
        <span class="brand-icon d-flex align-items-center justify-content-center">⚕️</span>
        <span class="fw-bold">MedAnalyzer</span>
      </RouterLink>
      <div class="d-flex align-items-center gap-2">
        <RouterLink to="/login" class="btn btn-login">Iniciar sesión</RouterLink>
        <RouterLink to="/registro" class="btn btn-primary btn-cta">Crear cuenta</RouterLink>
      </div>
    </header>

    <!-- Hero -->
    <section class="hero container">
      <div class="row align-items-center gy-5">
        <div class="col-lg-6">
          <span class="badge rounded-pill px-3 py-2 eyebrow d-inline-flex align-items-center gap-2">
            <span class="pulse-dot"></span> Análisis clínico con inteligencia artificial
          </span>
          <h1 class="fw-bold hero-title mt-4 mb-3">
            Diagnosticar con IA, <span class="text-primary">sin demoras</span>
          </h1>
          <p class="hero-sub mt-3 mb-4">
            Plataforma clínica que conecta pacientes, médicos y administración en un solo lugar:
            citas inteligentes, signos vitales, síntomas y diagnóstico asistido por IA.
          </p>
          <div class="d-flex flex-wrap gap-3 mb-4">
            <RouterLink to="/registro" class="btn btn-primary btn-lg px-4 shadow-lg d-inline-flex align-items-center gap-2">
              Empezar ahora
              <span>→</span>
            </RouterLink>
            <RouterLink to="/login" class="btn btn-outline-secondary btn-lg px-4">
              Ya tengo cuenta
            </RouterLink>
          </div>
          <div class="d-flex flex-wrap gap-4 trusted">
            <span class="d-flex align-items-center gap-2"><i class="check text-success">✔</i> Sin instalación</span>
            <span class="d-flex align-items-center gap-2"><i class="check text-success">✔</i> Datos privados</span>
            <span class="d-flex align-items-center gap-2"><i class="check text-success">✔</i> Gratis para pacientes</span>
          </div>
        </div>
        <div class="col-lg-6">
          <div class="mockup-wrap">
            <div class="mockup">
              <div class="mockup-head d-flex justify-content-between align-items-center">
                <span class="fw-semibold">Dashboard clínico</span>
                <span class="badge virtual-badge">Vista en vivo</span>
              </div>
              <div class="mockup-stat row g-3">
                <div class="col">
                  <div class="stat-box">
                    <small>Pacientes</small>
                    <strong>1,248</strong>
                    <span class="trend up">+12%</span>
                  </div>
                </div>
                <div class="col">
                  <div class="stat-box">
                    <small>Citas agendadas</small>
                    <strong>86</strong>
                    <span class="trend neutral">hoy</span>
                  </div>
                </div>
                <div class="col">
                  <div class="stat-box">
                    <small>Análisis IA</small>
                    <strong>532</strong>
                    <span class="trend up">esta semana</span>
                  </div>
                </div>
              </div>
              <div class="mockup-chart card-body">
                <small class="text-muted">Diagnósticos asistidos por IA (últimos 7 días)</small>
                <div class="bars d-flex align-items-end gap-2">
                  <span class="bar" v-for="(h, i) in bars" :key="i" :style="{ height: h + '%' }"></span>
                </div>
              </div>
              <div class="mockup-foot d-flex justify-content-between align-items-center">
                <div>
                  <small class="text-muted d-block">Recomendación IA</small>
                  <strong>Seguimiento en 7 días</strong>
                </div>
                <span class="status-badge">● On track</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Features -->
    <section class="features">
      <div class="container">
        <div class="text-center mx-auto section-head">
          <span class="eyebrow d-inline-block px-3 py-2 rounded-pill">Todo en una plataforma</span>
          <h2 class="fw-bold section-title">Pensada para la atención moderna</h2>
          <p class="text-secondary section-sub mx-auto">
            Herramientas integradas que ahorran horas a tu equipo médico cada día.
          </p>
        </div>
        <div class="row g-4">
          <div class="col-md-6 col-lg-3" v-for="f in features" :key="f.title">
            <div class="card feature-card h-100">
              <div class="feature-icon d-flex align-items-center justify-content-center" :style="{ background: f.bg, color: f.color }">
                {{ f.icon }}
              </div>
              <h3 class="h6 fw-semibold">{{ f.title }}</h3>
              <p class="feature-desc">{{ f.desc }}</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Prueba / Demo -->
    <section class="proof">
      <div class="container">
        <div class="row align-items-center gy-5">
          <div class="col-lg-6">
            <div class="proof-visual rounded-3 shadow-2xl-soft p-4">
              <div class="d-flex justify-content-between align-items-center mb-3">
                <span class="fw-semibold">Expediente del paciente</span>
                <span class="badge text-bg-light border">ID #00258</span>
              </div>
              <div class="proof-row d-flex justify-content-between">
                <span>Presión arterial</span>
                <strong>120/80 <small class="text-success">Normal</small></strong>
              </div>
              <div class="progress mb-3"><div class="progress-bar-a" style="width: 72%"></div></div>
              <div class="proof-row d-flex justify-content-between">
                <span>Frecuencia cardíaca</span>
                <strong>68 bpm <small class="text-success">Normal</small></strong>
              </div>
              <div class="progress mb-3"><div class="progress-bar-b" style="width: 85%"></div></div>
              <div class="proof-row d-flex justify-content-between">
                <span>Saturación de oxígeno</span>
                <strong>98% <small class="text-success">Óptimo</small></strong>
              </div>
              <div class="progress"><div class="progress-bar-c" style="width: 96%"></div></div>
            </div>
          </div>
          <div class="col-lg-6">
            <span class="eyebrow d-inline-block px-3 py-2 rounded-pill">Asistencia real</span>
            <h2 class="fw-bold section-title mt-3 mb-3">
              Datos clínicos, <span class="text-primary">interpretados por IA</span>
            </h2>
            <p class="proof-sub mb-4">
              La IA cruza síntomas, signos vitales e historial para sugerir diagnósticos
              y alertar al equipo médico sobre riesgos en el momento exacto.
            </p>
            <div class="proof-list">
              <div class="proof-item d-flex align-items-start gap-3">
                <span class="proof-dot d-flex align-items-center justify-content-center">⚡</span>
                <div>
                  <strong>Sugerencias de diagnóstico</strong>
                  <p class="mb-0">Recomendaciones basadas en el expediente completo del paciente.</p>
                </div>
              </div>
              <div class="proof-item d-flex align-items-start gap-3">
                <span class="proof-dot d-flex align-items-center justify-content-center">🔔</span>
                <div>
                  <strong>Alertas tempranas</strong>
                  <p class="mb-0">Detecta valores críticos y notifica al equipo al instante.</p>
                </div>
              </div>
              <div class="proof-item d-flex align-items-start gap-3">
                <span class="proof-dot d-flex align-items-center justify-content-center">📄</span>
                <div>
                  <strong>Reportes listos</strong>
                  <p class="mb-0">Genera resúmenes y reportes en PDF con un clic.</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA final -->
    <section class="cta-final container text-center rounded-4">
      <h2 class="fw-bold text-white cta-title mb-3">El futuro de tu consulta empieza hoy</h2>
      <p class="cta-sub text-white opacity-75 mx-auto mb-4">
        Únete a los centros que ya diagnostican más rápido con MedAnalyzer.
      </p>
      <div class="d-flex flex-wrap gap-3 justify-content-center">
        <RouterLink to="/registro" class="btn btn-cta-final btn-lg px-4">Crear cuenta gratis</RouterLink>
        <RouterLink to="/login" class="btn btn-outline-light btn-lg px-4">Iniciar sesión</RouterLink>
      </div>
    </section>

    <!-- Footer -->
    <footer class="footer container d-flex flex-column flex-md-row justify-content-between align-items-center gap-3">
      <div class="d-flex align-items-center gap-2">
        <span class="brand-mark small d-flex align-items-center justify-content-center">⚕️</span>
        <span class="fw-semibold">MedAnalyzer</span>
      </div>
      <small class="text-muted">© 2026 MedAnalyzer. Proyecto académico.</small>
      <div class="d-flex gap-4 small text-muted">
        <RouterLink to="/login" class="footer-link">Acceso</RouterLink>
        <RouterLink to="/registro" class="footer-link">Registro</RouterLink>
      </div>
    </footer>
  </div>
</template>

<script setup>
const bars = [42, 58, 46, 72, 64, 88, 96]

const features = [
  { icon: '🩺', title: 'Citas inteligentes', desc: 'Agenda, síntomas y signos vitales en un solo flujo por cita.', bg: '#e0f2fe', color: '#0ea5e9' },
  { icon: '🧠', title: 'Diagnóstico con IA', desc: 'Recomendaciones basadas en el historial completo del paciente.', bg: '#ede9fe', color: '#8b5cf6' },
  { icon: '🫀', title: 'Signos vitales', desc: 'Registro y seguimiento de presión, frecuencia y saturación.', bg: '#d1fae5', color: '#10b981' },
  { icon: '📊', title: 'Reportes en PDF', desc: 'Resúmenes clínicos listos para imprimir o compartir.', bg: '#fee2e2', color: '#ef4444' }
]
</script>

<style scoped>
.landing {
  font-family: 'Poppins', sans-serif;
  color: #111827;
  min-height: 100vh;
}

/* Header */
.header {
  position: sticky;
  top: 0;
  z-index: 50;
  padding: 1rem clamp(1.25rem, 5vw, 4rem);
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(8px);
  border-bottom: 1px solid #f3f4f6;
}
.brand-mark {
  width: 38px;
  height: 38px;
  border-radius: 12px;
  background: linear-gradient(135deg, #0ea5e9, #2563eb);
  color: #fff;
  font-size: 1.1rem;
}
.brand-mark.small {
  width: 30px;
  height: 30px;
  font-size: 0.9rem;
  border-radius: 9px;
}
.brand {
  text-decoration: none;
  color: #111827;
  font-size: 1.1rem;
}
.btn-login {
  background: #fff;
  border: 1px solid #e5e7eb;
  color: #111827;
  font-weight: 600;
  border-radius: 10px;
  padding: 0.5rem 1.1rem;
}
.btn-cta {
  border-radius: 10px;
  font-weight: 600;
}
.eyebrow {
  background: #e0f2fe;
  color: #0ea5e9;
  font-size: 0.8rem;
  font-weight: 600;
}
.pulse-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #10b981;
  display: inline-block;
}

/* Hero */
.hero {
  padding-top: 80px;
  padding-bottom: 80px;
  max-width: 1200px;
}
.hero-title {
  font-size: 2.6rem;
  line-height: 1.1;
}
.hero-sub {
  max-width: 520px;
}
.trusted .check {
  font-style: normal;
  font-size: 0.95rem;
}
.trusted span {
  color: #4b5563;
  font-size: 0.9rem;
  font-weight: 500;
}
.mockup-wrap {
  position: relative;
}
.mockup-wrap::before {
  content: '';
  position: absolute;
  inset: -18px 10px 10px -18px;
  background: linear-gradient(135deg, #0ea5e9, #2563eb);
  border-radius: 28px;
  transform: rotate(2deg);
  opacity: 0.9;
}
.mockup {
  position: relative;
  z-index: 1;
  background: #fff;
  border-radius: 22px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  padding: 20px;
  border: 1px solid #f3f4f6;
}
.mockup-head {
  border-bottom: 1px solid #f3f4f6;
  padding-bottom: 14px;
}
.virtual-badge {
  background: #d1fae5;
  color: #059669;
  font-size: 0.72rem;
}
.stat-box {
  background: #f9fafb;
  border: 1px solid #f3f4f6;
  border-radius: 14px;
  padding: 14px;
}
.stat-box small {
  display: block;
  color: #6b7280;
  font-size: 0.75rem;
}
.stat-box strong {
  font-size: 1.5rem;
  display: block;
}
.trend {
  font-size: 0.72rem;
  font-weight: 600;
}
.trend.up { color: #10b981; }
.mockup-chart {
  margin-top: 14px;
  border: 1px solid #f3f4f6;
  border-radius: 14px;
  padding: 14px;
}
.bars {
  height: 90px;
}
.bar {
  flex: 1;
  background: linear-gradient(180deg, #0ea5e9, #38bdf8);
  border-radius: 6px 6px 2px 2px;
  opacity: 0.9;
}
.mockup-footer {
  margin-top: 14px;
}
.status-badge {
  color: #059669;
  font-weight: 600;
  font-size: 0.8rem;
}

/* Features */
.features {
  background: #f9fafb;
  padding: 80px 0;
}
.section-head {
  max-width: 640px;
  margin-bottom: 48px;
}
.section-title { font-size: 1.9rem; }
.section-sub { color: #6b7280; font-size: 1.05rem; }
.card {
  border: 1px solid #f3f4f6 !important;
  border-radius: 18px !important;
  padding: 22px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
}
.feature-icon {
  width: 48px;
  height: 48px;
  border-radius: 14px;
  font-size: 1.3rem;
  margin-bottom: 16px;
}
.feature-desc { color: #6b7280; font-size: 0.88rem; margin: 0; }

/* Proof */
.proof {
  padding: 80px 24px;
  background: #fff;
}
.proof-sub {
  max-width: 560px;
}
.proof-row {
  padding: 8px 0;
  border-bottom: 1px dashed #e5e7eb;
  font-size: 0.9rem;
}
.proof-row small { font-weight: 600; }
.progress {
  height: 6px;
  background: #e5e7eb;
  border-radius: 99px;
}
.progress-bar-a { background: #0ea5e9; border-radius: 99px; height: 100%; }
.progress-bar-b { background: #10b981; border-radius: 99px; height: 100%; }
.progress-bar-c { background: #8b5cf6; border-radius: 99px; height: 100%; }
.proof-item { padding: 10px 0; }
.proof-dot {
  width: 42px;
  height: 42px;
  border-radius: 13px;
  background: #e0f2fe;
  font-size: 1.1rem;
  flex-shrink: 0;
}

/* CTA final */
.cta-final {
  position: relative;
  background: linear-gradient(135deg, #0ea5e9, #2563eb);
  margin: 40px auto 80px;
  padding: 64px 24px;
  max-width: 1000px;
  box-shadow: 0 25px 50px -12px rgba(37, 99, 235, 0.45);
}
.cta-title { font-size: 2rem; }
.cta-sub { max-width: 480px; }
.btn-cta-final {
  background: #fff;
  color: #2563eb;
  font-weight: 700;
  border-radius: 12px;
  border: none;
}
.btn-cta-final:hover { background: #f1f5f9; color: #1e40af; }
.btn-outline-light { border-radius: 12px; font-weight: 600; }

/* Footer */
.footer {
  padding: 28px 32px;
  background: #fff;
}
.footer-link {
  text-decoration: none;
  color: inherit;
}
.footer-link:hover { color: #0ea5e9; }

/* Responsive */
@media (max-width: 991px) {
  .hero {
    padding: 48px 20px;
  }
  .hero-title { text-align: center; }
  .hero-sub { margin-left: auto; margin-right: auto; text-align: center; }
  .trusted { justify-content: center; }
}
</style>