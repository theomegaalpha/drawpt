import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { type AccountInfo } from '@azure/msal-browser'

export const useAccountStore = defineStore('account', () => {
  const account = ref({} as AccountInfo)
  function updateAccount(accountData: AccountInfo) {
    account.value = accountData
  }

  function logout() {
    account.value = {} as AccountInfo
  }

  const isAuthenticated = computed(() => account.value !== null && account.value !== undefined)

  return { updateAccount, logout, account, isAuthenticated }
})
