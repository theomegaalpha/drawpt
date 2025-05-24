<script setup lang="ts">
import { computed } from 'vue'
import { useMsal } from '@/auth/useMsal'
import { loginRequest } from '@/authConfig'
const { accounts, instance } = useMsal()
const loginRedirect = () => {
  instance.loginRedirect(loginRequest)
}

const isAuthenticated = computed(() => {
  return accounts.value.length > 0
})

const logout = () => {
  instance.logoutRedirect()
}
</script>

<template>
  <div
    class="fixed left-0 right-0 z-50 flex items-center justify-between bg-white px-10 py-4 shadow-md dark:bg-zinc-800"
  >
    <div class="flex items-center">
      <img src="@/assets/logo.png" alt="logo" class="mr-2 h-12 w-12" />
      <h1 class="ml-2 text-2xl font-bold text-gray-800 dark:text-gray-200">DrawPT</h1>
    </div>
    <button
      v-if="isAuthenticated"
      class="flex-item rounded-lg border border-zinc-200 bg-zinc-50 px-4 py-2 shadow hover:bg-zinc-100 dark:border-zinc-700 dark:bg-zinc-800 dark:hover:bg-zinc-700"
      @click="logout"
    >
      Logout
    </button>
    <button
      v-else
      class="flex-item rounded-lg border border-zinc-200 bg-zinc-50 px-4 py-2 shadow hover:bg-zinc-100 dark:border-zinc-700 dark:bg-zinc-800 dark:hover:bg-zinc-700"
      @click="loginRedirect"
    >
      Login
    </button>
  </div>
</template>
