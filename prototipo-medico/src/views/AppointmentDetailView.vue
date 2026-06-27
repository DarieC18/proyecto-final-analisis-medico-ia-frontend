<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Expediente Clínico: Juan Pérez</h3>
        <p class="text-muted">Cita #4829 - Pendiente</p>
      </div>
      <RouterLink to="/citas" class="btn btn-light border text-muted shadow-sm">Volver al listado</RouterLink>
    </div>

    <div class="card shadow-sm p-2 mb-4">
      <ul class="nav nav-pills nav-fill gap-2 p-1">
        <li class="nav-item">
          <a class="nav-link rounded-pill fw-medium" :class="{ 'active bg-primary shadow-sm': tabActual === 'info', 'text-muted': tabActual !== 'info' }" @click="tabActual = 'info'" href="#">📝 Info Básica</a>
        </li>
        <li class="nav-item">
          <a class="nav-link rounded-pill fw-medium" :class="{ 'active bg-primary shadow-sm': tabActual === 'sintomas', 'text-muted': tabActual !== 'sintomas' }" @click="tabActual = 'sintomas'" href="#">🤒 Síntomas</a>
        </li>
        <li class="nav-item">
          <a class="nav-link rounded-pill fw-medium" :class="{ 'active bg-primary shadow-sm': tabActual === 'signos', 'text-muted': tabActual !== 'signos' }" @click="tabActual = 'signos'" href="#">❤️ Signos Vitales</a>
        </li>
        <li class="nav-item">
          <a class="nav-link rounded-pill fw-bold" :class="{ 'active bg-dark text-white shadow-sm': tabActual === 'ia', 'text-primary bg-primary bg-opacity-10': tabActual !== 'ia' }" @click="tabActual = 'ia'" href="#">🤖 Análisis IA</a>
        </li>
      </ul>
    </div>

    <div class="card shadow-sm">
      <div class="card-body p-5">

        <div v-if="tabActual === 'info'" class="animation-fade">
          <h5 class="fw-bold text-dark mb-4">Notas de Evolución</h5>
          <textarea v-model="notas" class="form-control bg-light border-0 p-4 rounded-4 mb-4" rows="4" placeholder="Redacte el historial o evolución médica aquí..."></textarea>
          <div class="d-flex justify-content-end">
            <button @click="guardarNotas" class="btn btn-primary px-4 py-2" :disabled="!notas">Guardar Registro</button>
          </div>
          <div v-if="notasGuardadas" class="alert alert-success border-0 rounded-3 mt-3 small">Notas guardadas exitosamente.</div>
        </div>

        <div v-if="tabActual === 'sintomas'" class="animation-fade">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h5 class="fw-bold text-dark mb-0">Registro de Síntomas</h5>
            <button @click="agregarSintoma" class="btn btn-primary btn-sm px-3 shadow-sm">+ Agregar Síntoma</button>
          </div>

          <div v-if="nuevoSintoma" class="card bg-light border-0 p-4 rounded-4 mb-4">
            <div class="row g-3">
              <div class="col-md-4">
                <label class="form-label text-muted small fw-bold text-uppercase">Síntoma</label>
                <input v-model="sintomaForm.nombre" type="text" class="form-control bg-white border-0" placeholder="Ej: Fiebre">
              </div>
              <div class="col-md-3">
                <label class="form-label text-muted small fw-bold text-uppercase">Severidad</label>
                <select v-model="sintomaForm.severidad" class="form-select bg-white border-0">
                  <option value="Leve">Leve</option>
                  <option value="Moderado">Moderado</option>
                  <option value="Severo">Severo</option>
                </select>
              </div>
              <div class="col-md-3">
                <label class="form-label text-muted small fw-bold text-uppercase">Inicio</label>
                <input v-model="sintomaForm.inicio" type="date" class="form-control bg-white border-0">
              </div>
              <div class="col-md-2 d-flex align-items-end">
                <button @click="guardarSintoma" class="btn btn-success w-100">Agregar</button>
              </div>
            </div>
          </div>

          <div class="row g-4">
            <div class="col-md-6" v-for="(s, idx) in sintomas" :key="idx">
              <div class="card bg-light border-0 border-start border-4 shadow-sm h-100 p-3 rounded-4" :class="`border-${s.color}`">
                <div class="d-flex justify-content-between align-items-start mb-2">
                  <h6 class="fw-bold mb-0">{{ s.nombre }}</h6>
                  <span class="badge rounded-pill px-3" :class="s.badgeClass">{{ s.severidad }}</span>
                </div>
                <p class="text-muted small mb-2"><strong>Inicio:</strong> {{ s.inicio }}</p>
                <p class="mb-0 text-dark small">{{ s.descripcion }}</p>
              </div>
            </div>
          </div>
        </div>

        <div v-if="tabActual === 'signos'" class="animation-fade">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <div>
              <h5 class="fw-bold text-dark mb-0">Mediciones Clínicas</h5>
              <p class="text-muted small mb-0">Última toma: 16 Jun 2026, 10:15 AM</p>
            </div>
            <button class="btn btn-primary btn-sm px-3 shadow-sm">+ Registrar Medición</button>
          </div>

          <div class="row g-4 text-center mt-2">
            <div class="col-md-4 col-sm-6">
              <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                <div class="display-6 mb-2">🌡️</div>
                <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Temperatura</h6>
                <h3 class="fw-bold text-danger mb-0">39.5 °C</h3>
              </div>
            </div>
            <div class="col-md-4 col-sm-6">
              <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                <div class="display-6 mb-2">💓</div>
                <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Frec. Cardíaca</h6>
                <h3 class="fw-bold text-dark mb-0">110 lpm</h3>
              </div>
            </div>
            <div class="col-md-4 col-sm-6">
              <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                <div class="display-6 mb-2">🩸</div>
                <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Presión Arterial</h6>
                <h3 class="fw-bold text-dark mb-0">120/80</h3>
                <small class="text-muted fw-medium">mmHg</small>
              </div>
            </div>
            <div class="col-md-6 col-sm-6">
              <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                <div class="display-6 mb-2">🫁</div>
                <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Saturación O2</h6>
                <h3 class="fw-bold text-dark mb-0">97 %</h3>
              </div>
            </div>
            <div class="col-md-6 col-sm-6">
              <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                <div class="display-6 mb-2">🍬</div>
                <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Glucosa</h6>
                <h3 class="fw-bold text-dark mb-0">95</h3>
                <small class="text-muted fw-medium">mg/dL</small>
              </div>
            </div>
          </div>
        </div>

        <div v-if="tabActual === 'ia'" class="animation-fade">
          <div class="text-center py-5" v-if="!analisisGenerado">
            <div class="display-4 mb-3">🧠</div>
            <h4 class="fw-bold">Asistente de Diagnóstico</h4>
            <p class="text-muted w-50 mx-auto">La Inteligencia Artificial analizará el cuadro clínico, signos vitales y síntomas para sugerir recomendaciones y calcular factores de riesgo.</p>
            <button @click="generarIA" class="btn btn-dark btn-lg px-5 mt-3 shadow">
              <span v-if="cargando" class="spinner-border spinner-border-sm me-2"></span>
              <span v-if="cargando">Procesando modelo...</span>
              <span v-else>Generar Reporte IA</span>
            </button>
          </div>

          <div v-else class="bg-light p-4 rounded-4 border-start border-5 border-info position-relative">
            <span class="badge bg-info position-absolute top-0 end-0 m-3 px-3 py-2 rounded-pill">Riesgo Medio</span>
            <h5 class="fw-bold text-info mb-3">🤖 Resultado del Análisis Clínico</h5>
            <div class="mb-3">
              <h6 class="fw-bold text-dark mb-1">Resumen Estructurado:</h6>
              <p class="text-muted">Paciente presenta cuadro febril agudo con mareos severos. Los signos vitales sugieren una posible infección viral que requiere monitoreo.</p>
            </div>
            <div class="mb-4">
              <h6 class="fw-bold text-dark mb-1">Recomendaciones sugeridas:</h6>
              <ul class="text-muted text-start d-inline-block">
                <li>Hidratación constante (vía intravenosa si no hay tolerancia oral).</li>
                <li>Realizar panel viral y hemograma completo.</li>
                <li>Monitoreo de curva térmica cada 4 horas.</li>
              </ul>
            </div>
            <div class="alert alert-warning mb-0 border-0 rounded-3 shadow-sm text-center">
              <small class="fw-bold">⚠️ AVISO LEGAL:</small>
              <small>Este análisis es generado por IA para asistir la decisión médica y no sustituye el diagnóstico oficial de un profesional de la salud.</small>
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'

const tabActual = ref('info')
const cargando = ref(false)
const analisisGenerado = ref(false)
const notas = ref('')
const notasGuardadas = ref(false)
const nuevoSintoma = ref(false)
const sintomas = ref([
  { nombre: 'Fiebre y Escalofríos', severidad: 'Severo', inicio: '14 Jun 2026', descripcion: 'El paciente reporta picos de fiebre de hasta 39.5°C principalmente por las noches, acompañados de sudoración.', color: 'danger', badgeClass: 'bg-danger' },
  { nombre: 'Mareos y Fatiga', severidad: 'Moderado', inicio: '15 Jun 2026', descripcion: 'Sensación de vértigo al levantarse bruscamente de la cama. Debilidad generalizada.', color: 'warning', badgeClass: 'bg-warning text-dark' }
])

const sintomaForm = reactive({
  nombre: '', severidad: 'Moderado', inicio: ''
})

const guardarNotas = () => {
  notasGuardadas.value = true
  setTimeout(() => { notasGuardadas.value = false }, 3000)
}

const agregarSintoma = () => {
  nuevoSintoma.value = !nuevoSintoma.value
}

const guardarSintoma = () => {
  if (!sintomaForm.nombre || !sintomaForm.inicio) return
  const colorMap = { Leve: 'success', Moderado: 'warning', Severo: 'danger' }
  const badgeMap = { Leve: 'bg-success', Moderado: 'bg-warning text-dark', Severo: 'bg-danger' }
  sintomas.value.push({
    nombre: sintomaForm.nombre,
    severidad: sintomaForm.severidad,
    inicio: new Date(sintomaForm.inicio).toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' }),
    descripcion: 'Síntoma registrado durante la consulta.',
    color: colorMap[sintomaForm.severidad],
    badgeClass: badgeMap[sintomaForm.severidad]
  })
  sintomaForm.nombre = ''
  sintomaForm.inicio = ''
  nuevoSintoma.value = false
}

const generarIA = () => {
  cargando.value = true
  setTimeout(() => {
    cargando.value = false
    analisisGenerado.value = true
  }, 2000)
}
</script>

<style scoped>
.animation-fade { animation: fadeIn 0.3s ease-in-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(5px); } to { opacity: 1; transform: translateY(0); } }
</style>
