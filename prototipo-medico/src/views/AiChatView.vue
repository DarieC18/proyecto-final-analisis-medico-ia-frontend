<template>
  <div class="chat-page">
    <PageHeader title="Asistente IA" subtitle="Consulta clínica con inteligencia artificial" :icon="IconAi" />

    <BaseCard flush class="chat-card">
      <div ref="scrollEl" class="chat-messages">
        <EmptyState
          v-if="!messages.length"
          :icon="IconBot"
          title="¿En qué puedo ayudarte?"
          message="Pregunta sobre síntomas, interacciones de medicamentos, guías clínicas o pide ayuda para interpretar resultados."
        />

        <div
          v-for="msg in messages"
          :key="msg.id"
          class="chat-msg"
          :class="`chat-msg--${msg.role}`"
        >
          <IconTile v-if="msg.role === 'assistant'" :icon="IconBot" tone="brand" size="sm" />
          <AvatarInitials v-else :name="auth.user?.name" :last-name="auth.user?.lastName" size="sm" />

          <div class="chat-bubble" :class="{ 'chat-bubble--error': msg.error }">
            <p class="chat-bubble__text">{{ msg.text }}</p>
          </div>
        </div>

        <div v-if="loading" class="chat-msg chat-msg--assistant">
          <IconTile :icon="IconBot" tone="brand" size="sm" />
          <div class="chat-bubble chat-bubble--typing">
            <span class="chat-typing-dot" /><span class="chat-typing-dot" /><span class="chat-typing-dot" />
          </div>
        </div>
      </div>

      <form class="chat-input-bar" @submit.prevent="sendMessage">
        <textarea
          ref="inputEl"
          v-model="draft"
          class="form-control chat-input"
          placeholder="Escribe tu pregunta…"
          rows="1"
          :disabled="loading"
          @keydown.enter.exact.prevent="sendMessage"
        />
        <AppButton type="submit" :icon="IconSend" :loading="loading" :disabled="!draft.trim()" pill>
          Enviar
        </AppButton>
      </form>
    </BaseCard>
  </div>
</template>

<script setup>
import { nextTick, ref } from 'vue'
import { authStore } from '@/stores/auth'
import { aiChatService } from '@/api/aiChat'
import { PageHeader, BaseCard, EmptyState, IconTile, AvatarInitials, AppButton } from '@/components/ui'
import { IconAi, IconBot, IconSend } from '@/lib/icons'

const auth = authStore

const messages = ref([])
const draft = ref('')
const loading = ref(false)
const scrollEl = ref(null)
const inputEl = ref(null)

let nextId = 1

/** Texto de respuesta a partir de un cuerpo ya deserializado. */
const replyFrom = (obj) => obj?.reply ?? obj?.message ?? obj?.response ?? null

/**
 * Texto a mostrar a partir del cuerpo de la respuesta.
 *
 * Aunque el endpoint se anuncia como `text/plain`, responde con
 * `Content-Type: application/json`, así que axios YA lo entrega deserializado:
 * `data` es el objeto `{ reply: "..." }`, no una cadena. Tratarlo como cadena
 * reventaba con "raw.trim is not a function".
 *
 * Se admiten las dos formas de todos modos, porque un error del backend sí
 * llega como texto plano y algunos modelos envuelven el JSON en ```json```.
 */
function extractReply(raw) {
  if (!raw) return ''

  if (typeof raw === 'object') {
    return replyFrom(raw) ?? JSON.stringify(raw, null, 2)
  }

  if (typeof raw !== 'string') return String(raw)

  const cleaned = raw
    .trim()
    .replace(/^```json\s*/i, '')
    .replace(/^```\s*/, '')
    .replace(/\s*```$/, '')
  try {
    return replyFrom(JSON.parse(cleaned)) ?? raw
  } catch {
    return raw
  }
}

/**
 * Mensaje de error legible. El backend responde con varias formas según el
 * fallo: `{ Error }` en los 401/403, `{ detail }` o `{ title }` en los
 * ProblemDetails de ASP.NET, y nada en un timeout (ahí el interceptor de
 * ./api/axios ya sustituye el error por uno con texto propio).
 */
function errorMessage(err) {
  const data = err?.response?.data
  const detalle =
    (typeof data === 'string' && data) ||
    data?.Error ||
    data?.detail ||
    data?.message ||
    data?.title ||
    err?.message

  if (err?.response?.status === 403) {
    return 'Tu usuario no tiene permiso para usar el asistente.'
  }
  return detalle
    ? `No se pudo obtener respuesta del asistente: ${detalle}`
    : 'No se pudo obtener respuesta del asistente. Intenta de nuevo.'
}

async function scrollToBottom() {
  await nextTick()
  if (scrollEl.value) scrollEl.value.scrollTop = scrollEl.value.scrollHeight
}

async function sendMessage() {
  const text = draft.value.trim()
  if (!text || loading.value) return

  messages.value.push({ id: nextId++, role: 'user', text })
  draft.value = ''
  loading.value = true
  scrollToBottom()

  try {
    const res = await aiChatService.ask(text)
    messages.value.push({ id: nextId++, role: 'assistant', text: extractReply(res.data) })
  } catch (err) {
    // Sin esto el fallo real queda invisible: un 500 del backend, un timeout y
    // una caída de red producían todos el mismo mensaje genérico.
    console.error('[AiChat] fallo al consultar el asistente:', err)
    messages.value.push({
      id: nextId++,
      role: 'assistant',
      text: errorMessage(err),
      error: true
    })
  } finally {
    loading.value = false
    scrollToBottom()
    nextTick(() => inputEl.value?.focus())
  }
}
</script>

<style scoped>
.chat-page {
  display: flex;
  flex-direction: column;
  height: calc(100dvh - var(--app-topbar-h) - 5.5rem);
}

.chat-card {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
  overflow: hidden;
}

/* BaseCard envuelve el slot en un <div class="card-body"> sin estilo propio
   cuando `flush`; sin este flex no hereda la altura y el scroll interno no
   funciona. :deep() porque ese div vive en el template de BaseCard, no aquí. */
:deep(.chat-card > div) {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

.chat-messages {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  padding: 1.25rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.chat-msg {
  display: flex;
  align-items: flex-start;
  gap: 0.625rem;
  max-width: 42rem;
}

.chat-msg--user {
  flex-direction: row-reverse;
  align-self: flex-end;
}

.chat-bubble {
  padding: 0.625rem 0.875rem;
  border-radius: var(--app-radius-lg);
  background-color: var(--app-surface-sunken);
  border: 1px solid var(--app-border);
}

.chat-msg--user .chat-bubble {
  background-color: var(--bs-primary-bg-subtle);
  border-color: var(--bs-primary-border-subtle);
}

.chat-bubble--error {
  background-color: var(--bs-danger-bg-subtle);
  border-color: var(--bs-danger-border-subtle);
  color: var(--bs-danger-text-emphasis);
}

.chat-bubble__text {
  margin: 0;
  white-space: pre-wrap;
  font-size: 0.9rem;
  line-height: 1.5;
}

.chat-bubble--typing {
  display: flex;
  align-items: center;
  gap: 0.3rem;
  padding: 0.75rem 1rem;
}

.chat-typing-dot {
  width: 0.4rem;
  height: 0.4rem;
  border-radius: 50%;
  background-color: var(--app-text-subtle);
  animation: chat-typing 1s infinite ease-in-out;
}
.chat-typing-dot:nth-child(2) {
  animation-delay: 0.15s;
}
.chat-typing-dot:nth-child(3) {
  animation-delay: 0.3s;
}

@keyframes chat-typing {
  0%,
  60%,
  100% {
    opacity: 0.35;
    transform: translateY(0);
  }
  30% {
    opacity: 1;
    transform: translateY(-2px);
  }
}

.chat-input-bar {
  flex: none;
  display: flex;
  align-items: flex-end;
  gap: 0.625rem;
  padding: 0.875rem 1.25rem;
  border-top: 1px solid var(--app-border);
  background-color: var(--app-surface);
}

.chat-input {
  resize: none;
  max-height: 6.5rem;
}
</style>
