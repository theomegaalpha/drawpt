<script setup lang="ts">
import { computed, ref } from 'vue' // Added ref
import { useAuthStore } from '@/stores/auth'
import { MenuIcon, XIcon } from 'lucide-vue-next' // Added MenuIcon and XIcon

const authStore = useAuthStore()
const isAuthenticated = computed(() => {
  return !!authStore.user
})

const menuItems = [{ name: 'About', href: '/about' }]
const isMobileMenuOpen = ref(false) // State for mobile menu

const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}
</script>

<template>
  <header>
    <nav
      class="group fixed z-20 w-full border border-b border-slate-500/10 bg-white/30 backdrop-blur md:relative dark:border-white/10 dark:bg-zinc-950/50 lg:dark:bg-transparent"
    >
      <div class="m-auto max-w-7xl px-6">
        <div class="flex flex-wrap items-center justify-between gap-6 py-3 lg:gap-0 lg:py-4">
          <div class="flex w-full items-center justify-between lg:w-auto">
            <router-link to="/" aria-label="home" class="flex items-center space-x-2">
              <img src="@/assets/logo.png" alt="logo" class="mr-2 h-12 w-12" />
              <h1 class="text-2xl font-bold text-indigo-600 dark:text-indigo-300">DrawPT</h1>
            </router-link>
            <div class="lg:hidden">
              <button
                @click="toggleMobileMenu"
                aria-label="Toggle menu"
                class="rounded-md p-2 text-gray-700 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-indigo-500 dark:text-gray-300 dark:hover:bg-zinc-800"
              >
                <MenuIcon v-if="!isMobileMenuOpen" class="h-6 w-6" />
                <XIcon v-else class="h-6 w-6" />
              </button>
            </div>
          </div>

          <div
            :class="[
              'w-full lg:flex lg:w-auto lg:items-center lg:pr-4',
              isMobileMenuOpen ? 'block' : 'hidden'
            ]"
          >
            <ul
              class="flex flex-col space-y-6 py-4 text-base lg:flex-row lg:items-center lg:gap-8 lg:space-y-0 lg:py-0 lg:text-sm"
            >
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
