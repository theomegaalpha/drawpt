import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { msalPlugin } from './plugins/msalPlugin'
import { type AuthenticationResult, EventType } from '@azure/msal-browser'
import { msalInstance } from './authConfig'

import App from './App.vue'
import router from './router'

// Account selection logic is app dependent. Adjust as needed for different use cases.
const accounts = msalInstance.getAllAccounts()
if (accounts.length > 0) {
  msalInstance.setActiveAccount(accounts[0])
}
msalInstance.addEventCallback((event) => {
  if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
    const payload = event.payload as AuthenticationResult
    const account = payload.account
    msalInstance.setActiveAccount(account)
  }
})

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(msalPlugin, msalInstance)

app.directive('autocapitalize', {
  updated(el) {
    el.value = el.value.toUpperCase()
  },
})

app.mount('#app')
