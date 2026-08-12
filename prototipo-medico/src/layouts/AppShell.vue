<template>
  <div class="app-shell">
    <AppSidebar />
    <AppDrawer />

    <div class="app-shell__main">
      <AppTopbar />
      <main class="app-content">
        <slot />
      </main>
    </div>
  </div>
</template>

<script setup>
import AppDrawer from './AppDrawer.vue'
import AppSidebar from './AppSidebar.vue'
import AppTopbar from './AppTopbar.vue'
</script>

<style scoped>
/* Grid de dos columnas en escritorio, una sola en móvil. El sidebar es sticky
   con altura de viewport propia (ver AppSidebar) y la topbar es sticky dentro
   de la columna de contenido: entre las dos eliminan cualquier necesidad de
   compensar alturas a mano. */
.app-shell {
  display: grid;
  grid-template-columns: 1fr;
  min-height: 100dvh;
  background-color: var(--app-canvas);
}

@media (min-width: 992px) {
  .app-shell {
    grid-template-columns: var(--app-sidebar-w) minmax(0, 1fr);
  }
}

.app-shell__main {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.app-content {
  flex: 1;
  width: 100%;
  max-width: var(--app-content-max);
  margin-inline: auto;
  padding: 1.5rem 1rem 4rem;
}

@media (min-width: 768px) {
  .app-content {
    padding-inline: 1.5rem;
  }
}
</style>
