<template>
  <span class="avatar" :class="`avatar--${size}`" :title="name || undefined">
    {{ initials }}
  </span>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  name: { type: String, default: '' },
  lastName: { type: String, default: '' },
  size: { type: String, default: 'md' }
})

const initials = computed(() => {
  const parts = `${props.name ?? ''} ${props.lastName ?? ''}`.trim().split(/\s+/).filter(Boolean)
  if (!parts.length) return '—'
  return (parts[0][0] + (parts[1]?.[0] ?? '')).toUpperCase()
})
</script>

<style scoped>
.avatar {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex: none;
  border-radius: var(--app-radius-pill);
  background: linear-gradient(135deg, var(--c-brand-500), var(--c-brand-700));
  color: #fff;
  font-weight: 600;
  letter-spacing: 0.02em;
  line-height: 1;
  user-select: none;
}

.avatar--sm {
  width: 28px;
  height: 28px;
  font-size: 0.7rem;
}
.avatar--md {
  width: 36px;
  height: 36px;
  font-size: 0.8rem;
}
.avatar--lg {
  width: 56px;
  height: 56px;
  font-size: 1.15rem;
}
.avatar--xl {
  width: 84px;
  height: 84px;
  font-size: 1.75rem;
}
</style>
