import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  server: {
    proxy: {
      '/api': {
        target: 'http://medanalyzer-medanalyzerbackendstaging-af-3eb6c0-144-126-151-120.sslip.io',
        changeOrigin: true
      },
      '/uploads': {
        target: 'http://medanalyzer-medanalyzerbackendstaging-af-3eb6c0-144-126-151-120.sslip.io',
        changeOrigin: true
      }
    }
  }
})
