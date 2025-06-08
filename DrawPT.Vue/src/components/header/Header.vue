<script setup lang="ts">
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const isAuthenticated = computed(() => {
  return !!authStore.user
})

const menuItems = [{ name: 'About', href: '/about' }]
</script>

<template>
  <header>
    <nav
      class="group fixed z-20 w-full border border-b border-slate-500/10 bg-white/30 backdrop-blur md:relative dark:border-white/10 dark:bg-zinc-950/50 lg:dark:bg-transparent"
    >
      <div class="m-auto max-w-7xl px-6">
        <div class="flex flex-wrap items-center justify-between gap-6 py-3 lg:gap-0 lg:py-4">
          <div class="flex w-full justify-between lg:w-auto">
            <router-link to="/" aria-label="home" class="flex items-center space-x-2">
              <img src="@/assets/logo.png" alt="logo" class="mr-2 h-12 w-12" />
              <h1 class="text-2xl font-bold text-indigo-600 dark:text-indigo-300">DrawPT</h1>
            </router-link>
          </div>

          <div class="lg:pr-4">
            <ul class="space-y-6 text-base lg:flex lg:gap-8 lg:space-y-0 lg:text-sm">
              <li v-for="(item, index) in menuItems" :key="index">
                <router-link
                  :to="item.href"
                  class="text-muted-foreground hover:text-accent-foreground block duration-150"
                >
                  <span>{{ item.name }}</span>
                </router-link>
              </li>
              <li v-if="isAuthenticated">
                <router-link
                  to="/logout"
                  class="text-muted-foreground hover:text-accent-foreground block duration-150"
                >
                  <span>Logout</span>
                </router-link>
              </li>
              <li v-else>
                <router-link
                  to="/login"
                  class="text-muted-foreground hover:text-accent-foreground block duration-150"
                >
                  <span>Login</span>
                </router-link>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </nav>
  </header>
</template>
