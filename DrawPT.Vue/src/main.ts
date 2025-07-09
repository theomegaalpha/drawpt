import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import Clarity from '@microsoft/clarity'
Clarity.init(import.meta.env.VITE_CLARITY_PROJECT_ID)

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.directive('autocapitalize', {
  updated(el, directive) {
    if (directive.value === false) return
    el.value = el.value.toUpperCase()
  }
})

app.mount('#app')
