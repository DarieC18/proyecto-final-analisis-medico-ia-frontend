// El orden importa: Bootstrap primero, nuestra capa de diseño encima.
import 'bootstrap/dist/css/bootstrap.min.css'
import '@/assets/styles/index.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { initTheme } from '@/composables/useTheme'

initTheme()

const app = createApp(App)

app.use(router)

app.mount('#app')
