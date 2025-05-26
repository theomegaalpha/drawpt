<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const isAuthenticated = computed(() => {
  return !!authStore.user
})

const handleLogin = () => {
  router.push('/login')
}

const handleSignUp = () => {
  router.push('/register')
}

const handleLogout = async () => {
  await authStore.signOut()
  router.push('/')
}
</script>

<template>
  <header class="bg-white p-4 shadow-sm dark:bg-zinc-900">
    <div class="container mx-auto flex items-center justify-between">
      <div class="flex items-center gap-2">
        <img src="@/assets/logo.png" alt="logo" class="mr-2 h-12 w-12" />
        <h1 class="text-2xl font-bold text-indigo-600 dark:text-indigo-300">DrawPT</h1>
      </div>
      <div class="flex gap-2">
        <button
          v-if="isAuthenticated"
          class="flex-item rounded-lg border border-zinc-200 bg-zinc-50 px-4 py-2 shadow hover:bg-zinc-100 dark:border-zinc-700 dark:bg-zinc-800 dark:text-zinc-100 dark:hover:bg-zinc-700"
          @click="handleLogout"
        >
          Logout
        </button>
        <button
          v-else
          class="flex-item rounded-lg border border-zinc-200 bg-zinc-50 px-4 py-2 shadow hover:bg-zinc-100 dark:border-zinc-700 dark:bg-zinc-800 dark:text-zinc-100 dark:hover:bg-zinc-700"
          @click="handleLogin"
        >
          Login
        </button>
        <button
          v-if="!isAuthenticated"
          class="flex-item rounded-lg border border-zinc-200 bg-zinc-50 px-4 py-2 shadow hover:bg-zinc-100 dark:border-zinc-700 dark:bg-zinc-800 dark:text-zinc-100 dark:hover:bg-zinc-700"
          @click="handleSignUp"
        >
          Sign Up
        </button>
      </div>
    </div>
  </header>
</template>
