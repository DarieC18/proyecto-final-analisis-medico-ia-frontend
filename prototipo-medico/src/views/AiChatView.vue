<template>
  <div class="container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h3 class="fw-bold mb-0">Chat de Consulta Médica IA</h3>
        <p class="text-muted">Realiza consultas médicas generales asistidas por inteligencia artificial</p>
      </div>
    </div>

    <div class="card shadow-sm border-0 rounded-4">
      <div class="card-body p-4" style="min-height: 500px; display: flex; flex-direction: column;">
        <div class="flex-grow-1 overflow-auto mb-4" ref="chatContainer" style="max-height: 400px; min-height: 350px;">
          <div v-if="mensajes.length === 0" class="text-center py-5 text-muted">
            <div class="display-4 mb-3">🤖</div>
            <h5 class="fw-bold">Asistente Médico IA</h5>
            <p class="text-muted">Pregunta sobre diagnóstico, tratamientos, medicamentos o procedimientos médicos.</p>
          </div>

          <div v-for="(m, idx) in mensajes" :key="idx" class="mb-3" :class="m.role === 'user' ? 'text-end' : 'text-start'">
            <div class="d-inline-block p-3 rounded-4" :class="m.role === 'user' ? 'bg-primary text-white' : 'bg-light'" style="max-width: 80%;">
              <small v-if="m.role === 'assistant'" class="fw-bold text-muted d-block mb-1">🤖 Asistente IA</small>
              <p class="mb-0" style="white-space: pre-wrap;">{{ m.content }}</p>
            </div>
          </div>

          <div v-if="cargando" class="text-center py-3">
            <div class="spinner-border text-primary" role="status"></div>
            <p class="text-muted small mt-2">La IA está analizando tu consulta...</p>
          </div>
        </div>

        <div v-if="error" class="alert alert-danger border-0 rounded-3 py-2 small mb-3">{{ error }}</div>

        <form @submit.prevent="enviarMensaje" class="d-flex gap-2">
          <input
            v-model="mensaje"
            type="text"
            class="form-control bg-light border-0 py-3"
            placeholder="Escribe tu consulta médica aquí..."
            :disabled="cargando"
          >
          <button type="submit" class="btn btn-primary px-4 shadow-sm" :disabled="cargando || !mensaje.trim()">
            <span v-if="cargando" class="spinner-border spinner-border-sm"></span>
            <span v-else>Enviar</span>
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, nextTick } from 'vue'
import { aiChatService } from '@/api/aiChat'

const mensaje = ref('')
const cargando = ref(false)
const error = ref('')
const mensajes = ref([])
const chatContainer = ref(null)

const scrollAbajo = async () => {
  await nextTick()
  if (chatContainer.value) {
    chatContainer.value.scrollTop = chatContainer.value.scrollHeight
  }
}

const enviarMensaje = async () => {
  if (!mensaje.value.trim() || cargando.value) return
  const texto = mensaje.value.trim()
  mensajes.value.push({ role: 'user', content: texto })
  mensaje.value = ''
  cargando.value = true
  error.value = ''
  await scrollAbajo()

  try {
    const res = await aiChatService.ask(texto)
    mensajes.value.push({ role: 'assistant', content: res.data.reply })
  } catch (err) {
    error.value = err.response?.data?.message || 'Error al obtener respuesta de la IA'
  } finally {
    cargando.value = false
    await scrollAbajo()
  }
}
</script>
