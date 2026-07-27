<template>
  <div class="file-icon" :class="`file-icon--${type}`">
    <svg v-if="type === 'pdf'" viewBox="0 0 40 48" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M4 0C1.79 0 0 1.79 0 4V44C0 46.21 1.79 48 4 48H36C38.21 48 40 46.21 40 44V16L24 0H4Z" fill="currentColor" opacity="0.15"/>
      <path d="M24 0L40 16H28C25.79 16 24 14.21 24 12V0Z" fill="currentColor" opacity="0.25"/>
      <path d="M24 0L40 16H28C25.79 16 24 14.21 24 12V0Z" stroke="currentColor" stroke-width="0.5" fill="none"/>
      <path d="M4 0C1.79 0 0 1.79 0 4V44C0 46.21 1.79 48 4 48H36C38.21 48 40 46.21 40 44V16L24 0H4Z" stroke="currentColor" stroke-width="1.5" fill="none"/>
      <rect x="8" y="24" width="18" height="3" rx="1.5" fill="currentColor" opacity="0.5"/>
      <rect x="8" y="30" width="14" height="3" rx="1.5" fill="currentColor" opacity="0.35"/>
      <rect x="8" y="36" width="10" height="3" rx="1.5" fill="currentColor" opacity="0.25"/>
      <text x="20" y="20" text-anchor="middle" font-size="7" font-weight="700" fill="currentColor" font-family="sans-serif">PDF</text>
    </svg>

    <svg v-else-if="type === 'image'" viewBox="0 0 40 48" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M4 0C1.79 0 0 1.79 0 4V44C0 46.21 1.79 48 4 48H36C38.21 48 40 46.21 40 44V4C40 1.79 38.21 0 36 0H4Z" fill="currentColor" opacity="0.12"/>
      <path d="M4 0C1.79 0 0 1.79 0 4V44C0 46.21 1.79 48 4 48H36C38.21 48 40 46.21 40 44V4C40 1.79 38.21 0 36 0H4Z" stroke="currentColor" stroke-width="1.5" fill="none"/>
      <circle cx="13" cy="15" r="3.5" stroke="currentColor" stroke-width="1.5" fill="currentColor" opacity="0.2"/>
      <path d="M6 38L14 28L20 34L26 26L34 38" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" fill="currentColor" opacity="0.15"/>
      <path d="M6 38L14 28L20 34L26 26L34 38" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" fill="none"/>
    </svg>

    <svg v-else viewBox="0 0 40 48" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M4 0C1.79 0 0 1.79 0 4V44C0 46.21 1.79 48 4 48H36C38.21 48 40 46.21 40 44V4C40 1.79 38.21 0 36 0H4Z" fill="currentColor" opacity="0.12"/>
      <path d="M4 0C1.79 0 0 1.79 0 4V44C0 46.21 1.79 48 4 48H36C38.21 48 40 46.21 40 44V4C40 1.79 38.21 0 36 0H4Z" stroke="currentColor" stroke-width="1.5" fill="none"/>
      <rect x="8" y="12" width="20" height="2.5" rx="1.25" fill="currentColor" opacity="0.4"/>
      <rect x="8" y="18" width="16" height="2.5" rx="1.25" fill="currentColor" opacity="0.3"/>
      <rect x="8" y="24" width="20" height="2.5" rx="1.25" fill="currentColor" opacity="0.2"/>
      <rect x="8" y="30" width="12" height="2.5" rx="1.25" fill="currentColor" opacity="0.15"/>
    </svg>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  fileName: { type: String, default: '' },
  filePath: { type: String, default: '' },
  size: { type: String, default: 'md' }
})

const type = computed(() => {
  const getExt = () => {
    const nameExt = props.fileName.split('.').pop()?.toLowerCase()
    if (nameExt && props.fileName.includes('.')) return nameExt
    const pathExt = props.filePath.split('.').pop()?.toLowerCase()
    if (pathExt && props.filePath.includes('.')) return pathExt
    return ''
  }
  const ext = getExt()
  if (ext === 'pdf') return 'pdf'
  if (['jpg', 'jpeg', 'png'].includes(ext)) return 'image'
  return 'generic'
})
</script>

<style scoped>
.file-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
}
.file-icon svg {
  width: 100%;
  height: auto;
}
.file-icon--pdf { color: #dc3545; }
.file-icon--image { color: #0d6efd; }
.file-icon--generic { color: #6c757d; }
</style>
