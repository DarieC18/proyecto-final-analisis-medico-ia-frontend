<template>
  <Teleport to="body">
    <div v-if="visible" class="doc-viewer-overlay" @click.self="$emit('close')">
      <div class="doc-viewer-container">
        <div class="doc-viewer-header">
          <div class="d-flex align-items-center gap-2">
            <div style="width: 24px; color: rgba(255,255,255,0.8);"><FileIcon :file-name="document?.fileName" :file-path="document?.filePath" /></div>
            <h6 class="fw-bold mb-0 text-white">{{ document?.fileName || 'Documento' }}</h6>
          </div>
          <div class="d-flex align-items-center gap-2">
            <a v-if="fullUrl" :href="fullUrl" target="_blank" class="btn btn-sm text-white border px-3 tab-btn" title="Abrir en nueva pestaña">
              Abrir en nueva pestaña
            </a>
            <button @click="$emit('close')" class="btn btn-sm text-white border-0 px-2 fs-4 close-btn">&times;</button>
          </div>
        </div>
        <div class="doc-viewer-body">
          <div v-if="loadError" class="unsupported-container text-center py-5">
            <div class="display-1 mb-3">⚠️</div>
            <h5 class="fw-bold text-dark mb-2">No se pudo cargar el documento</h5>
            <p class="text-muted mb-4">El archivo no está disponible o no se puede acceder desde el navegador.</p>
            <a v-if="fullUrl" :href="fullUrl" target="_blank" class="btn btn-primary px-4 shadow-sm">
              Abrir en nueva pestaña
            </a>
          </div>
          <div v-else-if="isImage" class="image-container">
            <img :src="fullUrl" :alt="document?.fileName" class="doc-image" @error="loadError = true" />
          </div>
          <div v-else-if="isPdf" class="pdf-container">
            <iframe :src="fullUrl" class="doc-iframe" frameborder="0" @error="loadError = true"></iframe>
          </div>
          <div v-else class="unsupported-container text-center py-5">
            <div style="width: 80px; margin: 0 auto 1rem;"><FileIcon :file-name="document?.fileName" :file-path="document?.filePath" /></div>
            <h5 class="fw-bold text-dark mb-2">Tipo de archivo no previsualizable</h5>
            <p class="text-muted mb-4">Este tipo de archivo no se puede visualizar directamente en el navegador.</p>
            <a v-if="fullUrl" :href="fullUrl" target="_blank" class="btn btn-primary px-4 shadow-sm">
              Descargar / Abrir archivo
            </a>
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { computed, ref, watch } from 'vue'
import FileIcon from '@/components/FileIcon.vue'

const props = defineProps({
  visible: Boolean,
  document: { type: Object, default: null }
})

defineEmits(['close'])

const loadError = ref(false)

watch(() => props.document, () => {
  loadError.value = false
})

const fullUrl = computed(() => {
  if (!props.document?.filePath) return null
  const path = props.document.filePath
  if (path.startsWith('http')) return path
  const base = import.meta.env.VITE_API_BASE_URL || ''
  return base + path
})

const getExtension = () => {
  const name = props.document?.fileName || ''
  const path = props.document?.filePath || ''
  const nameExt = name.split('.').pop()?.toLowerCase()
  if (nameExt && name.includes('.')) return nameExt
  const pathExt = path.split('.').pop()?.toLowerCase()
  if (pathExt && path.includes('.')) return pathExt
  return ''
}

const isImage = computed(() => ['jpg', 'jpeg', 'png'].includes(getExtension()))
const isPdf = computed(() => getExtension() === 'pdf')
</script>

<style scoped>
.doc-viewer-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.75);
  z-index: 2000;
  display: flex;
  align-items: center;
  justify-content: center;
  animation: fadeIn 0.2s ease;
}

.doc-viewer-container {
  width: 95vw;
  height: 95vh;
  display: flex;
  flex-direction: column;
  background: #1a1a2e;
  border-radius: 0.75rem;
  overflow: hidden;
  box-shadow: 0 25px 60px rgba(0, 0, 0, 0.5);
}

.doc-viewer-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1.25rem;
  background: #16213e;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  min-height: 52px;
}

.doc-viewer-body {
  flex: 1;
  overflow: hidden;
  background: #f8f9fa;
}

.image-container {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
  overflow: auto;
  background: #1a1a2e;
}

.doc-image {
  max-width: 100%;
  max-height: 100%;
  object-fit: contain;
  border-radius: 0.5rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.pdf-container {
  width: 100%;
  height: 100%;
}

.doc-iframe {
  width: 100%;
  height: 100%;
  border: none;
}

.unsupported-container {
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.2);
}

.tab-btn:hover {
  background: rgba(255, 255, 255, 0.15);
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}
</style>
